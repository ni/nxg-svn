using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using System.ComponentModel.Composition;
using ViewpointSystems.Svn.Plugin.SubMenu;

namespace ViewpointSystems.Svn.Plugin.Revert
{    
    public class RevertCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx RevertShellRelayCommand = new ShellRelayCommand(Revert)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Revert.RevertShellRelayCommand",
            LabelTitle = "Revert",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };
        
        /// <summary>
        /// Revert changes
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void Revert(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var filePath = ((Envoy)parameter.Parameter).GetFilePath();
            var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
            var success = svnManager.Revert(filePath);
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            if (success)
            {
                var envoy = ((Envoy)parameter.Parameter);
                var projectItem = envoy.GetProjectItemViewModel(site);
                if (null != projectItem)
                {
                    projectItem.RefreshIcon();
                }

                //TODO: needs to be addressed before go live
                //This will revert a file once, but you have to close and reopen in order to revert the same file a second time.
                //If open, you also have to manually close the file and reopen to see the reversion.
                var referencedFile = envoy.GetReferencedFileService();
                referencedFile.RefreshReferencedFileAsync();
             
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Revert {filePath}"));
            }
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Revert {filePath}"));
            }

        }       
    }
}
