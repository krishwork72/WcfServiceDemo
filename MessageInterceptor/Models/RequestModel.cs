using System.Collections.Generic;
using System.Net;

namespace MessageInterceptor.Models
{
    public class RequestModel
    {
        public RequestModel()
        {

        }
        public string Url { get; set; }
        public string Method { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<HeaderModel> Headers { get; set; }
        public string Payload { get; set; }
    }
}