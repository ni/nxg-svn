using System;
using System.ComponentModel.Composition;
using NationalInstruments.Composition;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using ViewpointSystems.Svn.Plugin.Add;
using ViewpointSystems.Svn.Plugin.Commit;
using ViewpointSystems.Svn.Plugin.History;
using ViewpointSystems.Svn.Plugin.Lock;
using ViewpointSystems.Svn.Plugin.ReleaseLock;
using ViewpointSystems.Svn.Plugin.Revert;
using ViewpointSystems.Svn.Plugin.SubMenu;

namespace ViewpointSystems.Svn.Plugin.ContextMenu
{
    [ExportPushCommandContent]
    public class CreateContextMenus : PushCommandContent
    {       
        [Import]
        public ICompositionHost Host { get; set; }
        
       
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
                        var status = svnManager.Status(projectItem.FullPathForced);

                        if (status.Exists && status.IsVersionable)
                            context.Add(new ShellCommandInstance(SvnCommands.SvnSubMenuCommand) { CommandParameter = projectItem.Envoy });

                        if (status.IsVersionable && !status.IsVersioned)
                            context.Add(new ShellCommandInstance(AddCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });
                        if (status.IsVersioned && status.IsModified)
                            context.Add(new ShellCommandInstance(CommitCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });
                        if (status.IsVersioned && !status.IsAdded)
                            context.Add(new ShellCommandInstance(HistoryCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });
                        if (status.IsVersioned && !status.IsLocked && !status.IsAdded)
                            context.Add(new ShellCommandInstance(LockCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });
                        if (status.IsVersioned && status.IsLocked)
                            context.Add(new ShellCommandInstance(ReleaseLockCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });
                        if (status.IsVersioned && status.IsModified && !status.IsAdded)
                            context.Add(new ShellCommandInstance(RevertCommand.ShellSelectionRelayCommand) { CommandParameter = projectItem.Envoy });


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
