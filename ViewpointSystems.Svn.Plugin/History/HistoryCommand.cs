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

namespace ViewpointSystems.Svn.Plugin.History
{
    [ExportPushCommandContent]
    public class HistoryCommand : PushCommandContent
    {
        public static readonly ICommandEx HistoryShellRelayCommand = new ShellRelayCommand(ViewHistory)
        {
            UniqueId = "ViewpointSystems.Svn.Plugin.History.HistoryShellRelayCommand",
            LabelTitle = "View History",
        };
       
        /// <summary>
        /// Command handler to view history
        /// </summary>
        public static void ViewHistory(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            //how to launch a tools window, via guid
            var historyToolWindow = site.ShowToolWindow(new Guid("b7e7ce66-d3fa-4c19-a7c9-8834e91a31f3"), true);
            //TODO: grab name of file that was right clicked to provide to HistoryViewModel
            ((HistoryViewModel) (historyToolWindow.DataContext)).DocumentName = "provide name of document here";            
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
                            //TODO: decide if history command should be shown based on if file is in SVN
                            context.Add(HistoryShellRelayCommand);       
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
