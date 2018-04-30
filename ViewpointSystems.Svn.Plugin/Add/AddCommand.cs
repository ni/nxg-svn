using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using ViewpointSystems.Svn.Plugin.SubMenu;

namespace ViewpointSystems.Svn.Plugin.Add
{    
    public class AddCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx ShellSelectionRelayCommand = new ShellRelayCommand(Add, CanAdd)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Add.ShellSelectionRelayCommand",
            LabelTitle = "Add",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };

        public static bool CanAdd(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            return true;
        }
        
        /// <summary>
        /// Command handler to add file to SVN
        /// </summary>
        public static void Add(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            try
            {
                var filePath = ((Envoy)parameter.Parameter).GetFilePath();
                var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
                var success = svnManager.Add(filePath);                
                if (success)
                {
                    var envoy = ((Envoy)parameter.Parameter);
                    var projectItem = envoy.GetProjectItemViewModel(site);
                    if (null != projectItem)
                    {
                        projectItem.RefreshIcon();
                    }
                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Add {filePath}"));
                }                
            }
            catch (Exception e)
            {                
                Console.WriteLine(e);
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Add {e.Message}"));
                const string caption = "Error SVN";
                var result = MessageBox.Show(e.Message, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            
        }        
    }
}
