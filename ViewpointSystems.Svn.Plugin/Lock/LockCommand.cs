using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using System.ComponentModel.Composition;

namespace ViewpointSystems.Svn.Plugin.Lock
{    
    public class LockCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx TakeLockShellRelayCommand = new ShellRelayCommand(TakeLock)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Lock.TakeLockShellRelayCommand",
            LabelTitle = "Lock",
        };
        
        /// <summary>
        /// Take lock
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void TakeLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
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
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
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
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Lock {filePath}"));
            }
            // }
        }        
    }
}
