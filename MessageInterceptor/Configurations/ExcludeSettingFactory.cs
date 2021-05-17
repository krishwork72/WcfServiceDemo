using System.Configuration;

namespace MessageInterceptor.Configurations
{
    public sealed class ExcludeSettingFactory
    {
        ExcludeSettingFactory() { }
        private static GenericConfigurationElementCollection<ExcludeSetting> _list = null;
        public static GenericConfigurationElementCollection<ExcludeSetting> ExcludeSettings
        {
            get
            {
                if (_list == null)
                {
                    _list = GetExcludeSettings();
                    LogWriter.Log($"The exclude method setting count:{_list.Count}");
                }
                return _list;
            }
        }
        private static GenericConfigurationElementCollection<ExcludeSetting> GetExcludeSettings()
        {
            var excludeCustomServiceBehavior = GetExcludeCustomServiceBehavior();
            if (excludeCustomServiceBehavior == null)
            {
                return new GenericConfigurationElementCollection<ExcludeSetting>();
            }
          return excludeCustomServiceBehavior.ExcludeSettings;
        }
        private static ExcludeCustomServiceBehavior GetExcludeCustomServiceBehavior()
        {
           return ConfigurationManager.GetSection("excludeCustomServiceBehavior") 
                    as ExcludeCustomServiceBehavior;
        }
    }
}
