using System;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using ViewpointSystems.Svn.Plugin.SubMenu;

namespace ViewpointSystems.Svn.Plugin.Lock
{    
    public class LockCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx ShellSelectionRelayCommand = new ShellRelayCommand(TakeLock, CanLock)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Lock.ShellSelectionRelayCommand",
            LabelTitle = "Lock",

            // this will inform the system that this command should be parented under the given command in a popup menu
            PopupMenuParent = SvnCommands.SvnSubMenuCommand
        };

        public static bool CanLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            return true;
        }

        /// <summary>
        /// Take lock
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void TakeLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            try
            {
                var filePath = ((Envoy)parameter.Parameter).GetFilePath();
                //TODO: flush out View / ViewModel for lock - bsh todo
                //if (SvnPreferences.PromptToLock)
                //{
                //    var lockWindow = new LockView();
                //    lockWindow.Owner = (Window)site.RootVisual;
                //    lockWindow.ShowDialog();
                //    
                //}
                //else
                //{
                var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
                var success = svnManager.Lock(filePath);
                
                if (success)
                {
                    var envoy = ((Envoy)parameter.Parameter);
                    var projectItem = envoy.GetProjectItemViewModel(site);
                    if (null != projectItem)
                    {
                        projectItem.RefreshIcon();
                    }
                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Lock {filePath}"));
                }                
                // }
            }
            catch (Exception e)
            {                
                Console.WriteLine(e);
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Lock {e.Message}"));
                const string caption = "Error SVN";
                var result = MessageBox.Show(e.Message, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }            
        }        
    }
}
