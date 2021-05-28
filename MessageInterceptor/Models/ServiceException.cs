using System;

namespace MessageInterceptor.Models
{
    public class ServiceException
    {
        public string Type { get; set; }
        public Exception Exception { get; set; }
    }
}
