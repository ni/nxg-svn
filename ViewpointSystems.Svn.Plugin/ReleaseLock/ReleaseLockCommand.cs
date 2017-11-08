using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using System.ComponentModel.Composition;

namespace ViewpointSystems.Svn.Plugin.ReleaseLock
{
    public class ReleaseLockCommand
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx ReleaseLockShellRelayCommand = new ShellRelayCommand(ReleaseLock)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.ReleaseLock.ReleaseLockShellRelayCommand",
            LabelTitle = "Release Lock",
        };

        /// <summary>
        /// Release lock
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="host"></param>
        /// <param name="site"></param>
        public static void ReleaseLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var filePath = ((Envoy)parameter.Parameter).GetFilePath();
            var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
            var success = svnManager.ReleaseLock(filePath);
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
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
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Release Lock {filePath}"));
            }

        }
    }
}
