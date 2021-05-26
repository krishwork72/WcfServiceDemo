using MessageInterceptor.Filters;
using MessageInterceptor.Models;
using System.Collections.Generic;

namespace WebAPIDemo.Services
{
    public class CheckInterceptor : ICheckApiInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, string payload)
        {
            //in case of get action, payoad will empty 
            if (string.IsNullOrEmpty(payload))
            {
                //do logic here
            }
            return true;
        }
    }
}