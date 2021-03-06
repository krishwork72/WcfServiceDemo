using System;
using System.ServiceModel.Configuration;

namespace MessageInterceptor
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
