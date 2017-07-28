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
using ViewpointSystems.Svn.SvnThings;


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

        
        public static void TakeLock(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            //TODO: provide which VI was locked to Lock UI
            //TODO: proper way to generate modal dialog
            //TODO: proper way to create view model
            //TODO: Read settings and decide if user even wants to see lock dialog, skip or show
            //TODO: Settings
            var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
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
                            //TODO: decide if lock command should be shown or not                            
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
