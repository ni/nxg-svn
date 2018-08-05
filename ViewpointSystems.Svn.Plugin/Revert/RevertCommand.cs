using System;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Svn.Plugin.SubMenu;

namespace Svn.Plugin.Revert
{    
    public class RevertCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx ShellSelectionRelayCommand = new ShellRelayCommand(Revert, CanRevert)
        {
            UniqueId = "Svn.Plugin.Revert.ShellSelectionRelayCommand",
            LabelTitle = "Revert",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };

        public static bool CanRevert(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            return true;
        }

        /// <summary>
        /// Revert changes
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void Revert(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            try
            {
                var filePath = ((Envoy)parameter.Parameter).GetFilePath();
                var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
                var success = svnManager.Revert(filePath);
                
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
            }
            catch (Exception e)
            {                
                Console.WriteLine(e);
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Revert {e.Message}"));
                const string caption = "Error SVN";
                var result = MessageBox.Show(e.Message, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            

        }       
    }
}
