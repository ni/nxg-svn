using NationalInstruments.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NationalInstruments.Core;

namespace Svn.Plugin.UserPreferences
{
    /// <summary>
    /// Interaction logic for SvnPreferencesPage.xaml
    /// </summary>
    public partial class SvnPreferencesPage : UserControl, IUserPreferencesPage
    {
        public SvnPreferencesPage()
        {
            InitializeComponent();
            _promptToLockCtrl.IsChecked = SvnPreferences.PromptToLock;
        }

        public PlatformVisual Visual => this;

        public void CancelChanges()
        {
            _promptToLockCtrl.IsChecked = SvnPreferences.PromptToLock;
        }

        public UserPreferencesChangeResults CommitChanges()
        {
            SvnPreferences.PromptToLock = (bool)_promptToLockCtrl.IsChecked;
            return UserPreferencesChangeResults.None;
        }

        public void NotifyResetDefaults()
        {
            SvnPreferences.PromptToLock = true;
            _promptToLockCtrl.IsChecked = SvnPreferences.PromptToLock;
        }
    }
}
