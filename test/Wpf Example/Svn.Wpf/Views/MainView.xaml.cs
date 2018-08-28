using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Svn.Core.ViewModels;
using Svn.Wpf.Utilities;

namespace Svn.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    [Region(Region.FullScreen)]
    public partial class MainView
    {
        private MainViewModel viewModel;

        public new MainViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = base.ViewModel as MainViewModel); }
        }

        public MainView()
        {
            InitializeComponent();
            Loaded += MainViewViewLoaded;
        }

        private void MainViewViewLoaded(object sender, RoutedEventArgs e)
        {                                 
            ViewModel.Loaded();
            Loaded -= MainViewViewLoaded;
        }
    }
}
