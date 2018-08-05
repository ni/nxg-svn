using System;
using System.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Threading;
using Svn.SvnThings;
using Application = System.Windows.Application;

namespace Svn.Core.ViewModels
{
    /// <summary>
    /// View model for the StatusView
    /// </summary>
    public class StatusViewModel : BaseViewModel
    {
        private SvnManager _svnManager = Mvx.Resolve<SvnManager>();
        
        /// <summary>
        /// True, actively monitor for changes
        /// </summary>
        public bool Monitor = true;
        private int items = 1;

        public StatusViewModel()
        {
            Mvx.RegisterSingleton<StatusViewModel>(this);
            items = 1;
        }

        /// <summary>
        /// Fires when the user opens the StatusView.
        /// </summary>
        public override void Start()
        {
            base.Start();                                    
            Monitor = true;
            MonitorStatus();
            _svnManager.RemoveItemFromProjectEvent += SvnManagerRemoveItemFromViewer;
        }

        private void SvnManagerRemoveItemFromViewer(object sender, EventArgs e)
        {
            foreach (var item in FileStatus)
            {
                if (item.FilePath == sender.ToString())
                {
                    Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Background,
                                new Action(() => {
                                    FileStatus.Remove(item);
                                }));
                }
            }
        }       

        /// <summary>
        /// list of Filenames, paths and versions which is displayed on the status view data grid.
        /// </summary>
        private ObservableCollection<SvnFileStatus> fileStatus= new ObservableCollection<SvnFileStatus>();
        public ObservableCollection<SvnFileStatus> FileStatus 
        {
            get { return fileStatus; }
            set
            {
                fileStatus = value;
                RaisePropertyChanged(() => FileStatus);                           
            }
        }

        /// <summary>
        /// Used when renaming, or removing files from the status view/cache. 
        /// This function updates our view.
        /// </summary>
        /// <param name="svnItem"></param>
        public void RemoveItemFromViewer(SvnItem svnItem)
        {
            foreach (var item in FileStatus)
            {
                if (item.FilePath == svnItem.FullPath)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Background,
                                new Action(() => {
                                    FileStatus.Remove(item);
                                }));
                }
            }
        }

        /// <summary>
        /// Monitor Status spins up a thread which while the view is open, we can see live add and commits.
        /// </summary>
        public async void MonitorStatus()
        {
           
            await Task.Run(() =>
            {
                while (Monitor)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                                DispatcherPriority.Background,
                                new Action(() => {
                                    var x = _svnManager.GetMappings();
                                    if (FileStatus.Count != x.Count)
                                    {
                                        foreach (var item in x)
                                        {
                                            if(items <= x.Count)
                                            {
                                                var stat = new SvnFileStatus(item.Value.Name, item.Value.FullPath, item.Value.Status.LocalNodeStatus.ToString());
                                                var y = FileStatus.Any(st => st.FilePath == stat.FilePath);
                                                if (!y)
                                                {
                                                    FileStatus.Add(new SvnFileStatus(item.Value.Name, item.Value.FullPath, item.Value.Status.LocalNodeStatus.ToString()));
                                                    if (items != x.Count)
                                                    {
                                                        items++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int i = 0;
                                        foreach (var item in x)
                                        {
                            
                                            FileStatus[i].Filename = item.Value.Name;
                                            FileStatus[i].FilePath = item.Value.FullPath;
                                            FileStatus[i].Version = item.Value.Status.LocalNodeStatus.ToString();
                                            i++;
                                        }
                                    }
                                }));
                    Thread.Sleep(1000);

                    
                }
            });
        }
    }
}
