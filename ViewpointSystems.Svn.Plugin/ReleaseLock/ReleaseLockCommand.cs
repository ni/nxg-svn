using System;
using System.Collections.Generic;
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
using NationalInstruments.VI.SourceModel;
using NationalInstruments.SourceModel.Envoys;
using ViewpointSystems.Svn.Plugin.UserPreferences;
using System.ComponentModel.Composition;

namespace ViewpointSystems.Svn.Plugin.ReleaseLock
{
    [ExportPushCommandContent]
    public class ReleaseLockCommand : PushCommandContent
    {
        public static readonly ICommandEx ReleaseLockShellRelayCommand = new ShellRelayCommand(ReleaseLock)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.ReleaseLock.ReleaseLockShellRelayCommand",
            LabelTitle = "Release Lock",
        };

        [Import]
        public ICompositionHost Host { get; set; }

        public static void ReleaseLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            var filePath = ((Envoy)parameter.Parameter).GetFilePath();
            var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
            var success = svnManager.ReleaseLock(filePath);
            var debugHost = host.GetSharedExportedValue<IDebugHost>();
            if (success)
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Release Lock {filePath}"));
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Release Lock {filePath}"));
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
                        if(status.IsLocked)                            
                            context.Add(new ShellCommandInstance(ReleaseLockShellRelayCommand) { CommandParameter = projectItem.Envoy });
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
