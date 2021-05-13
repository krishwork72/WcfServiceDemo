using System.Configuration;

namespace WcfMessageInterceptor.Configurations
{
    public class ExcludeCustomServiceBehavior : ConfigurationSection
    {
        [ConfigurationProperty("maxRetry", IsRequired = false, DefaultValue = 5)]
        public int MaxRetry
        {
            get
            {
                return (int)this["maxRetry"];
            }
            set
            {
                this["maxRetry"] = value;
            }
        }

        [ConfigurationProperty("excludeSettings", IsRequired = true)]
        [ConfigurationCollection(typeof(ExcludeSetting), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public GenericConfigurationElementCollection<ExcludeSetting> ExcludeSettings
        {
            get { return (GenericConfigurationElementCollection<ExcludeSetting>)this["excludeSettings"]; }
        }
    }
}
