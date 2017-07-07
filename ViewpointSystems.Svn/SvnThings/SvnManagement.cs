using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;
//using SharpSvnTest.Core.ViewModels;
using ViewpointSystems.Svn.Cache;

namespace ViewpointSystems.Svn.SvnThings
{
    public class SvnManagement
    {
        private SvnClient svnClient = new SvnClient();
        private string path = Properties.Settings.Default.URL;
        private Uri uri;
        private string repo;
        private SvnStatusCache statusCache;
        //private StatusViewModel statusViewModel;

        //public event 
        // This event is what is used to notify the main UI that the connection to the camera has been established.
        public event EventHandler RemoveItemFromViewer;


        public SvnManagement()
        {
            uri = new Uri(path);
            statusCache = new SvnStatusCache(false, this);
        }

        public void InitializeStatusView()
        {
            //statusViewModel = Mvx.Resolve<StatusViewModel>();
        }

        /// <summary>
        /// Loads the current SVN Items from the repository, and builds a StatusCache object
        /// </summary>
        public void LoadCurrentSvnItemsInLocalRepository(string localRepo)
        {
            repo = localRepo;
            AddtoCache(repo);
            statusCache.StartFileSystemWatcher(repo);
        }

        public void ChangeName(string old, string snew)
        {
            SvnItem itemOfChoice;
            if (statusCache._map.ContainsKey(old))
            {
                if (statusCache._map.TryGetValue(old, out itemOfChoice))
                {
                    if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.NotVersioned)
                    {
                        Remove(old);
                        RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                        //statusViewModel.RemoveItemFromViewer(itemOfChoice);
                        AddtoCache(snew);
                    }
                    else if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.Added)
                    {
                        UpdateCache();
                        Remove(old);
                        RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                        //statusViewModel.RemoveItemFromViewer(itemOfChoice);
                    }
                    else if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.Normal)
                    {
                        UpdateCache();
                    }
                }
            }
        }

        public void AddtoCache(string path)
        {
            using (SvnClient svnClient = new SvnClient())
            {
                Collection<SvnStatusEventArgs> statusContents;
                repo = path;
                try
                {
                    if (svnClient.GetStatus(repo, new SvnStatusArgs
                    {
                        Depth = SvnDepth.Infinity,
                        RetrieveAllEntries = true
                    }, out statusContents))
                    {
                        foreach (var content in statusContents)
                        {
                            var contentStatusData = new SvnStatusData(content);
                            statusCache.StoreItem(statusCache.CreateItem(content.FullPath, contentStatusData));
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        /// <summary>
        /// Get the status of each known svn Item in the working local copy.
        /// </summary>
        /// <returns></returns>
        public Collection<SvnStatusEventArgs> GetStatus()
        {
            Collection<SvnStatusEventArgs> statusContents;
            
            svnClient.GetStatus(repo, new SvnStatusArgs
            {
                Depth = SvnDepth.Infinity,
                RetrieveAllEntries = true
            }, out statusContents);
            return statusContents;
        }

        /// <summary>
        /// Remove item from status cache and viewer
        /// </summary>
        public void Remove(string path)
        {
            SvnItem itemOfChoice;
            if (statusCache._map.ContainsKey(path))
            {
                if (statusCache._map.TryGetValue(path, out itemOfChoice))
                {
                    RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                    //statusViewModel.RemoveItemFromViewer(itemOfChoice);
                    statusCache._map.Remove(path);
                }
            }
        }

        /// <summary>
        /// Update the items in the status cache when their is an svn changed event.
        /// </summary>
        public void UpdateCache()
        {
            DoAgain:
            var j = statusCache._map.Count;
            
            foreach (var item in statusCache._map.Values)
            {
                statusCache.RefreshItem(item, item.NodeKind);
                if (j != statusCache._map.Count)
                {
                    goto DoAgain;
                }
            }
        }

        /// <summary>
        /// Get all of the SvnItems that we have stored with their statuses
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, SvnItem> GetMappings()
        {
            return statusCache._map;
        }

        /// <summary>
        /// Adds a file to be committed 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Add(string path)
        {
            using (SvnClient client = new SvnClient())
            {
                SvnAddArgs args = new SvnAddArgs();
                args.Depth = SvnDepth.Empty;
                Console.Out.WriteLine(path);
                args.AddParents = true;
                
                try
                {
                    return client.Add(path, args);
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// Lock a committed file
        /// </summary>
        /// <param name="target"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool SvnLock(string target, string comment)
        {
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    return client.Lock(target, comment);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw e;
                }
            }
        }

        //private void LogHandler(object sender, SvnLogEventArgs svnLogEventArgs)
        //{
            
        //}

        /// <summary>
        /// Unlock a locked file.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SvnUnLock(string target)
        {
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    return client.Unlock(target);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw e;
                }
            }
        }

        public bool SvnRename(string oldPath, string newPath)
        {
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    return client.Move(oldPath, newPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw e;
                }
            }
        }

        public bool CommitChosenFiles(string path, string message)
        {
            bool successfulCommit = false;

            using (SvnClient client = new SvnClient())
            {
                SvnCommitArgs args = new SvnCommitArgs();

                args.LogMessage = message;
                args.ThrowOnError = true;
                args.ThrowOnCancel = true;
                args.Depth = SvnDepth.Empty;
                

                try
                {
                    return client.Commit(path, args);
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        throw new Exception(e.InnerException.Message, e);
                    }

                    throw e;
                }
            }
        }

        /// <summary>
        /// Commits the Staged/Added files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Commit(string path, string message)
        {
            using (SvnClient client = new SvnClient())
            {
                SvnCommitArgs args = new SvnCommitArgs();

                args.LogMessage = message;
                args.ThrowOnError = true;
                args.ThrowOnCancel = true;

                try
                {
                    SvnStatusArgs sa = new SvnStatusArgs();
                    sa.Depth = SvnDepth.Infinity;
                    sa.RetrieveAllEntries = true; //the new line
                    Collection<SvnStatusEventArgs> statuses;

                    client.GetStatus(repo, sa, out statuses);
                    foreach (var item in statuses)
                    {
                        if (item.LocalContentStatus == SvnStatus.Added || item.LocalContentStatus == SvnStatus.Modified)
                        {
                            client.Commit(item.FullPath, args);
                        }
                    }
                    return true; 
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                    {
                        throw new Exception(e.InnerException.Message, e);
                    }

                    throw e;
                }
            }
        }

        /// <summary>
        /// Checks to see if the directory is the working local copy
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsWorkingCopy(string path)
        {
            using (SvnClient client = new SvnClient())
            {
                var uri = client.GetUriFromWorkingCopy(path);
                return uri != null;
            }
        }

        /// <summary>
        /// Checks out the latest from the Server, and updates the local working copy.
        /// </summary>
        /// <returns></returns>
        public bool CheckOut(string localRepo)
        {
            bool returnVal = false;
            SvnUriTarget svnUriTarget = new SvnUriTarget(uri);
            repo = localRepo;
            if (svnClient.CheckOut(svnUriTarget, repo))
            {
                returnVal = true;
            }
            return returnVal;
        }
    }
}
