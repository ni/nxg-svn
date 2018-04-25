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

namespace ViewpointSystems.Svn.Plugin.History
{
    public class HistoryViewModel : IToolWindowViewModel
    {        
        private ToolWindowEditSite _editSite;

        public ICommand CompareWithWorkingCopyCommand { get; set; }
        public ICommand RevertToThisRevisionCommand { get; set; }


        public HistoryViewModel(ToolWindowEditSite site)
        {
            _editSite = site;
            CompareWithWorkingCopyCommand = new RelayCommand(DoComapreWithWorkingCopyCommand);
            RevertToThisRevisionCommand = new RelayCommand(DoRevertToThisRevisionCommand);            
        }

        /// <summary>
        /// Revert to this version
        /// </summary>
        /// <param name="obj"></param>
        private void DoRevertToThisRevisionCommand(object obj)
        {
            if (null != SelectedHistoryRow)
            {
                var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();                
                var success = svnManager.ReverseMerge(FilePath, HistoryStatus.First().Revision, SelectedHistoryRow.Revision);
                var debugHost = _editSite.Host.GetSharedExportedValue<IDebugHost>();
                if (success)
                {                    
                    ////TODO: needs to be addressed before go live
                    ////This will revert a file once, but you have to close and reopen in order to revert the same file a second time.
                    ////If open, you also have to manually close the file and reopen to see the reversion.
                    //var referencedFile = envoy.GetReferencedFileService();
                    //referencedFile.RefreshReferencedFileAsync();

                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Revert to revision {SelectedHistoryRow.Revision} {_filePath}"));
                }
                else
                {
                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed to revert revision {SelectedHistoryRow.Revision} {_filePath}"));
                }
            }
        }

        /// <summary>
        /// Compare selected version with working version
        /// </summary>
        /// <param name="obj"></param>
        private void DoComapreWithWorkingCopyCommand(object obj)
        {
            if (null != SelectedHistoryRow)
            {
                var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
                var tempFilePathOldVersion = Path.Combine(Path.GetTempPath(), Path.GetFileName(FilePath));                
                var success = svnManager.Write(FilePath, tempFilePathOldVersion, SelectedHistoryRow.Revision);
                var debugHost = _editSite.Host.GetSharedExportedValue<IDebugHost>();
                if (success)
                {
                    //TODO: how to call compare

                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Information, $"Compare to revision {SelectedHistoryRow.Revision} {_filePath}"));
                }
                else
                {
                    debugHost.LogMessage(new DebugMessage("Viewpoint.Svn", DebugMessageSeverity.Error, $"Failed Compare to revision {SelectedHistoryRow.Revision} {_filePath}"));
                }
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
                HistoryStatus = svnManager.History(_filePath);
                OnPropertyChanged();
            }
        }

        private Collection<SvnLogEventArgs> _historyStatus = new Collection<SvnLogEventArgs>();
        /// <summary>
        /// Listing of history for given file
        /// </summary>
        public Collection<SvnLogEventArgs> HistoryStatus
        {
            get { return _historyStatus; }
            set
            {
                _historyStatus = value;
                OnPropertyChanged();
            }
        }

        private SvnLogEventArgs _selectedHistoryRow;
        /// <summary>
        /// Selected row from history table
        /// </summary>
        public SvnLogEventArgs SelectedHistoryRow
        {
            get { return _selectedHistoryRow; }
            set
            {
                _selectedHistoryRow = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// Called when the active document changes
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event data</param>
        private void HandleActiveDocumentChanged(object sender, ActiveDocumentChangedEventArgs args)
        {

            if (args.ActiveDocument == null)
            {
                FilePath = "No Selection";
                //_documentTypeControl.Text = "No Selection";
                return;
            }
            var name = args.ActiveDocument.DocumentName;
            var type = args.ActiveDocument.Envoy.ModelDefinitionType.ToString();

            FilePath = name;
            //_documentTypeControl.Text = type;
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
            get { return new HistoryView(_editSite, this); }
        }

        public string Name
        {
            get
            {
                return "SVN History";
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
