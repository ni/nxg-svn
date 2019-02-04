using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using NationalInstruments;
using NationalInstruments.Comparison;
using NationalInstruments.Core;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel.Envoys;
using SharpSvn;

namespace Svn.Plugin.History
{
    public class HistoryViewModel : IToolWindowViewModel
    {        
        private ToolWindowEditSite _editSite;
        private Envoy _envoy;

        public ICommand CompareWithWorkingCopyCommand { get; set; }
        public ICommand RevertToThisRevisionCommand { get; set; }


        public HistoryViewModel(ToolWindowEditSite site)
        {
            _editSite = site;
            CompareWithWorkingCopyCommand = new RelayCommand(DoCompareWithWorkingCopyCommand, CanCompareWithWorkingCopy);
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

                    debugHost.LogMessage(new DebugMessage("Svn", DebugMessageSeverity.Information, $"Revert to revision {SelectedHistoryRow.Revision} {FilePath}"));
                }
                else
                {
                    debugHost.LogMessage(new DebugMessage("Svn", DebugMessageSeverity.Error, $"Failed to revert revision {SelectedHistoryRow.Revision} {FilePath}"));
                }
            }
        }

        /// <summary>
        /// Compare selected version with working version
        /// </summary>
        /// <param name="obj"></param>
        private void DoCompareWithWorkingCopyCommand(object obj)
        {
            if (null != SelectedHistoryRow)
            {
                var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
                var fileNameOldVersion = $"{Envoy.Name.Last} #{SelectedHistoryRow.Revision}";
                var tempFilePathOldVersion = Path.Combine(Path.GetTempPath(), fileNameOldVersion);                
                var success = svnManager.Write(FilePath, tempFilePathOldVersion, SelectedHistoryRow.Revision);
                var debugHost = _editSite.Host.GetSharedExportedValue<IDebugHost>();
                if (success)
                {
                    var parameter = new CompareItemsCommandParameter(tempFilePathOldVersion, Envoy);
                    CompareCommands.CompareItemsCommand.Execute(parameter);

                    debugHost.LogMessage(new DebugMessage("Svn", DebugMessageSeverity.Information, $"Compare to revision {SelectedHistoryRow.Revision} {FilePath}"));
                }
                else
                {
                    debugHost.LogMessage(new DebugMessage("Svn", DebugMessageSeverity.Error, $"Failed Compare to revision {SelectedHistoryRow.Revision} {FilePath}"));
                }
            }
        }

        private bool CanCompareWithWorkingCopy(object obj)
        {
            if (Envoy == null)
            {
                return false;
            }
            var compareEngineManager = _editSite.Host.GetSharedExportedValue<ICompareEngineManager>();
            return compareEngineManager.IsFileTypeSupportedByCompare(Envoy.OverridingModelDefinitionType);
        }

        /// <summary>
        /// The file path of <see cref="Envoy"/>
        /// </summary>
        public string FilePath => Envoy?.GetFilePath();

        /// <summary>
        /// The file for which we are showing history.
        /// </summary>
        public Envoy Envoy
        {
            get { return _envoy; }
            set
            {
                _envoy = value;
                var svnManager = _editSite.Host.GetSharedExportedValue<SvnManagerPlugin>();
                HistoryStatus = svnManager.History(FilePath);
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
