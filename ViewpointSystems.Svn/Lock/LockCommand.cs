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
using ViewpointSystems.Svn.History;
using ViewpointSystems.Svn.Lock;

namespace ViewpointSystems.Svn.Lock
{
    [ExportPushCommandContent]
    public class LockCommand : PushCommandContent
    {       
        public static readonly ICommandEx TakeLockShellRelayCommand = new ShellRelayCommand(TakeLock)
        {
            UniqueId = "ViewpointSystems.Svn.Lock.TakeLockShellRelayCommand",
            LabelTitle = "Lock",
        };

        
        public static void TakeLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            //TODO: provide which VI was locked to UI
            var svnManager = host.GetSharedExportedValue<SvnManager>();
            var lockWindow = new LockView();
            lockWindow.Owner = (Window)site.RootVisual;
            lockWindow.ShowDialog();
                      
        }

        public override void CreateContextMenuContent(ICommandPresentationContext context, PlatformVisual sourceVisual)
        {
            var projectItem = sourceVisual.DataContext as ProjectItemViewModel;
            if (projectItem != null && projectItem.Envoy != null)
            {
                try
                {
                    var loadedEnvoy = projectItem.Envoy.Project.GetLinkedEnvoys(projectItem.Envoy).Where(e => e.ReferenceDefinition != null).FirstOrDefault();
                    if (loadedEnvoy != null)
                    {
                        var viDocument = loadedEnvoy.ReferenceDefinition as VirtualInstrument;
                        if (loadedEnvoy.ReferenceDefinition != null)
                        {                            
                            context.Add(TakeLockShellRelayCommand);
                        }
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
