using System.ComponentModel.Composition;
using NationalInstruments.Shell;
using NationalInstruments.Composition;

namespace ViewpointSystems.Svn.Plugin.History
{

    /// <summary>
    /// Factory class for our Selection Tracking Tool
    /// Performs registration with the framework with all of this metadata
    /// And factories new instances on demand
    /// </summary>
    [Export(typeof(IToolWindowType))]
    [ExportMetadata("UniqueID", "b7e7ce66-d3fa-4c19-a7c9-8834e91a31f3")]
    [Name("SVN History")]
    [ExportMetadata("SmallImagePath", "")]
    [ExportMetadata("LargeImagePath", "")]
    [ExportMetadata("DefaultCreationTime", ToolWindowCreationTime.Startup)]
    [ExportMetadata("DefaultCreationMode", ToolWindowCreationMode.Unpinned)]
    [ExportMetadata("DefaultLocation", ToolWindowLocation.Bottom)]
    [ExportMetadata("AssociatedDocumentType", "")]
    [ExportMetadata("Weight", 0.5)]
    [ExportMetadata("ForceOpenPinned", false)]
    public class HistoryType : ToolWindowType
    {
        public override IToolWindowViewModel CreateToolWindow(ToolWindowEditSite editSite)
        {
            return new HistoryViewModel(editSite);
        }
    }
}
