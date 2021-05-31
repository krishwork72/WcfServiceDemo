using MessageInterceptor.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MessageInterceptor.Core
{
    public class ServiceInterceptorAttribute : Attribute, IActionFilter
    {
        private readonly AssemblyHelper assemblyHelper;
        private const string RequestPayload = "RequestPayload";
        private readonly ICheckInterceptor checkInterceptor;

        public ServiceInterceptorAttribute(IOptions<AssemblyInfo> options)
        {
            this.assemblyHelper = new AssemblyHelper(options);
            checkInterceptor= assemblyHelper.CreateInstance<ICheckInterceptor>();
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            const string Type = "Request";
            var httpContext = filterContext.HttpContext;
            RequestModel model = null;
            try
            {
                var headers = GetHeaders(httpContext.Request);
                var payload = GetPayload(filterContext);
                if (!DoIntercept(headers, filterContext.ActionArguments))
                    return;

                model = new RequestModel();
                model.Schema = httpContext.Request.Scheme;
                model.Host = httpContext.Request.Host.Value;
                model.Url = httpContext.Request.Path.Value;
                model.Method = httpContext.Request.Method;
                model.Headers = GetHeaders(httpContext.Request);
                model.Payloads.Add(new Payloads()
                {
                    Type = Type,
                    Payload = GetPayload(filterContext)
                });
            }
            catch (Exception ex)
            {
                model?.Exceptions.Add(new ServiceException()
                {
                    Type = Type,
                    Exception = ex
                });
            }
            finally
            {
                // Returns:
                //     true if the element is successfully removed; otherwise, false. This method also
                //     returns false if key was not found in the Properties
                filterContext.ActionDescriptor.Properties.Remove(RequestPayload);
                // Add
                filterContext.ActionDescriptor.Properties.Add(RequestPayload, model);
            }
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            const string Type = "Response";
            RequestModel model = null;
            try
            {
                if (filterContext.ActionDescriptor.Properties.TryGetValue(RequestPayload, out object _requestModel))
                    model = (RequestModel)_requestModel;

                if (model == null)
                    return;

                model.StatusCode = (HttpStatusCode)httpContext.Response.StatusCode;
                model.Payloads.Add(new Payloads()
                {
                    Type = Type,
                    Payload = GetPayload(filterContext)
                });
            }
            catch (Exception ex)
            {
                model?.Exceptions.Add(new ServiceException()
                {
                    Type = Type,
                    Exception = ex
                });
            }
            finally
            {
                try
                {
                    LogWriter.Log(model);
                }
                catch { }
            }
        }
        private bool DoIntercept(List<HeaderModel> headers, IDictionary<string, object> payloads)
        {
            if (checkInterceptor == null)
            {
                return true;
            }
            return checkInterceptor.DoIntercept(headers, payloads);
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
    }
}
