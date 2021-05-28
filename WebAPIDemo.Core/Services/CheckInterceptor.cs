using MessageInterceptor.Core.Models;
using System.Collections.Generic;

namespace WebAPIDemo.Core.Services
{
    public class CheckInterceptor : ICheckInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                //logic here
            }
            return true;
        }
    }
}
