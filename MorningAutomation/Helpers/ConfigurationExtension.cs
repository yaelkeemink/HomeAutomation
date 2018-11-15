using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AutomationController.Helpers
{
    public static class ConfigurationExtension
    {
        public static string GetValue(this IEnumerable<IConfigurationSection> config, string key)
        {
            return config.Single(a => a.Key == key).Value;
        }
    }
}
