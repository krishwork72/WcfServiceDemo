using MessageInterceptor.Filters;
using MessageInterceptor.Models;
using System.Collections.Generic;

namespace WebAPIDemo.Services
{
    public class CheckInterceptor : ICheckApiInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, Dictionary<string, object> payloads)
        {
            //in case of get action, payoad will empty 
            if (payloads.Count>0)
            {
                //do logic here
            }
            return true;
        }
    }
}