using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using NationalInstruments;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.DataTypes;
using NationalInstruments.Design;
using NationalInstruments.MocCommon.Design;
using NationalInstruments.MocCommon.SourceModel;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using NationalInstruments.SourceModel.Envoys;
using NationalInstruments.VI.SourceModel;

namespace ViewpointSystems.Svn.Plugin.Add
{
    [ExportPushCommandContent]
    public class AddCommand : PushCommandContent
    {
        public static readonly ICommandEx AddShellRelayCommand = new ShellRelayCommand(Add)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Add.AddShellRelayCommand",
            LabelTitle = "Add",
        };

        [Import]
        public ICompositionHost Host { get; set; }

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

       
        public override void CreateContextMenuContent(ICommandPresentationContext context, PlatformVisual sourceVisual)
        {
            var projectItem = sourceVisual.DataContext as ProjectItemViewModel;
            if (projectItem?.Envoy != null)
            {
                try
                {
                    var envoy = projectItem.Envoy;
                    if (envoy != null)
                    {
                        var svnManager = Host.GetSharedExportedValue<SvnManagerPlugin>();
                        var status = svnManager.Status(projectItem.FullPath);
                        if (status.IsVersionable && !status.IsVersioned)
                            context.Add(new ShellCommandInstance(AddShellRelayCommand) { CommandParameter = projectItem.Envoy });
                    }
                }
                catch (Exception)
                {
                }
            }
            base.CreateContextMenuContent(context, sourceVisual);
        }
    }
}
