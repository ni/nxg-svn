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
using ViewpointSystems.Svn.Lock;

namespace ViewpointSystems.Svn.Commands
{
    [ExportPushCommandContent]
    public class Commands : PushCommandContent
    {
        public static readonly ICommandEx ViewHistoryCommand = new ShellRelayCommand(ViewHistory)
        {
            UniqueId = "ViewpointSystems.Svn.ViewHistoryCommand",
            LabelTitle = "View History",
        };

        /// <summary>
        /// Command handler which writes the active definition to a temporary merge script and opens that script in notepad.
        /// </summary>
        public static void ViewHistory(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            
            var lockWindow = new LockView();
            lockWindow.Owner = (Window)site.RootVisual;
            lockWindow.ShowDialog();

            var svnManager = host.GetSharedExportedValue<SvnManager>();
            //MessageBoxResult result = MessageBox.Show("Do you want to close this window?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{

            //}
            //var activeDefinition = site?.EditControl?.Document?.Envoy?.ReferenceDefinition;
            //if (activeDefinition != null)
            //{
            //    var fileName = Path.GetTempFileName();
            //    File.WriteAllText(fileName, MergeScriptBuilder.Create(activeDefinition.ToEnumerable(), host).ToString());
            //    Process.Start("Notepad.exe", fileName);
            //}
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
                            context.Add(ViewHistoryCommand);
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
