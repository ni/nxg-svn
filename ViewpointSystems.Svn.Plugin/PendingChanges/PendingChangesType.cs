using System.ComponentModel.Composition;
using NationalInstruments.Shell;
using NationalInstruments.Composition;

namespace Svn.Plugin.PendingChanges
{
    [Export(typeof(IToolWindowType))]
    [ExportMetadata("UniqueID", "cf9bf680-ee33-4343-9704-067a7555da31")]
    [Name("SVN Pending Changes")]
    [ExportMetadata("SmallImagePath", "")]
    [ExportMetadata("LargeImagePath", "")]
    [ExportMetadata("DefaultCreationTime", ToolWindowCreationTime.Startup)]
    [ExportMetadata("DefaultCreationMode", ToolWindowCreationMode.Unpinned)]
    [ExportMetadata("DefaultLocation", ToolWindowLocation.Bottom)]
    [ExportMetadata("AssociatedDocumentType", "")]
    [ExportMetadata("Weight", 0.5)]
    [ExportMetadata("ForceOpenPinned", false)]
    public class PendingChangesType : ToolWindowType
    {
        public override IToolWindowViewModel CreateToolWindow(ToolWindowEditSite editSite)
        {
            return new PendingChangesViewModel(editSite);
        }
    }
}
