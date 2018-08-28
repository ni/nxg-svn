using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using SharpSvn;
using Svn.SvnThings;


namespace Svn.Core.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private SvnManager _svnManager = Mvx.Resolve<SvnManager>();

        private string _repository;
        public string Repository
        {
            get { return _repository; }
            set { _repository = value; RaisePropertyChanged(() => Repository); }
        }
        private string _localWorkingLocation = @"C:\svnTesting\wpfapp1";
        public string LocalWorkingLocation
        {
            get { return _localWorkingLocation; }
            set { _localWorkingLocation = value; RaisePropertyChanged(() => LocalWorkingLocation); }
        }

        private Collection<SvnLogEventArgs> _svnFileHistory;
        public Collection<SvnLogEventArgs> SvnFileHistory
        {
            get { return _svnFileHistory; }
            set { _svnFileHistory = value; RaisePropertyChanged(() => SvnFileHistory); }
        }


        private string _report;
        public string Report
        {
            get { return _report; }
            set { _report = value; RaisePropertyChanged(() => Report); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(() => Message); }
        }

        private MvxCommand _addCommand;
        public MvxCommand AddCommand
        {
            get
            {
                _addCommand = _addCommand ?? new MvxCommand(DoAddCommand);
                return _addCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Add a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoAddCommand()
        {
            var addedNotifier = true;
            var directoryAdded = false;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = _svnManager.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added)
                                {

                                    directoryAdded = true;
                                }
                                else
                                {
                                    _svnManager.Add(directory);
                                    directoryAdded = true;
                                }
                            }
                            else
                            {
                                directoryAdded = true;
                            }
                        }
                    }

                    if (directoryAdded)
                    {
                        foreach (var file in arrAllFiles)
                        {
                            foreach (var subItem in mappings)
                            {
                                if (file == subItem.Key)
                                {
                                    if (subItem.Value.Status.LocalNodeStatus == SvnStatus.NotVersioned)
                                    {
                                        _svnManager.Add(file);
                                        ReportOut("Files added!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private MvxCommand _lockCommand;
        public MvxCommand LockCommand
        {
            get
            {
                _lockCommand = _lockCommand ?? new MvxCommand(DoLockCommand);
                return _lockCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Lock on a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoLockCommand()
        {
            bool normalNotifier = true;
            bool directoryNormal = true;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = _svnManager.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                directoryNormal = false;
                            }
                            else
                            {
                                directoryNormal = true;
                            }
                        }
                    }
                }

                if (directoryNormal)
                {
                    foreach (var item in arrAllFiles)
                    {
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Normal)
                                {
                                    normalNotifier = true;
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        _svnManager.Lock(item, Message);
                                    }
                                    else
                                    {
                                        ReportOut("Please add a lock message");
                                        return;
                                    }
                                }
                                else
                                {
                                    normalNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    normalNotifier = false;
                }

            }
            if (directoryNormal)
            {
                if (normalNotifier)
                {
                    ReportOut("Locked file successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): Lock file Failed, please make the file is committed.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Lock file Failed, please make sure the directory is committed.");
                return;
            }
        }

        private MvxCommand _unlockCommand;
        public MvxCommand UnLockCommand
        {
            get
            {
                _unlockCommand = _unlockCommand ?? new MvxCommand(DoUnlockCommand);
                return _unlockCommand;
            }

        }

        /// <summary>
        /// Performs an Svn UnLock on a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoUnlockCommand()
        {
            bool normalNotifier = true;
            bool directoryNormal = true;
            bool itemLocked = false;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = _svnManager.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                directoryNormal = false;
                            }
                            else
                            {
                                directoryNormal = true;
                            }
                        }
                    }
                }

                if (directoryNormal)
                {
                    foreach (var item in arrAllFiles)
                    {
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Normal)
                                {
                                    normalNotifier = true;
                                    if (subItem.Value.Status.IsLockedLocal)
                                    {
                                        itemLocked = true;
                                        _svnManager.ReleaseLock(item);
                                    }
                                    else
                                    {
                                        itemLocked = false;
                                        ReportOut("Selected item is not Locked!");
                                        return;
                                    }
                                }
                                else
                                {
                                    normalNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    normalNotifier = false;
                }

            }
            if (directoryNormal)
            {
                if (normalNotifier && itemLocked)
                {
                    ReportOut("Unlocked file successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): UnLock file Failed, please make the file is committed and is locked.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Unlock file Failed, please make sure the directory is committed.");
                return;
            }
        }

        private MvxCommand _historyCommand;
        public MvxCommand HistoryCommand
        {
            get
            {
                _historyCommand = _historyCommand ?? new MvxCommand(DoHistoryCommand);
                return _historyCommand;
            }

        }

        /// <summary>
        /// Performs an Svn GetLogs on a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoHistoryCommand()
        {
            bool directoryNormal = true;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = _svnManager.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    string directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                directoryNormal = false;
                            }
                            else
                            {
                                directoryNormal = true;
                            }
                        }
                    }
                }

                if (directoryNormal)
                {
                    foreach (var item in arrAllFiles)
                    {
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Normal)
                                {
                                    SvnFileHistory = new Collection<SvnLogEventArgs>();
                                    SvnFileHistory = _svnManager.GetHistory(item);
                                }
                                else
                                {
                                    ReportOut("File is not committed!");
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ReportOut("Selected directory is not committed!");
                    return;
                }

            }
        }


        private MvxCommand _commitCommand;
        public MvxCommand CommitCommand
        {
            get
            {
                _commitCommand = _commitCommand ?? new MvxCommand(DoCommitCommand);
                return _commitCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Commit a file(s) that the user selects from the dialog window.
        /// </summary>
        private void DoCommitCommand()
        {
            bool addedNotifier = true;
            bool directoryAdded = true;
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.Multiselect = true;
                string[] arrAllFiles = new string[20];
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string sFileName = dialog.FileName;
                    arrAllFiles = dialog.FileNames; //used when Multiselect = true           
                }

                var mappings = _svnManager.GetMappings();

                foreach (var item in arrAllFiles)
                {
                    var directory = System.IO.Path.GetDirectoryName(item);
                    foreach (var subItem in mappings)
                    {
                        if (subItem.Key == directory)
                        {
                            if (subItem.Value.Status.LocalNodeStatus != SvnStatus.Normal)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added)
                                {
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        _svnManager.Commit(directory, Message);
                                        directoryAdded = true;
                                    }
                                    else
                                    {
                                        ReportOut("Please add a commit message");
                                        return;
                                    }
                                    
                                }
                                else
                                {
                                    directoryAdded = false;
                                }
                            }
                            else
                            {
                                directoryAdded = true;
                            }
                        }
                    }
                }

                if (directoryAdded)
                {
                    foreach (var item in arrAllFiles)
                    {
                        var directory = System.IO.Path.GetDirectoryName(item);
                        foreach (var subItem in mappings)
                        {
                            if (item == subItem.Key)
                            {
                                if (subItem.Value.Status.LocalNodeStatus == SvnStatus.Added || subItem.Value.Status.LocalNodeStatus == SvnStatus.Modified)
                                {
                                    if (!string.IsNullOrEmpty(Message))
                                    {
                                        _svnManager.Commit(item, Message);
                                    }
                                    else
                                    {
                                        ReportOut("Please add a commit message");
                                        return;
                                    }
                                }
                                else
                                {
                                    addedNotifier = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    addedNotifier = false;
                }
                
            }
            if (directoryAdded)
            {
                if (addedNotifier)
                {
                    ReportOut("Committed all file(s) successfully");
                    Message = "";
                    return;
                }
                else
                {
                    ReportOut("Error(): Commit all file(s) Failed, please make sure all selected files/directories are added.");
                    return;
                }
            }
            else
            {
                ReportOut("Error(): Commit all file(s) Failed, please make sure the directory is committed.");
                return;
            }
            
        }

        private MvxCommand _checkOutCommand;
        public MvxCommand CheckOutCommand
        {
            get
            {
                _checkOutCommand = _checkOutCommand ?? new MvxCommand(DoCheckOutCommand);
                return _checkOutCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Checkout to the specified local directory, which is then your working copy.
        /// </summary>
        private void DoCheckOutCommand()
        {
            if (_svnManager.CheckOut(LocalWorkingLocation))
            {
                ReportOut("Successful Checkout");
                _svnManager.LoadCurrentSvnItemsInLocalRepository(LocalWorkingLocation);
            }
            else
            {
                ReportOut("Checkout Failed");
            }
        }

        private MvxCommand _showStatusViewCommand;
        public MvxCommand ShowStatusViewCommand
        {
            get
            {
                _showStatusViewCommand = _showStatusViewCommand ?? new MvxCommand(DoShowStatusViewCommand);
                return _showStatusViewCommand;
            }

        }

        /// <summary>
        /// Performs an Svn Checkout to the specified local directory, which is then your working copy.
        /// </summary>
        private void DoShowStatusViewCommand()
        {
            ShowViewModel<StatusViewModel>();
        }
        

        /// <summary>
        /// Report messages to the main UI message box.
        /// </summary>
        /// <param name="reportContent"></param>
        private void ReportOut(string reportContent)
        {
            Report = reportContent;
        }

        /// <summary>
        /// On startup, we try and load the svnItems and Status cache from the default directory
        /// </summary>
        public void Loaded()
        {            
            if (Directory.Exists(LocalWorkingLocation))
            {
                if (_svnManager.IsWorkingCopy(LocalWorkingLocation))
                {
                    _svnManager.LoadCurrentSvnItemsInLocalRepository(LocalWorkingLocation);
                    ReportOut("Loaded current working copy in directory " + LocalWorkingLocation);
                }
                else
                {
                    ReportOut("Directory exists but may not be mapped, try checking out");
                }
            }
            else
            {
                ReportOut("Directory does not exist, try checking out");
            }
        }
    }
}
