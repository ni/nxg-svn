using NationalInstruments.Core;

namespace ViewpointSystems.Svn.Plugin.UserPreferences
{
    public class SvnPreferences
    {
        public static bool PromptToLock
        {
            get { return PreferencesHelper.GetPreference(typeof(SvnPreferences), nameof(PromptToLock), true); }
            set { PreferencesHelper.SetPreference(typeof(SvnPreferences), nameof(PromptToLock), value); }
        }
    }
}