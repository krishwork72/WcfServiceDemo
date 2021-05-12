using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WcfMessageInterceptor;

namespace WcfMessageInterceptorDemo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true, Inherited =false)]
    public class CustomServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        private Type serviceType;
        public CustomServiceBehaviorAttribute():this(null)
        {

        }
        public CustomServiceBehaviorAttribute(Type serviceType) => this.serviceType = serviceType;
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            LogWriter.Log("This is ApplyDispatchBehavior.");
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                if (channelDispatcher != null)
                {
                    foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        MessageInspector inspector = new MessageInspector(serviceType);
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}