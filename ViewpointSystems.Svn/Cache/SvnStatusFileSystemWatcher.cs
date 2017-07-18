using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Cache
{
    public class SvnStatusFileSystemWatcher
    {
        private FileSystemWatcher myFileSystemWatcher;
        private SvnManagement svnManagement;
        readonly object _lock = new object();

        private string fullPath = "wc.db-journal";
        private string svnCommitPath = "C:\\UnitTestRepo\\.svn\\tmp\\";
        private WatcherChangeTypes previousEventType;

        public SvnStatusFileSystemWatcher(SvnManagement svnManagementObjectSvnManagement)
        {
            svnManagement = svnManagementObjectSvnManagement;
        }

        public void InitializeFileSystemWatcher(string path)
        {
            myFileSystemWatcher = new FileSystemWatcher();
            myFileSystemWatcher.Path = path;
            myFileSystemWatcher.NotifyFilter = NotifyFilters.Attributes |
            NotifyFilters.CreationTime |
            NotifyFilters.DirectoryName |
            NotifyFilters.FileName |
            NotifyFilters.LastWrite |
            NotifyFilters.Size;
            myFileSystemWatcher.IncludeSubdirectories = true;
            myFileSystemWatcher.Filter = "*.*";
            myFileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            myFileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            myFileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            myFileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            myFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            lock (_lock)
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Renamed:
                        svnManagement.ChangeName(e.OldFullPath, e.FullPath);
                        break;
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Deleted:
                        svnManagement.Remove(e.FullPath);
                        svnManagement.UpdateCache();
                        break;

                    case WatcherChangeTypes.Created:
                        string[] s = e.FullPath.Split('\\');
                        Array.Resize(ref s, s.Length - 1);
                        string path = "";
                        foreach (var item in s)
                        {
                            path = path + item + "\\";
                        }
                        if (path != svnCommitPath)
                        {
                            svnManagement.AddToCache(e.FullPath);
                        }
                        
                        break;

                    case WatcherChangeTypes.Changed:
                        string[] str = e.FullPath.Split('\\');
                        var x = str.GetValue(str.Length - 1);
                        if (x.ToString() == fullPath)
                        { 
                            svnManagement.UpdateCache();
                        }
                        break;
                }
                previousEventType = e.ChangeType;
            }
        }
    }
}
