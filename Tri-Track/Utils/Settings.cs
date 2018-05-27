// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace TriTrack.Utils
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
{
    private static ISettings AppSettings
    {
        get
        {
            return CrossSettings.Current;
        }
    }

    #region Setting Constants

    private const string LastUsernameSetting = "last_username_key";
        private const string LastPasswordSetting = "last_password_key";
    private static readonly string SettingsDefault = string.Empty;

    #endregion


    public static string LastUsername
    {
        get
        {
            return AppSettings.GetValueOrDefault(LastUsernameSetting, SettingsDefault);
        }
        set
        {
                AppSettings.AddOrUpdateValue(LastUsernameSetting, value);
        }
    }
        public static string LastPassword
        {
            get{
                return AppSettings.GetValueOrDefault(LastPasswordSetting, SettingsDefault);
            }
            set{
                AppSettings.AddOrUpdateValue(LastPasswordSetting, value);
            }
        }

}
}
