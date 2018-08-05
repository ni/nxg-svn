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
using System.Windows.Shapes;

using System.Windows.Forms;
using System.Data;

using System.ComponentModel;
using MvvmCross.Platform;
using Svn.Core.ViewModels;
using Svn.Wpf.Utilities;

namespace Svn.Wpf.Views
{
    /// <summary>
    /// Interaction logic for StatusView.xaml
    /// </summary>
    [Region(Region.ModalWindow)]
    public partial class StatusView
    {
        private StatusViewModel viewModel;
        private Window window;

        public new StatusViewModel ViewModel
        {
            get { return viewModel ?? (viewModel = base.ViewModel as StatusViewModel); }
        }

        public StatusView()
        {
            InitializeComponent();
            WindowTitle = "Status View";
            Loaded += StatusView_Loaded;
           
        }

        private void StatusView_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);
            window.Closing += Window_Closing;
            Loaded -= StatusView_Loaded;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //ViewModel.Monitor = false;
        }
    }
}
