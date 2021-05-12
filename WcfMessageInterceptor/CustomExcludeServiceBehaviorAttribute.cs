using System;

namespace WcfMessageInterceptor
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple =true, Inherited = false)]
   public class CustomExcludeServiceBehaviorAttribute : Attribute
    {
        public string Method { get; set; }
        public CustomExcludeServiceBehaviorAttribute()
        {
            Method = string.Empty;
        }
        public CustomExcludeServiceBehaviorAttribute(string methodName)
        {
            Method = methodName;
        }
    }
}
