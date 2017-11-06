using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NationalInstruments.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;

namespace ViewpointSystems.Svn.Plugin.History
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {               
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="host">The composition host this window is associated with</param>
        public HistoryView(ToolWindowEditSite site, HistoryViewModel historyViewModel)
        {
            // Set up a few things
            //_host = host;
           // SelectedItems = new ObservableCollection<SelectionDisplayInfo>();
            DataContext = historyViewModel;

            // This creates our View.  Standard WPF thing.
            InitializeComponent();

           // historyDataGrid.style
            // Here we are getting the "User interface" framework object and registering
            // for selection and document change notifications
            //site.RootEditSite.ActiveDocumentChanged += HandleActiveDocumentChanged;
           // site.RootEditSite.SelectedChanged += HandleSelectedChanged;

            // Make sure we are reflecting the currently active document and
            // selection
            //UpdateDocumentName(site.RootEditSite.ActiveDocument);
            //UpdateSelection(site.RootEditSite.ActiveSelection);
        }

        

        /// <summary>
        /// Called when the active selection changes
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event data</param>
        //private void HandleSelectedChanged(object sender, SelectedChangedEventArgs args)
        //{
        //    UpdateSelection(args.SelectedItems);
        //}

     

        /// <summary>
        /// Updates the data we are displaying about the selection when it changes
        /// </summary>
        /// <param name="selection">The new selection</param>
        //private void UpdateSelection(IEnumerable<IViewModel> selection)
        //{
        //    // Clear the current selection and handle the null case
        //    SelectedItems.Clear();
        //    if (selection == null)
        //    {
        //        return;
        //    }
        //    foreach (var item in selection)
        //    {
        //        // Gather some information about each selected item
        //        var model = item.Model as Element;
        //        var viewModel = item as NodeViewModel;
        //        if (model != null)
        //        {
        //            var info = new SelectionDisplayInfo()
        //            {
        //                Name = model.Documentation.Name,
        //                Type = model.SpecificKind
        //            };
        //            if (viewModel != null)
        //            {
        //                var image = RenderData.NineGridToImage(viewModel.IconData, new Size(16, 16));
        //                info.Image = image;
        //            }
        //            SelectedItems.Add(info);
        //        }
        //    }
        //}

       
    }

   
}

