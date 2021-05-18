using MessageInterceptor.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MessageInterceptor.Core
{
    public class ServiceInterceptorAttribute : Attribute, IActionFilter
    {
        private readonly IOptions<List<ExcludeSetting>> excludeSettings;
        private bool ignoreAction = true;
        public ServiceInterceptorAttribute(IOptions<List<ExcludeSetting>>  excludeSettings)
        {
            this.excludeSettings = excludeSettings;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            try
            {
                GetIgnoreAction(filterContext.ActionDescriptor);
                if (ignoreAction)
                {
                    LogWriter.Log("Skipping.");
                    return;
                }
                var model = new RequestModel();
                model.Schema = httpContext.Request.Scheme;
                model.Host = httpContext.Request.Host.Value;
                model.Url = httpContext.Request.Path.Value;
                model.Method = httpContext.Request.Method;
                model.Headers = GetHeaders(httpContext.Request);
                model.Payload = GetPayload(filterContext);
                LogWriter.Log(model);
            }
            finally {}
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            try
            {
                GetIgnoreAction(filterContext.ActionDescriptor);
                if (ignoreAction)
                {
                    LogWriter.Log("Skipping.");
                    return;
                }
                var model = new RequestModel();
                model.Schema = httpContext.Request.Scheme;
                model.Host = httpContext.Request.Host.Value;
                model.Url = httpContext.Request.Path.Value;
                model.Method = httpContext.Request.Method;
                model.StatusCode = (HttpStatusCode)httpContext.Response.StatusCode;
                model.Headers = GetHeaders(httpContext.Response);
                model.Payload = GetPayload(filterContext);
                LogWriter.Log(model);
            }
            finally { }
        }
        private void GetIgnoreAction(ActionDescriptor actionDescriptor)
        {
            var excludes = excludeSettings.Value;
           
            if (excludes.Count == 0 )
            {
                LogWriter.Log($"The exclude method setting count:{excludes.Count}");
                ignoreAction = false;
                return;
            }
            var actionName = ((ControllerActionDescriptor)actionDescriptor).ActionName;
            var ignoreActionSetting = excludes.FirstOrDefault(x 
                                    =>x.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
            if (ignoreActionSetting == null)
            {
                ignoreAction = false;
            }
        }
        private string GetPayload(ActionExecutingContext filterContext)
        {
            var sb = new StringBuilder();
            if (filterContext != null)
            {
                var count = filterContext.ActionArguments.Count;
                var index = 0;
                foreach (var argument in filterContext.ActionArguments)
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
        private List<HeaderModel> GetHeaders(HttpRequest httpRequest)
        {
            List<HeaderModel> headers = new List<HeaderModel>();
            if (httpRequest != null)
            {
                headers.Add(new HeaderModel()
                {
                    Name = "ContentType",
                    Value = httpRequest.ContentType
                });
                headers.Add(new HeaderModel()
                {
                    Name = "ContentLength",
                    Value = Convert.ToString(httpRequest.ContentLength)
                });
                foreach (var header in httpRequest.Headers)
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
        private string GetPayload(ActionExecutedContext filterContext)
        {
            var sb = new StringBuilder();
            if (filterContext != null)
            {
                sb.Append($"{JsonConvert.SerializeObject(filterContext.Result)}");
            }
            return sb.ToString();
        }
        private List<HeaderModel> GetHeaders(HttpResponse httpResponse)
        {
            List<HeaderModel> headers = new List<HeaderModel>();
            if (httpResponse != null)
            {
                headers.Add(new HeaderModel()
                {
                    Name = "ContentType",
                    Value = httpResponse.ContentType
                });
                headers.Add(new HeaderModel()
                {
                    Name = "ContentLength",
                    Value = Convert.ToString(httpResponse.ContentLength)
                });
                foreach (var header in httpResponse.Headers)
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
