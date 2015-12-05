using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace deeP.SPAWeb.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetSettingString(string key)
        {
            string value;
            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch
            {
                value = null;
            }

            return value;
        }

        public bool? GetSettingBoolean(string key)
        {
            bool? value;

            string str = GetSettingString(key);
            value = str != null ? (bool?)bool.Parse(str) : null;

            return value;
        }

        public int? GetSettingInteger(string key)
        {
            int? value;

            string str = GetSettingString(key);
            value = str != null ? (int?)int.Parse(str) : null;

            return value;
        }

        public long? GetSettingLong(string key)
        {
            long? value;

            string str = GetSettingString(key);
            value = str != null ? (long?)long.Parse(str) : null;

            return value;
        }

        public bool TryGetSettingString(string key, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(value))
            {
                value = GetSettingString(key);
                if (value == null)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryGetSettingBoolean(string key, out bool? value)
        {
            value = null;
            string str = null;
            if (TryGetSettingString(key, out str))
            {
                value = str != null ? (bool?)bool.Parse(str) : null;
                return true;
            }
            return false;
        }

        public bool TryGetSettingInteger(string key, out int? value)
        {
            value = null;
            string str = null;
            if (TryGetSettingString(key, out str))
            {
                value = str != null ? (int?)int.Parse(str) : null;
                return true;
            }
            return false;
        }

        public bool TryGetSettingLong(string key, out long? value)
        {
            value = null;
            string str = null;
            if (TryGetSettingString(key, out str))
            {
                value = str != null ? (long?)long.Parse(str) : null;
                return true;
            }
            return false;
        }

    }
}