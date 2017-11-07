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

        [Import]
        public ICompositionHost Host { get; set; }

        /// <summary>
        /// Command handler to view history
        /// </summary>
        public static void ViewHistory(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            //how to launch a tools window, via guid
            var historyToolWindow = site.ShowToolWindow(new Guid("b7e7ce66-d3fa-4c19-a7c9-8834e91a31f3"), true);            
            ((HistoryViewModel) (historyToolWindow.DataContext)).FilePath = ((Envoy)parameter.Parameter).GetFilePath();
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
                        if (status.IsVersioned)
                            context.Add(new ShellCommandInstance(HistoryShellRelayCommand) { CommandParameter = projectItem.Envoy });
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
