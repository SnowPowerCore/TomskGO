﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Core.Services.Utils.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Fields
        private readonly string _projectName = "Tomsk GO!";
        private readonly string _supportNumber = "-";
        private readonly string _supportEmail = "-";
        private readonly string _defaultApiUrl = "http://10.0.2.2:7071";
        private readonly string _defaultAppUpdateUrl = "";
        private readonly string _appCenterAndroidKey = "android=;";
        private readonly string _appCenteriOSKey = "ios=;";
        #endregion

        #region Properties
        public string ProjectName =>
            GetValueOrDefault("ProjectName", _projectName);

        public string SupportNumber =>
            GetValueOrDefault("SupportNumber", _supportNumber);

        public string SupportEmail =>
            GetValueOrDefault("SupportEmail", _supportEmail);

        public string DefaultApiUrl =>
            GetValueOrDefault("DefaultApiUrl", _defaultApiUrl);

        public string DefaultAppUpdateUrl =>
            GetValueOrDefault("DefaultAppUpdateUrl", _defaultAppUpdateUrl);

        public string AppCenterAndroidKey =>
            GetValueOrDefault("AppCenterAndroidKey", _appCenterAndroidKey);

        public string AppCenteriOSKey =>
            GetValueOrDefault("AppCenteriOSKey", _appCenteriOSKey);
        #endregion

        #region Methods
        public bool ContainsKey(string key) =>
            Application.Current.Properties.ContainsKey(key);

        public async Task AddOrUpdateValueAsync<T>(string key, T value, bool serialize = false)
        {
            if (serialize)
                Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
            else Application.Current.Properties[key] = value;

            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
            }
        }

        public async Task AddOrUpdateValuesAsync<T>(Dictionary<string, T> keyValues, bool serialize = false)
        {
            if (keyValues == null) return;

            foreach (var item in keyValues)
            {
                var key = item.Key;
                var value = item.Value;

                if (serialize)
                    Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
                else Application.Current.Properties[key] = value;
            }

            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this dict" + keyValues,
                    " Message: " + ex.Message);
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default, bool deserialize = false)
        {
            T value = defaultValue;
            if (Application.Current.Properties.ContainsKey(key))
            {
                var data = Application.Current.Properties[key];
                if (deserialize) value = JsonConvert.DeserializeObject<T>((string)data);
                else value = (T)data;
            }
            return value;
        }

        public T GetApplicationResourceOrDefault<T>(string key, T defaultValue = default)
        {
            T value = defaultValue;
            if (Application.Current.Resources.ContainsKey(key))
                value = (T)Application.Current.Resources[key];
            return null != value ? value : defaultValue;
        }

        public List<T> GetValuesOrDefaults<T>(string[] keys, T defaultValue = default, bool deserialize = false)
        {
            var list = new List<T>();
            for (var i = 0; i < keys.Length; i++)
                list.Add(GetValueOrDefault(keys[i], defaultValue, deserialize));
            return list;
        }

        public async Task DetermineAndSetDefaultsAsync()
        {
            if (!GetValueOrDefault<bool>("DefaultsSet"))
                await SetDefaultsAsync();
        }

        private async Task SetDefaultsAsync()
        {
            Application.Current.Properties.Clear();
            await AddOrUpdateValuesAsync(new Dictionary<string, string>
            {
                { nameof(ProjectName), _projectName },
                { nameof(SupportNumber), _supportNumber },
                { nameof(SupportEmail), _supportEmail },
                { nameof(DefaultApiUrl), _defaultApiUrl },
                { nameof(DefaultAppUpdateUrl), _defaultAppUpdateUrl },
                { nameof(AppCenterAndroidKey), _appCenterAndroidKey },
                { nameof(AppCenteriOSKey), _appCenteriOSKey },
            }).ContinueWith(async t => await AddOrUpdateValueAsync("DefaultsSet", true),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public async Task RemoveValueAsync(string key)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                Application.Current.Properties.Remove(key);

                try
                {
                    await Application.Current.SavePropertiesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                }
            }
        }

        public async Task RemoveValuesAsync(string[] keys)
        {
            if (keys == null) return;

            foreach (var item in keys)
            {
                if (Application.Current.Properties.ContainsKey(item))
                    Application.Current.Properties.Remove(item);
            }

            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this list " + keys,
                    " Message: " + ex.Message);
            }
        }
        #endregion
    }
}