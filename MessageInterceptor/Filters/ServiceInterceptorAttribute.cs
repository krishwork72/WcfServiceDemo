using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MessageInterceptor.Configurations;
using MessageInterceptor.Models;

namespace MessageInterceptor.Filters
{
    public class ServiceInterceptorAttribute : ActionFilterAttribute, IActionFilter
    {
        private bool ignoreAction = true;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var model = new RequestModel();
            var request = actionContext.Request;
            try
            {
                GetIgnoreAction(actionContext.ActionDescriptor);
                if (ignoreAction)
                {
                    return;
                }
                model.Url = request.RequestUri.AbsoluteUri;
                model.Method = request.Method.Method;
                model.Headers = GetHeaders(request);
                model.Payload = GetPayload(actionContext);
                LogWriter.Log(model);
            }
            finally
            {
                ignoreAction = true;
                base.OnActionExecuting(actionContext);
            }

        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var model = new RequestModel();
            var httpResponse = actionExecutedContext.Response;
            try
            {
                GetIgnoreAction(actionExecutedContext.Request.GetActionDescriptor());
                if (ignoreAction)
                {
                    return;
                }
                model.Url = actionExecutedContext.Request.RequestUri.AbsoluteUri;
                model.Method = actionExecutedContext.Request.Method.Method;
                model.StatusCode = httpResponse.StatusCode;
                model.Headers = GetHeaders(httpResponse);
                model.Payload = httpResponse.Content.ReadAsStringAsync().Result;
                LogWriter.Log(model);
            }
            finally
            {
                ignoreAction = true;
                base.OnActionExecuted(actionExecutedContext);
            }

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
        private void GetIgnoreAction(HttpActionDescriptor actionDescriptor)
        {
            var excludeSettings = ExcludeSettingFactory.ExcludeSettings;
            if (excludeSettings.Count == 0 || string.IsNullOrEmpty(actionDescriptor.ActionName))
            {
                LogWriter.Log($"The exclude method setting count:{excludeSettings.Count}");
                ignoreAction = false;
                return;
            }
            var ignoreActionSetting = excludeSettings.FirstOrDefault(x =>
                               x.MethodName.Equals(actionDescriptor.ActionName, StringComparison.OrdinalIgnoreCase));
            if (ignoreActionSetting == null)
            {
                ignoreAction = false;
            }
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
        private List<HeaderModel> GetHeaders(HttpResponseMessage httpResponse)
        {
            List<HeaderModel> headers = new List<HeaderModel>();
            if (httpResponse != null)
            {
                foreach (var header in httpResponse.Headers)
                {
                    headers.Add(new HeaderModel()
                    {
                        Name = header.Key,
                        Value = String.Join(",", header.Value)
                    });
                }
                foreach (var header in httpResponse.Content.Headers)
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
    }
}