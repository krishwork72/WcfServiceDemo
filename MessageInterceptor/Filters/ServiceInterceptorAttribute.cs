using MessageInterceptor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MessageInterceptor.Filters
{
    public class ServiceInterceptorAttribute : ActionFilterAttribute, IActionFilter
    {
        private const string RequestPayload = "RequestPayload";
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            RequestModel model = null;
            var request = actionContext.Request;
            const string type = "Request";
            try
            {
                var headers = GetHeaders(request);
                var payload = GetPayload(actionContext);
                if (!DoIntercept(headers, payload))
                    return;

                model = new RequestModel();
                model.Url = request.RequestUri.AbsoluteUri;
                model.Method = request.Method.Method;
                model.Headers = headers;
                model.Payloads.Add(new Payloads()
                {
                    Type = type,
                    Payload = payload
                });
            }
            catch (Exception ex)
            {
                model?.Exceptions.Add(new ServiceException()
                {
                    Type = type,
                    Exception = ex
                });
            }
            finally
            {
                actionContext.Request.Properties.Add(RequestPayload, model);
                base.OnActionExecuting(actionContext);
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            RequestModel model = null;
            var httpResponse = actionExecutedContext.Response;
            const string type = "Response";
            try
            {
                if (actionExecutedContext.Request.Properties.TryGetValue(RequestPayload, out object _requestModel))
                    model = (RequestModel)_requestModel;

                if (model == null)
                    return;

                model.StatusCode = httpResponse.StatusCode;
                model.Payloads.Add(new Payloads()
                {
                    Type = type,
                    Payload = httpResponse.Content.ReadAsStringAsync().Result
                });
            }
            catch (Exception ex)
            {
                model?.Exceptions.Add(new ServiceException()
                {
                    Type = type,
                    Exception = ex
                });
            }
            finally
            {
                base.OnActionExecuted(actionExecutedContext);
            }
            LogWriter.Log(model);
        }
        private string GetPayload(HttpActionContext actionContext)
        {
            var sb = new StringBuilder();
            if (actionContext != null)
            {
                var count = actionContext.ActionArguments.Count;
                var index = 0;
                foreach (var argument in actionContext.ActionArguments)
                {
                    sb.Append($"{JsonConvert.SerializeObject(argument.Value)}");
                    index++;
                    if (index < count)
                    {
                        sb.Append(",");
                    }
                }
            }
            return sb.ToString();
        }
        private List<HeaderModel> GetHeaders(HttpRequestMessage httpRequest)
        {
            List<HeaderModel> headers = new List<HeaderModel>();
            if (httpRequest != null)
            {
                foreach (var header in httpRequest.Headers)
                {
                    headers.Add(new HeaderModel()
                    {
                        Name = header.Key,
                        Value = String.Join(",", header.Value)
                    });
                }
                foreach (var header in httpRequest.Content.Headers)
                {
                    headers.Add(new HeaderModel()
                    {
                        Name = header.Key,
                        Value = String.Join(",", header.Value)
                    });
                }
            }
            return headers;
        }
        private bool DoIntercept(List<HeaderModel> headers, string payload)
        {
            var instance = AssemblyHelper.CreateInstance<ICheckApiInterceptor>();
            if (instance == null)
            {
                return true;
            }
            return instance.DoIntercept(headers, payload);
        }
    }
}