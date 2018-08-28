using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using NationalInstruments.Composition;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer;
using NationalInstruments.ProjectExplorer.Design;

namespace Svn.Plugin.SolutionExplorer
{
    [Export(typeof(IProjectExplorerInitialization))]
    internal class RefreshService : IProjectExplorerInitialization
    {
        [Import]
        public ICompositionHost Host { get; set; }

        /// <inheritdoc />
        public IList<string> HiddenFilesAndFolders => new string[0];

        /// <inheritdoc />
        public void OnProjectExplorerCreated(ProjectExplorerViewModel projectExplorerViewModel)
        {
        }

        /// <inheritdoc />
        public void OnValidateStateChanged(ProjectExplorerViewModel projectExplorerViewModel)
        {
            var debugHost = Host.GetSharedExportedValue<IDebugHost>();
            try
            {
                var svnManager = Host.GetSharedExportedValue<SvnManagerPlugin>();
                svnManager.UpdateCache();
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Status Cache Updated"));
            }
            catch (Exception e)
            {
                debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to Update Cache {e.Message}"));
                const string caption = "Error SVN";
                var result = System.Windows.Forms.MessageBox.Show(e.Message, caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }            
        }
    }
}
