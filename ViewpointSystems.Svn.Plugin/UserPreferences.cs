using NationalInstruments.Core;
using NationalInstruments.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewpointSystems.Svn.Plugin
{
    /// <summary>
    /// User preferences page factory export
    /// </summary>
    [ExportUserPreferences()]
    public class SvnPropertiesPageProvider : UserPreferencesProvider
    {
        /// <inheritdoc/>
        public override string PreferencesPageName
        {
            get
            {
                return "SVN Preferences";
            }
        }

        /// <inheritdoc/>
        public override IUserPreferencesPage CreatePage()
        {
            return Host.CreateInstance<SvnPreferencesPage>();
        }
    }

    public class SvnPreferences
    {
        public static bool PromptToLock
        {
            get { return PreferencesHelper.GetPreference(typeof(SvnPreferences), nameof(PromptToLock), true); }
            set { PreferencesHelper.SetPreference(typeof(SvnPreferences), nameof(PromptToLock), value); }
        }
    }
}
