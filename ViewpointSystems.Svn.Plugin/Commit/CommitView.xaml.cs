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

namespace Svn.Plugin.Commit
{
    /// <summary>
    /// Interaction logic for CommitView.xaml
    /// </summary>
    public partial class CommitView : BaseWindow
    {
        public CommitView(CommitViewModel commitViewModel)
        {
            DataContext = commitViewModel;
            InitializeComponent();            
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            ((CommitViewModel) DataContext).OkButtonClicked = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
