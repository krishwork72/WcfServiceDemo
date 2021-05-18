using System.Collections.Generic;
using System.Net;

namespace MessageInterceptor.Core.Models
{
    public class RequestModel
    {
        public RequestModel()
        {

        }
        public string Schema { get; set; }
        public string Host { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<HeaderModel> Headers { get; set; }
        public string Payload { get; set; }
    }
}