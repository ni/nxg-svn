using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Composition;
using NationalInstruments.Core;
using ViewpointSystems.Svn.SvnThings;
using NationalInstruments.Shell;
using System.IO;
using NationalInstruments.Design;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.SourceModel.Envoys;
using NationalInstruments.Restricted.Shell;

namespace ViewpointSystems.Svn.Plugin
{
    [Export(typeof(SvnManagerPlugin))]
    //[PartMetadata(ExportIdentifier.RootContainerKey, "")] // This has project afinity so we need to create one per Project
    public class SvnManagerPlugin : IPartImportsSatisfiedNotification
    {
        private SvnManager _svnManager;
        private IDocumentManager _documentManager;
        private Project _currentProject;

        public SvnManagerPlugin()
        {
        }

        [Import]
        public ICompositionHost Host { get; set; }

        public void OnImportsSatisfied()
        {
            _documentManager = Host.GetSharedExportedValue<IDocumentManager>();
            _documentManager.PropertyChanged += HandleDocumentManagerPropertyChanged;
            ConnectToProject();

        }

        private void HandleDocumentManagerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DocumentManagerExtensions.ActiveProjectPropertyChangedName)
            {
                ConnectToProject();
            }
        }
       
        private void ConnectToProject()
        {
            _currentProject = _documentManager.ActiveProject;
            _currentProject.PropertyChanged += HandleProjectPropertyChanged;
            ConnectToRepository(_currentProject.StoragePath); 
        }

        private void HandleProjectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Project.StoragePathPropertyChangedName)
            {
                ConnectToRepository(_currentProject.StoragePath);
            }
        }

        private void ConnectToRepository(string storagePath)
        {
            if (string.IsNullOrEmpty(storagePath))
            {
                // Unsaved project
                return;
            }            
            _svnManager = new SvnManager();
            if (_svnManager.IsWorkingCopy(Path.GetDirectoryName(storagePath))) 
            {
                _svnManager.LoadCurrentSvnItemsInLocalRepository(Path.GetDirectoryName(storagePath));                
            }
            _svnManager.SvnStatusUpdatedEvent += SvnManagerOnSvnStatusUpdatedEvent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="svnStatusUpdatedEventArgs"></param>
        private void SvnManagerOnSvnStatusUpdatedEvent(object sender, SvnStatusUpdatedEventArgs svnStatusUpdatedEventArgs)
        {
            Host.BeginInvoke(() =>
            {
                //GetStudioWindows() is in NationalInstruments.Restricted.Shell and may not always be publicly exposed.
                //This call may need to change at a later date, but should work for now.
                var studioWindows = StudioWindow.GetStudioWindows(Host); //Get all of the StudioWindows
                foreach (StudioWindow studioWindow in studioWindows) // use each StudioWindow's EditSite to refresh the appropriate icon
                {
                    var editSite = studioWindow.GetEditSite();
                    if (null != editSite) // check editSite != null just in case
                    {
                        var projectExplorerViewModel = editSite?.GetProjectExplorerViewModelFromEditSite();
                        var projectItem = projectExplorerViewModel?.FindProjectItemByFullPath(svnStatusUpdatedEventArgs.FullFilePath);
                        projectItem?.RefreshIcon();
                    }
                }
            }); 
        }


        //TODO: listen to add file, update cache

        /// <summary>
        /// Lock the file
        /// </summary>
        /// <param name="filename">file to lock</param>
        /// <param name="comment">optional lock comment</param>
        /// <returns></returns>
        public bool Lock(string filename, string comment = "")
        {
            return _svnManager.Lock(filename, comment);
        }

        /// <summary>
        /// Release lock
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>true if success, false otherwise</returns>
        public bool ReleaseLock(string filePath)
        {
            return _svnManager.ReleaseLock(filePath);
        }

        /// <summary>
        /// Revert File
        /// </summary>
        /// <param name="filePath">path to file</param>
        /// <returns></returns>
        public bool Revert(string filePath)
        {
            return _svnManager.Revert(filePath);
        }

        /// <summary>
        /// Status of a single file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public SvnItem Status(string filename)
        {
            return _svnManager.GetSingleItemStatus(filename);
        }
    }
}
