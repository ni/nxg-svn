using NationalInstruments.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn.Plugin.UserPreferences
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
}
