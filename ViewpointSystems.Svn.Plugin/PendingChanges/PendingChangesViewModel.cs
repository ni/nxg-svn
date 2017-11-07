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

namespace ViewpointSystems.Svn.Plugin.PendingChanges
{
    public class PendingChangesViewModel : IToolWindowViewModel
    {        
        private ToolWindowEditSite _editSite;

        //public ICommand CompareWithWorkingCopyCommand { get; set; }
        
        public PendingChangesViewModel(ToolWindowEditSite site)
        {
            _editSite = site;
            //CompareWithWorkingCopyCommand = new RelayCommand(DoComapreWithWorkingCopyCommand);            
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