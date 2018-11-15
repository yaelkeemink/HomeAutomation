using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorningAutomation.Helpers
{
    public static class ConfigurationExtension
    {
        public static string GetValue(this IEnumerable<IConfigurationSection> config, string key)
        {
            return config.Single(a => a.Key == key).Value;
        }
    }
}
