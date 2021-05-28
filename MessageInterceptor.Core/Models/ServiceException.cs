using System;

namespace MessageInterceptor.Core.Models
{
    public class ServiceException
    {
        public string Type { get; set; }
        public Exception Exception { get; set; }
    }
}
