using MessageInterceptor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Xml;

namespace MessageInterceptor
{
    internal class MessageInspector : IDispatchMessageInspector
    {
        private readonly ICheckWcfInterceptor checkInterceptor;

        public MessageInspector()
        {
            checkInterceptor = AssemblyHelper.CreateInstance<ICheckWcfInterceptor>();
        }
        #region IDispatchMessageInspector Members
        public object AfterReceiveRequest(ref Message request, IClientChannel channel,
            InstanceContext instanceContext)
        {
            var model = new RequestModel();
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            var context = WebOperationContext.Current;
            const string type = "Request";
            try
            {
                var headers = GetHeaders(context);
                if (!DoIntercept(headers, buffer))
                {
                    return null;
                }
                model.Url = GetRequestUrl();
                model.Method = context.IncomingRequest.Method;
                model.Headers = headers;
                model.Payloads.Add(new Payloads()
                {
                    Type = type,
                    Payload = GetPayload(buffer)
                });
            }
            catch (Exception ex)
            {
                model.Exceptions.Add(new ServiceException()
                {
                    Type = type,
                    Exception = ex
                });
            }
            finally
            {
                request = buffer.CreateMessage();
            }
            return model;
        }
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            var context = WebOperationContext.Current;
            var model = (RequestModel)correlationState;
            const string type = "Response";
            try
            {
                if (model == null)
                    return;

                var messageCopy = buffer.CreateMessage();
                model.StatusCode = context.OutgoingResponse.StatusCode;
                model.Payloads.Add(new Payloads()
                {
                    Type = type,
                    Payload = GetPayload(buffer)
                });
            }
            catch (Exception ex)
            {
                model.Exceptions.Add(new ServiceException()
                {
                    Type = type,
                    Exception = ex
                });
            }
            finally
            {
                LogWriter.Log(model);
                reply = buffer.CreateMessage();
            }
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
        private List<HeaderModel> GetHeaders(WebOperationContext context)
        {
            var result = new List<HeaderModel>();
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
            return result;
        }
        private bool DoIntercept(List<HeaderModel> headers, MessageBuffer messageBuffer)
        {
            if (checkInterceptor == null)
            {
                return true;
            }
            return checkInterceptor.DoIntercept(headers, messageBuffer);
        }
        #region GetPayload
        private string GetPayload(MessageBuffer messageBuffer)
        {
            if (messageBuffer == null)
                return string.Empty;

            //Creating message from messagebuffer
            var ms = messageBuffer.CreateMessage();
            if (ms.IsEmpty)
                return string.Empty;

            //Setup StringWriter to use as input for our StreamWriter
            //This is needed in order to capture the body of the message, because the body is streamed.
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    ms.WriteMessage(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                    //convert xmlTextWriter to result string
                    return stringWriter.ToString();
                };
            }
        }
        #endregion
    }
}
