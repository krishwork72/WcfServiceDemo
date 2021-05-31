using MessageInterceptor.Core.Models;
using System.Collections.Generic;

namespace WebAPIDemo.Core.Services
{
    public class CheckInterceptor : ICheckInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, IDictionary<string, object> payloads)
        {
            if (payloads.Count>0)
            {
                //logic here
            }
            return true;
        }
    }
}
