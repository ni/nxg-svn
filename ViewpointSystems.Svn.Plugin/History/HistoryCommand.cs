using System;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using ViewpointSystems.Svn.Plugin.SubMenu;

namespace ViewpointSystems.Svn.Plugin.History
{
    [ExportPushCommandContent]
    public class HistoryCommand : PushCommandContent
    {
        public static readonly ICommandEx HistoryShellRelayCommand = new ShellRelayCommand(ViewHistory)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.History.HistoryShellRelayCommand",
            LabelTitle = "View History",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };

        /// <summary>
        /// Command handler to view history
        /// </summary>
        public static void ViewHistory(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {            
            var historyToolWindow = site.ShowToolWindow(new Guid("b7e7ce66-d3fa-4c19-a7c9-8834e91a31f3"), true);            
            ((HistoryViewModel) (historyToolWindow.DataContext)).FilePath = ((Envoy)parameter.Parameter).GetFilePath();
        }        
    }
}
