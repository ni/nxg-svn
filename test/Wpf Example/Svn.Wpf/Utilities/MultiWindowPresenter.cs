using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Exceptions;
using MvvmCross.Platform.Platform;
using MvvmCross.Wpf.Views;
using Svn.Wpf.Views;

namespace Svn.Wpf.Utilities
{
    public class MultiWindowPresenter : MvxWpfViewPresenter
    {
        private readonly ContentControl mainWindow;
        private ModalWindow modalWindow;

        public MultiWindowPresenter(ContentControl mainWindow)
        {
            this.mainWindow = mainWindow;

            //modalWindow
        }

        public override void Show(MvxViewModelRequest request)
        {
            try
            {
                var loader = Mvx.Resolve<IMvxSimpleWpfViewLoader>();
                var view = loader.CreateView(request);
                Present(view);
            }
            catch (Exception exception)
            {
                MvxTrace.Error("Error seen during navigation request to {0} - error {1}", request.ViewModelType.Name,
                               exception.ToLongString());
            }
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            if (hint is MvxClosePresentationHint)
            {
                //for the multi window presenter the only close hints we will receive will
                //be to close the modal window
                modalWindow.Close();
            }
            base.ChangePresentation(hint);
        }

        public override void Close(IMvxViewModel toClose)
        {
            //throw new NotImplementedException();
        }

        public override void Present(FrameworkElement frameworkElement)
        {
            //grab the attribute off of the view
            var attribute = frameworkElement
                                .GetType()
                                .GetCustomAttributes(typeof(RegionAttribute), true)
                                .FirstOrDefault() as RegionAttribute;

            var regionName = null == attribute ? Region.Unknown : attribute.Region;
            //based on region decide where we are going to show it
            switch (regionName)
            {
                case Region.FullScreen:
                    mainWindow.Content = frameworkElement;
                    break;
                case Region.ModalWindow:
                    //set the base tab
                    modalWindow = new ModalWindow();

                    modalWindow.Content = frameworkElement;
                    modalWindow.Left = ((Window)mainWindow).Left + ((Window)mainWindow).Width;
                    modalWindow.Top = ((Window)mainWindow).Top;
                    modalWindow.Title = ((BaseView)frameworkElement).WindowTitle;
                    modalWindow.Height = ((BaseView)frameworkElement).Height;
                    modalWindow.Width = ((BaseView)frameworkElement).Width;
                    //xxx confirm this is going to work for other operations in the system
                    modalWindow.ShowDialog();
                    break;
            }

        }
    }
}
