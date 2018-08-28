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

namespace Svn.Plugin.ReleaseLock
{
    public class ReleaseLockCommand
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx ShellSelectionRelayCommand = new ShellRelayCommand(ReleaseLock, CanReleaseLock)
        {
            UniqueId = "Svn.Plugin.ReleaseLock.ShellSelectionRelayCommand",
            LabelTitle = "Release Lock",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };

        public static bool CanReleaseLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            return true;
        }

        /// <summary>
        /// Release lock
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void ReleaseLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            try
            {
                var filePath = ((Envoy)parameter.Parameter).GetFilePath();
                var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
                var success = svnManager.ReleaseLock(filePath);
                
                if (success)
                {
                    var envoy = ((Envoy)parameter.Parameter);
                    var projectItem = envoy.GetProjectItemViewModel(site);
                    if (null != projectItem)
                    {
                        projectItem.RefreshIcon();
                    }
                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Release Lock {filePath}"));
                }                
            }
            catch (Exception e)
            {                
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Release Lock {e.Message}"));
                Console.WriteLine(e);
                const string caption = "Error SVN";
                var result = MessageBox.Show(e.Message, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }            
        }
    }
}
