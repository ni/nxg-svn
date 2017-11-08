using System;
using System.ComponentModel.Composition;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;

namespace ViewpointSystems.Svn.Plugin.Add
{    
    public class AddCommand 
    {
        [Import]
        public ICompositionHost Host { get; set; }

        public static readonly ICommandEx AddShellRelayCommand = new ShellRelayCommand(Add)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Add.AddShellRelayCommand",
            LabelTitle = "Add",
        };
        
        /// <summary>
        /// Command handler to add file to SVN
        /// </summary>
        public static void Add(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var filePath = ((Envoy)parameter.Parameter).GetFilePath();
            var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
            var success = svnManager.Add(filePath);
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
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
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Add {filePath}"));
            }
        }        
    }
}
