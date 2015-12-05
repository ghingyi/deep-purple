using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace deeP.SPAWeb.Services
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets the string value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <returns>Value of application setting.</returns>
        string GetSettingString(string key);

        /// <summary>
        /// Gets the Boolean value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <returns>Value of application setting.</returns>
        bool? GetSettingBoolean(string key);

        /// <summary>
        /// Gets the integer value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <returns>Value of application setting.</returns>
        int? GetSettingInteger(string key);

        /// <summary>
        /// Gets the long value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <returns>Value of application setting.</returns>
        long? GetSettingLong(string key);

        /// <summary>
        /// Tries to get the string value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <param name="value">Value of application setting or null if failed to retrieve.</param>
        /// <returns>True if successfully retrieved the value for the key; false otherwise.</returns>
        bool TryGetSettingString(string key, out string value);

        /// <summary>
        /// Tries to get the Boolean value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <param name="value">Value of application setting or null if failed to retrieve.</param>
        /// <returns>True if successfully retrieved the value for the key; false otherwise.</returns>
        bool TryGetSettingBoolean(string key, out bool? value);

        /// <summary>
        /// Tries to get the integer value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <param name="value">Value of application setting or null if failed to retrieve.</param>
        /// <returns>True if successfully retrieved the value for the key; false otherwise.</returns>
        bool TryGetSettingInteger(string key, out int? value);

        /// <summary>
        /// Tries to get the long value of an application setting.
        /// </summary>
        /// <param name="key">Key of application setting.</param>
        /// <param name="value">Value of application setting or null if failed to retrieve.</param>
        /// <returns>True if successfully retrieved the value for the key; false otherwise.</returns>
        bool TryGetSettingLong(string key, out long? value);
    }
}