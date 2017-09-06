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
using NationalInstruments.SourceModel.Envoys;

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
                //TODO: Update UI with latest status of items in SVN
            }
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

        public bool ReleaseLock(string filename)
        {
            return _svnManager.ReleaseLock(filename);
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
