using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
using ViewpointSystems.Svn.SccThings;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Cache
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    sealed partial class SvnStatusCache : ISvnStatusCache, ISvnItemChange
    {
        private readonly object _lock = new object();
        private readonly SvnClient _client;
        public readonly Dictionary<string, SvnItem> Map; // Maps from full-normalized paths to SvnItems
        public readonly Dictionary<string, SvnDirectory> DirectoryMap;
        bool _enableUpgrade;        
        private bool _usingShell = false;
        private SvnStatusFileSystemWatcher _statusFileSystemWatcher;

        public SvnStatusCache(bool shell, SvnManager svnManager)
        {
            _client = new SvnClient();
            Map = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            DirectoryMap = new Dictionary<string, SvnDirectory>(StringComparer.OrdinalIgnoreCase);
            _usingShell = shell;
            if (_usingShell)
            {
                //InitializeShellMonitor();
            }
            else
            {
                _statusFileSystemWatcher = new SvnStatusFileSystemWatcher(svnManager);
            }

        }

        /// <summary>
        /// Starts the File System watcher, referencing a directory of interest.
        /// </summary>
        /// <param name="path"></param>
        public void StartFileSystemWatcher(string path)
        {
            if (!_usingShell)
            {
                _statusFileSystemWatcher.InitializeFileSystemWatcher(path);
            }
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                if (_usingShell)
                {
                    //ReleaseShellMonitor(disposing);
                }

                _client.Dispose();
            }
            finally
            {
                Dispose(disposing);
            }
        }

        public void RefreshItem(SvnItem item, SvnNodeKind nodeKind)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            RefreshPath(item.FullPath, nodeKind);

            var updateItem = (ISvnItemUpdate)item;

            if (!updateItem.IsStatusClean())
            {
                // Ok, the status update did not refresh the item requesting to be refreshed
                // That means the item is not here or RefreshPath would have added it

                SvnItem other;
                if (Map.TryGetValue(item.FullPath, out other) && other != item)
                {
                    updateItem.RefreshTo(other); // This item is no longer current; but we have the status anyway
                }
                else
                {
                    Debug.Assert(false, "RefreshPath did not deliver up to date information",
                        "The RefreshPath public api promises delivering up to date data, but none was received");

                    updateItem.RefreshTo(item.Exists ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                }
            }

            Debug.Assert(updateItem.IsStatusClean(), "The item requesting to be updated is updated");
        }

        void ISvnStatusCache.RefreshWCRoot(SvnItem svnItem)
        {
            if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            Debug.Assert(svnItem.IsDirectory);

            // We retrieve nesting information by walking the entry data of the parent directory
            lock (_lock)
            {
                string root;

                try
                {
                    root = _client.GetWorkingCopyRoot(svnItem.FullPath);
                }
                catch { root = null; }

                if (root == null)
                {
                    ((ISvnItemUpdate)svnItem).SetState(SvnItemState.None, SvnItemState.IsWCRoot);
                    return;
                }

                // Set all nodes between this node and the root to not-a-wcroot
                ISvnItemUpdate oi;
                while (root.Length < svnItem.FullPath.Length)
                {
                    oi = (ISvnItemUpdate)svnItem;

                    oi.SetState(SvnItemState.None, SvnItemState.IsWCRoot);
                    svnItem = svnItem.Parent;

                    if (svnItem == null)
                        return;
                }

                oi = svnItem;

                if (svnItem.FullPath == root)
                    oi.SetState(SvnItemState.IsWCRoot, SvnItemState.None);
                else
                    oi.SetState(SvnItemState.None, SvnItemState.IsWCRoot);
            }
        }

        public SvnItem CreateItem(string fullPath, SvnStatusData status)
        {
            return new SvnItem(fullPath, status);
        }

        public SvnItem CreateItem(string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
        {
            return new SvnItem(fullPath, status, nodeKind);
        }

        public SvnItem CreateItem(string fullPath, NoSccStatus status)
        {
            return CreateItem(fullPath, status, SvnNodeKind.Unknown);
        }

        /// <summary>
        /// Stores the item in the caching dictionary/ies
        /// </summary>
        /// <param name="item"></param>
        public void StoreItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Map[item.FullPath] = item;

            SvnDirectory dir;
            if (DirectoryMap.TryGetValue(item.FullPath, out dir))
            {
                if (item.IsDirectory)
                {
                    ((ISvnDirectoryUpdate)dir).Store(item);
                }
                else
                    ScheduleForCleanup(dir);
            }

            var parentDir = SvnTools.GetNormalizedDirectoryName(item.FullPath);

            if (string.IsNullOrEmpty(parentDir) || parentDir == item.FullPath)
                return; // Skip root directory

            if (DirectoryMap.TryGetValue(item.FullPath, out dir))
            {
                ((ISvnDirectoryUpdate) dir)?.Store(item);
            }
        }

        public void RemoveItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var deleted = false;
            SvnDirectory dir;
            if (DirectoryMap.TryGetValue(item.FullPath, out dir))
            {
                // The item is a directory itself.. remove it's map
                if (dir.Directory == item)
                {
                    DirectoryMap.Remove(item.FullPath);
                    deleted = true;
                }
            }

            SvnItem other;
            if (Map.TryGetValue(item.FullPath, out other))
            {
                if (item == other)
                    Map.Remove(item.FullPath);
            }

            if (!deleted)
                return;

            var parentDir = SvnTools.GetNormalizedDirectoryName(item.FullPath);

            if (string.IsNullOrEmpty(parentDir) || parentDir == item.FullPath)
                return; // Skip root directory

            if (DirectoryMap.TryGetValue(item.FullPath, out dir))
            {
                dir.Remove(item.FullPath);
            }
        }

        /// <summary>
        /// Refreshes the specified path using the specified depth
        /// </summary>
        /// <param name="path">A normalized path</param>
        /// <param name="pathKind"></param>
        /// <param name="depth"></param>
        /// <remarks>
        /// If the path is a file and depth is greater that <see cref="SvnDepth.Empty"/> the parent folder is walked instead.
        /// 
        /// <para>This method guarantees that after calling it at least one up-to-date item exists 
        /// in the statusmap for <paramref name="path"/>. If we can not find information we create
        /// an unspecified item
        /// </para>
        /// </remarks>
        void RefreshPath(string path, SvnNodeKind pathKind)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var walkPath = path;
            var walkingDirectory = false;

            switch (pathKind)
            {
                case SvnNodeKind.Directory:
                    walkingDirectory = true;
                    break;
                case SvnNodeKind.File:
                    walkPath = SvnTools.GetNormalizedDirectoryName(path);
                    walkingDirectory = true;
                    break;
                default:
                    try
                    {
                        if (File.Exists(path)) // ### Not long path safe
                        {
                            pathKind = SvnNodeKind.File;
                            goto case SvnNodeKind.File;
                        }
                    }
                    catch (PathTooLongException)
                    { /* Fall through */ }
                    break;
            }

            var args = new SvnStatusArgs();
            args.Depth = SvnDepth.Children;
            args.RetrieveAllEntries = true;
            args.RetrieveIgnoredEntries = true;
            args.ThrowOnError = false;

            lock (_lock)
            {
                ISvnDirectoryUpdate updateDir;
                SvnItem walkItem = null;

                // We get more information for free, lets use that to update other items
                DirectoryMap.TryGetValue(walkPath, out var directory);
                if (null != directory)
                {
                    updateDir = directory;
                    updateDir.TickAll();
                }
                else
                {
                    // No existing directory instance, let's create one
                    directory = new SvnDirectory(walkPath);
                    updateDir = directory = GetDirectory(walkPath);
                    DirectoryMap[walkPath] = directory;
                }


                bool ok;
                var statSelf = false;
                var noWcAtAll = false;

                // Don't retry file open/read operations on failure. These would only delay the result 
                // (default number of delays = 100)
                using (new SharpSvn.Implementation.SvnFsOperationRetryOverride(0))
                {
                    ok = _client.Status(walkPath, args, RefreshCallback);
                }

                if (directory != null)
                    walkItem = directory.Directory; // Might have changed via casing

                if (!statSelf && null != walkItem)
                {
                    if (((ISvnItemUpdate)walkItem).ShouldRefresh())
                        statSelf = true;
                    else if (walkingDirectory && !walkItem.IsVersioned)
                        statSelf = true;
                }

                if (statSelf)
                {
                    // Svn did not stat the items for us.. Let's make something up

                    if (walkingDirectory)
                        StatDirectory(walkPath, directory, noWcAtAll);
                    else
                    {
                        // Just stat the item passed and nothing else in the Depth.Empty case

                        if (walkItem == null)
                        {
                            var truepath = SvnTools.GetTruePath(walkPath); // Gets the on-disk casing if it exists

                            StoreItem(walkItem = CreateItem(truepath ?? walkPath,
                                (truepath != null) ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown));
                        }
                        else
                        {
                            ((ISvnItemUpdate)walkItem).RefreshTo(walkItem.Exists ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                        }
                    }
                }

                if (directory != null)
                {
                    foreach (ISvnItemUpdate item in directory)
                    {
                        if (item.IsItemTicked()) // These items were not found in the stat calls
                            item.RefreshTo(NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                    }

                    if (updateDir.ScheduleForCleanup)
                        ScheduleForCleanup(directory); // Handles removing already deleted items
                    // We keep them cached for the current command only
                }


                SvnItem pathItem; // We promissed to return an updated item for the specified path; check if we updated it

                if (!Map.TryGetValue(path, out pathItem))
                {
                    // We did not; it does not even exist in the cache
                    StoreItem(pathItem = CreateItem(path, NoSccStatus.NotExisting));

                    if (directory != null)
                    {
                        updateDir.Store(pathItem);
                        ScheduleForCleanup(directory);
                    }
                }
                else
                {
                    ISvnItemUpdate update = pathItem;

                    if (!update.IsStatusClean())
                    {
                        update.RefreshTo(NoSccStatus.NotExisting, SvnNodeKind.Unknown); // We did not see it in the walker

                        if (directory != null)
                        {
                            ((ISvnDirectoryUpdate)directory).Store(pathItem);
                            ScheduleForCleanup(directory);
                        }
                    }
                }
            }
        }

        private void StatDirectory(string walkPath, SvnDirectory directory, bool noWcAtAll)
        {
            // Note: There is a lock(_lock) around this in our caller

            bool canRead;
            var adminName = SvnClient.AdministrativeDirectoryName;
            var noSccStatus = noWcAtAll ? NoSccStatus.NotVersionable : NoSccStatus.NotVersioned;
            foreach (var node in SccFileSystemNode.GetDirectoryNodes(walkPath, out canRead))
            {
                if (string.Equals(node.Name, adminName, StringComparison.OrdinalIgnoreCase) || node.IsHiddenOrSystem)
                    continue;

                SvnItem item;
                if (node.IsFile)
                {
                    if (!Map.TryGetValue(node.FullPath, out item))
                        StoreItem(CreateItem(node.FullPath, noSccStatus, SvnNodeKind.File));
                    else
                    {
                        ISvnItemUpdate updateItem = item;
                        if (updateItem.ShouldRefresh())
                            updateItem.RefreshTo(noSccStatus, SvnNodeKind.File);
                    }
                }
                else
                {
                    if (!Map.TryGetValue(node.FullPath, out item))
                        StoreItem(CreateItem(node.FullPath, noSccStatus, SvnNodeKind.Directory));
                    // Don't clear state of a possible working copy
                }
            }

            if (canRead) // The directory exists
            {
                SvnItem item;

                if (!Map.TryGetValue(walkPath, out item))
                {
                    StoreItem(CreateItem(walkPath, NoSccStatus.NotVersioned, SvnNodeKind.Directory));
                    // Mark it as existing if we are sure 
                }
                else
                {
                    ISvnItemUpdate updateItem = item;
                    if (updateItem.ShouldRefresh())
                        updateItem.RefreshTo(NoSccStatus.NotVersioned, SvnNodeKind.Directory);
                }
            }

            // Note: There is a lock(_lock) around this in our caller
        }

        bool _sendUpgrade;
        public void ResetUpgradeWarning()
        {
            _sendUpgrade = false;
        }

        bool _postedCleanup;
        List<SvnDirectory> _cleanup = new List<SvnDirectory>();
        private void ScheduleForCleanup(SvnDirectory directory)
        {
            lock (_lock)
            {
                if (!_cleanup.Contains(directory))
                    _cleanup.Add(directory);
            }
        }

        public void OnCleanup()
        {
            lock (_lock)
            {
                _postedCleanup = false;

                while (_cleanup.Count > 0)
                {
                    var dir = _cleanup[0];
                    var path = dir.FullPath;

                    _cleanup.RemoveAt(0);

                    for (var i = 0; i < dir.Count; i++)
                    {
                        var item = dir[i];
                        if (((ISvnItemUpdate)item).ShouldClean())
                        {
                            RemoveItem(item);
                            dir.RemoveAt(i--);
                        }
                    }

                    if (dir.Count == 0)
                    {
                        // We cache the path before.. as we don't want the svnitem to be generated again
                        DirectoryMap.Remove(path);
                    }
                }
            }
        }

        static bool NewFullPathOk(SvnItem item, string fullPath, SvnStatusData status)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (status == null)
                throw new ArgumentNullException("status");

            if (fullPath == item.FullPath)
                return true;

            switch (status.LocalNodeStatus)
            {
                case SvnStatus.Added:
                case SvnStatus.Conflicted:
                case SvnStatus.Merged:
                case SvnStatus.Modified:
                case SvnStatus.Normal:
                case SvnStatus.Replaced:
                case SvnStatus.Deleted:
                case SvnStatus.Incomplete:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Called from RefreshPath's call to <see cref="SvnClient::Status"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// All information we receive here is live from SVN and Disk and is therefore propagated
        /// in all SvnItems wishing information
        /// </remarks>
        void RefreshCallback(object sender, SvnStatusEventArgs e)
        {
            // Note: There is a lock(_lock) around this in our caller

            var status = new SvnStatusData(e);
            var path = e.FullPath; // Fully normalized

            SvnItem item;
            if (!Map.TryGetValue(path, out item) || !NewFullPathOk(item, path, status))
            {
                // We only create an item if we don't have an existing
                // with a valid path. (No casing changes allowed!)

                var newItem = CreateItem(path, status);
                StoreItem(newItem);

                if (item != null)
                {
                    ((ISvnItemUpdate)item).RefreshTo(newItem);
                    item.Dispose();
                }

                item = newItem;
            }
            else
                ((ISvnItemUpdate)item).RefreshTo(status);

            // Note: There is a lock(_lock) around this in our caller
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void ISccStatusCache.MarkDirty(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var normPath = SvnTools.GetNormalizedFullPath(path);

            lock (_lock)
            {
                SvnItem item;

                if (Map.TryGetValue(normPath, out item))
                {
                    item.MarkDirty();
                }
            }
        }

        void ISccStatusCache.MarkDirtyRecursive(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                var names = new List<string>();

                foreach (var v in Map.Values)
                {
                    var name = v.FullPath;
                    if (v.IsBelowPath(path))
                    {
                        v.MarkDirty();
                    }
                }
            }
        }

        public IEnumerable<string> GetCachedBelow(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                var items = new List<string>();

                foreach (var v in Map.Values)
                {
                    if (v.IsBelowPath(path))
                        items.Add(v.FullPath);
                }

                return items;
            }
        }

        public IEnumerable<string> GetCachedBelow(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                var items = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);

                foreach (var path in paths)
                {
                    foreach (var v in Map.Values)
                    {
                        if (v.IsBelowPath(path))
                            items[v.FullPath] = v;
                    }
                }

                return new List<string>(items.Keys);
            }
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void ISccStatusCache.MarkDirty(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            lock (_lock)
            {
                foreach (var path in paths)
                {
                    var normPath = SvnTools.GetNormalizedFullPath(path);
                    SvnItem item;

                    if (Map.TryGetValue(normPath, out item))
                    {
                        item.MarkDirty();
                    }
                }
            }
        }


        public SvnItem this[string path]
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");

                path = SvnTools.GetNormalizedFullPath(path);

                return GetAlreadyNormalizedItem(path);
            }
        }

        public SvnItem GetAlreadyNormalizedItem(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                SvnItem item;

                if (!Map.TryGetValue(path, out item))
                {
                    var truePath = SvnTools.GetTruePath(path, true);

                    // Just create an item based on his name. Delay the svn calls as long as we can
                    StoreItem(item = new SvnItem(truePath ?? path, NoSccStatus.Unknown, SvnNodeKind.Unknown));

                    //item.MarkDirty(); // Load status on first access
                }

                return item;
            }
        }

        /// <summary>
        /// Clears the whole cache; called from solution closing (Scc)
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                DirectoryMap.Clear();
                Map.Clear();
            }
        }

        void ISvnStatusCache.SetSolutionContained(string path, bool inSolution, bool sccExcluded)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item;
            if (Map.TryGetValue(path, out item))
                ((ISvnItemStateUpdate)item).SetSolutionContained(inSolution, sccExcluded);
        }

        #region IFileStatusCache Members


        public SvnDirectory GetDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                SvnDirectory dir;

                if (DirectoryMap.TryGetValue(path, out dir))
                    return dir;

                var item = this[path];

                if (item.IsDirectory)
                {
                    dir = new SvnDirectory(path);
                    dir.Add(item);
                    return dir;
                }
                else
                    return null;
            }
        }

        #endregion

        //internal void BroadcastChanges()
        //{
        //    ISvnItemStateUpdate update;
        //    if (Map.Count > 0)
        //        update = EnumTools.GetFirst(Map.Values);
        //    else
        //        update = this["C:\\"]; // Just give me a SvnItem instance to access the interface

        //    IList<SvnItem> updates = update.GetUpdateQueueAndClearScheduled();

        //    if (updates != null)
        //        OnSvnItemsChanged(new SvnItemsEventArgs(updates));
        //}

        public event EventHandler<SvnItemsEventArgs> SvnItemsChanged;

        public void OnSvnItemsChanged(SvnItemsEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (SvnItemsChanged != null)
                SvnItemsChanged(this, e);
        }

        public bool EnableUpgradeCommand
        {
            get { return _enableUpgrade; }
        }

        SccItem ISccStatusCache.this[string path]
        {
            get { return this[path]; }
        }
    }
}
