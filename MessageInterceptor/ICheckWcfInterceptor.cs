using MessageInterceptor.Models;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace MessageInterceptor
{
    public interface ICheckWcfInterceptor
    {
        bool DoIntercept(List<HeaderModel> headers, MessageBuffer messageBuffer);
        
    }
}
