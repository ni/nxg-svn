using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using NationalInstruments;
using NationalInstruments.Composition;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using SharpSvn;
using ViewpointSystems.Svn.SvnThings;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ViewpointSystems.Svn.Plugin.PendingChanges
{
    public class PendingChangesViewModel : IToolWindowViewModel
    {
        private ToolWindowEditSite _editSite;

        //public ICommand CompareWithWorkingCopyCommand { get; set; }

        /// <summary>
        /// True, actively monitor for changes
        /// </summary>
        public bool Monitor = true;

        private SvnManagerPlugin _svnManager;

        public PendingChangesViewModel(ToolWindowEditSite site)
        {
            _editSite = site;
            //Monitor = true;
            // MonitorStatus();
            _svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
            InitialLoad();
            _svnManager.SvnStatusUpdatedEvent += SvnManagerOnSvnStatusUpdatedEvent;
            //CompareWithWorkingCopyCommand = new RelayCommand(DoComapreWithWorkingCopyCommand);            
        }

        /// <summary>
        /// Grab all items svn manager knows about to populate pending changes 
        /// </summary>
        private void InitialLoad()
        {
            var mappings = _svnManager.GetMappings();

            foreach (var item in mappings)
            {
                if (item.Value.IsFile)
                {
                    FileStatus.Add(new PendingChange()
                    {
                        Path = item.Value.FullPath,
                        Status = item.Value.Status.CombinedStatus.ToString()
                    });
                }
            }            
        }

        /// <summary>
        /// update based on 'things happening' via svn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SvnManagerOnSvnStatusUpdatedEvent(object sender, SvnStatusUpdatedEventArgs e)
        {
            var status = _svnManager.Status(e.FullFilePath);
            if (status.IsFile)
            {
                var existingItem = FileStatus.FirstOrDefault(x => x.Path == e.FullFilePath);
                if (null != existingItem)
                {
                    existingItem.Status = status.Status.CombinedStatus.ToString();
                }
                else
                {
                    FileStatus.Add(new PendingChange()
                    {
                        Path = status.FullPath,
                        Status = status.Status.CombinedStatus.ToString()
                    });
                }
            }
        }


        /// <summary>
        /// Compare selected version with working version
        /// </summary>
        /// <param name="obj"></param>
        //private void DoComapreWithWorkingCopyCommand(object obj)
        //{
        //    if (null != SelectedHistoryRow)
        //    {
        //        var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
        //        var tempFilePathOldVersion = Path.Combine(Path.GetTempPath(), Path.GetFileName(FilePath));                
        //        var success = svnManager.Write(FilePath, tempFilePathOldVersion, SelectedHistoryRow.Revision);
        //        var debugHost = _editSite.Host.GetSharedExportedValue<IDebugHost>();
        //        if (success)
        //        {
        //            //TODO: how to call compare

        //            debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Compare to revision {SelectedHistoryRow.Revision} {filePath}"));
        //        }
        //        else
        //        {
        //            debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed Compare to revision {SelectedHistoryRow.Revision} {filePath}"));
        //        }
        //    }
        //}

        /// <summary>
        /// list of Filenames, paths and versions which is displayed on the status view data grid.
        /// </summary>
        private ObservableCollection<PendingChange> fileStatus = new ObservableCollection<PendingChange>();
        public ObservableCollection<PendingChange> FileStatus
        {
            get { return fileStatus; }
            set
            {
                fileStatus = value;
                OnPropertyChanged();
            }
        }
        
        public object Model
        {
            get { return null; }
        }

        public IViewModel ParentViewModel
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public object TryFindResource(object key)
        {
            return null;
        }

        public PlatformVisual View
        {
            get { return new PendingChangesView(_editSite, this); }
        }

        public string Name
        {
            get
            {
                return "SVN Pending Changes";
            }
        }

        public ImageSource SmallImage
        {
            get
            {
                return null;
            }
        }

        public QueryResult<T> QueryService<T>() where T : class
        {
            return new QueryResult<T>();
        }

        public void Initialize(IToolWindowTypeInfo info)
        {
        }



        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}