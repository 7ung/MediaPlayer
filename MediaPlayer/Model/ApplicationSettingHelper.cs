using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MediaPlayer
{
    sealed class ApplicationSettingHelper
    {
        // lấy dữ liệu và xoá khỏi danh sách
        public static object ReadResetSettingsValue(string key)
        {
            // LocalSettings.values có IDictionary
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key) == false)
                return null;
            var value = ApplicationData.Current.LocalSettings.Values[key];
            ApplicationData.Current.LocalSettings.Values.Remove(key);
            return value;
        }

        public static void SaveSettingsValue(string key, object value)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
        }
    }
}
