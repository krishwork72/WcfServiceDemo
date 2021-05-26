using MessageInterceptor;
using MessageInterceptor.Models;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace WcfServiceDemo
{
    public class CheckInterceptor : ICheckInterceptor
    {
        public bool DoIntercept(List<HeaderModel> headers, MessageBuffer messageBuffer)
        {
            var message = messageBuffer.CreateMessage();
            if (message.IsEmpty)
            {
                //do logic here
            }
            return true;
        }
    }
}