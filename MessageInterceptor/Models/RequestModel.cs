using System.Collections.Generic;
using System.Net;

namespace MessageInterceptor.Models
{
    internal class RequestModel
    {
        public RequestModel()
        {
            Headers = new List<HeaderModel>();
            Payloads = new List<Payloads>();
            Exceptions = new List<ServiceException>();
        }
        public string Url { get; set; }
        public string Method { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<HeaderModel> Headers { get; set; }
        public List<Payloads> Payloads { get; set; }
        public List<ServiceException> Exceptions { get; set; }
    }
}