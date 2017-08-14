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
            var filePath = ((Envoy)parameter.Parameter).GetFilePath();            
            if (SvnPreferences.PromptToLock)
            {
                var lockWindow = new LockView();
                lockWindow.Owner = (Window)site.RootVisual;
                lockWindow.ShowDialog();
                //TODO: flush out View / ViewModel for lock - bsh todo
            }
            else
            {
                var svnManager = host.GetSharedExportedValue<SvnManagerPlugin>();
                //TODO: Console/output of what happened?  i.e. file lock success, or failure?                
                svnManager.Lock(filePath);
            }
        }

        public override void CreateContextMenuContent(ICommandPresentationContext context, PlatformVisual sourceVisual)
        {
            //TODO: right now option on menu is only shown if the VI is loaded in the editor, it should be 'possible' to be shown in all instances

            var projectItem = sourceVisual.DataContext as ProjectItemViewModel;
            if (projectItem != null && projectItem.Envoy != null)
            {
                try
                {
                    var envoy = projectItem.Envoy;
                    if (envoy != null)
                    {
                        //TODO: decide if lock command should be shown or not                            
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
