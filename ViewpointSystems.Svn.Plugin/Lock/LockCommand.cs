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

namespace ViewpointSystems.Svn.Plugin.Lock
{
    [ExportPushCommandContent]
    public class LockCommand : PushCommandContent
    {
        public static readonly ICommandEx TakeLockShellRelayCommand = new ShellRelayCommand(TakeLock)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.Lock.TakeLockShellRelayCommand",
            LabelTitle = "Lock",
        };

        [Import]
        public ICompositionHost Host { get; set; }

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

                //var envoy = ((Envoy)parameter.Parameter);
                //ProjectItemViewModel projectItem = envoy.GetProjectItemViewModel(site);
                //if (null != projectItem)
                //{
                //    projectItem.RefreshIcon();
                //}

                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Lock {filePath}"));
            }
            else
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Lock {filePath}"));
            }
            // }
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
                        if (status.IsVersioned && !status.IsLocked && !status.IsAdded)
                            context.Add(new ShellCommandInstance(TakeLockShellRelayCommand) { CommandParameter = projectItem.Envoy });
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
