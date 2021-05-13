using System.Configuration;

namespace WcfMessageInterceptor.Configurations
{
    public class ExcludeSetting : ConfigurationElement
    {
        [ConfigurationProperty("methodName", DefaultValue = "", IsRequired = true)]
        public string MethodName
        {
            get
            {
                return (string)this["methodName"];
            }
            set
            {
                value = (string)this["methodName"];
            }
        }
        [ConfigurationProperty("reason", DefaultValue = "", IsRequired = false)]
        public string Reason
        {
            get
            {
                return (string)this["reason"];
            }
            set
            {
                value = (string)this["reason"];
            }
        }
    }
}
