using System;
using System.ServiceModel.Configuration;
using WcfMessageInterceptorDemo;

namespace WcfServiceDemo
{
    public class CustomInspectorBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new CustomServiceBehaviorAttribute();
        }
        public override Type BehaviorType
        {
            get { return typeof(CustomServiceBehaviorAttribute); }
        }
    }
}
