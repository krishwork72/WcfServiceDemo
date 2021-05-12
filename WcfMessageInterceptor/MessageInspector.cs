using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using WcfMessageInterceptor;
using WcfMessageInterceptorDemo.Models;

namespace WcfMessageInterceptorDemo
{
    internal class MessageInspector : IDispatchMessageInspector
    {
        private Type serviceType;
        private bool ignoreMethod = false;
        public MessageInspector(Type serviceType) => this.serviceType = serviceType;

        #region IDispatchMessageInspector Members
        public object AfterReceiveRequest(ref Message request, IClientChannel channel,
            InstanceContext instanceContext)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            var context = WebOperationContext.Current;
            try
            {
                SetIgnoreMethod(context, buffer);
                if (ignoreMethod && context != null)
                {
                    var requestModel = new RequestModel()
                    {
                        Url = GetRequestUrl(),
                        Method = context.IncomingRequest.Method,
                        Headers = GetHeaders(context),
                        Payload = request.ToString()
                    };
                    LogWriter.Log(requestModel);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
            }
            finally
            {
                ignoreMethod = false;
                request = buffer.CreateMessage();
            }
            return null;
        }
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            var context = WebOperationContext.Current;
            try
            {
                var messageCopy = buffer.CreateMessage();
                SetIgnoreMethod(context, buffer);
                if (ignoreMethod && context != null)
                {
                    var responseModel = new RequestModel()
                    {
                        Url = GetRequestUrl(),
                        Method = context.IncomingRequest.Method,
                        StatusCode = context.OutgoingResponse.StatusCode,
                        Headers = GetHeaders(context, true),
                        Payload = messageCopy.ToString()
                    };
                    LogWriter.Log(responseModel);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
            }
            finally
            {
                ignoreMethod = false;
                reply = buffer.CreateMessage();
            }
        }
        #endregion
        #region Exclude method to execution
        private void SetIgnoreMethod(WebOperationContext context, MessageBuffer messageBuffer)
        {
            if (serviceType == null)
            {
                ignoreMethod = true;
                return;
            }
            var currentMethod = serviceType.GetMethod(GetMethodName(context, messageBuffer));
            if (currentMethod == null)
            {
                return;
            }
            var customExcludeAttribute = currentMethod.GetCustomAttributes(true)
                                                      .OfType<CustomExcludeServiceBehaviorAttribute>()
                             .FirstOrDefault();
            if (customExcludeAttribute == null)
            {
                ignoreMethod = true;
            }
        }
        private string GetMethodName(WebOperationContext context, MessageBuffer messageBuffer)
        {
            var methodName = string.Empty;
            if (context.IncomingRequest != null && context.IncomingRequest.UriTemplateMatch != null)
            {
                var match = context.IncomingRequest.UriTemplateMatch;
                if (match.Data != null)
                {
                    methodName = match.Data.ToString();
                }
            }
            var messageCopy = messageBuffer.CreateMessage();
            if (string.IsNullOrEmpty(methodName) && !messageCopy.IsEmpty)
            {
                XmlDictionaryReader bodyReader = messageCopy.GetReaderAtBodyContents();
                methodName = bodyReader.Name.Replace("Response",string.Empty);
            }
            return methodName;
        }
        #endregion
        #region Reading request url
        private string GetRequestUrl()
        {
            var context = OperationContext.Current;
            if (context.RequestContext == null
                || context.RequestContext.RequestMessage == null)
            {
                return string.Empty;
            }
            return context.RequestContext.RequestMessage.Properties.Via.AbsoluteUri;
        }
        #endregion
        private List<HeaderModel> GetHeaders(WebOperationContext context, bool response = false)
        {
            var result = new List<HeaderModel>();
            if (response)
            {
                var headers = context.OutgoingResponse.Headers;
                result.Add(new HeaderModel()
                {
                    Name = "ContentType",
                    Value = context.OutgoingResponse.ContentType
                });
                foreach (var headerKey in headers.AllKeys)
                {
                    result.Add(new HeaderModel()
                    {
                        Name = headerKey,
                        Value = headers[headerKey]
                    });
                }
            }
            else
            {
                var headers = context.IncomingRequest.Headers;
                result.Add(new HeaderModel()
                {
                    Name = "ContentType",
                    Value = context.IncomingRequest.ContentType
                });
                foreach (var headerKey in headers.AllKeys)
                {
                    result.Add(new HeaderModel()
                    {
                        Name = headerKey,
                        Value = headers[headerKey]
                    });
                }
               
            }
            return result;
        }
    }
}
