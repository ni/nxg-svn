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
        private string repo;
        private SvnStatusCache statusCache;

        // This event is what is used to notify the main UI that the connection to the camera has been established.
        public event EventHandler RemoveItemFromViewer;
        public SvnManagement()
        {            
            statusCache = new SvnStatusCache(false, this);
        }

        /// <summary>
        /// Loads the current SVN Items from the repository, and builds a StatusCache object
        /// </summary>
        public void LoadCurrentSvnItemsInLocalRepository(string localRepo)
        {
            repo = localRepo;
            AddToCache(repo);
            statusCache.StartFileSystemWatcher(repo);
        }

        public void ChangeName(string old, string snew)
        {
            if (statusCache._map.ContainsKey(old))
            {
                if (statusCache._map.TryGetValue(old, out SvnItem itemOfChoice))
                {
                    if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.NotVersioned)
                    {
                        Remove(old);
                        RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                        AddToCache(snew);
                    }
                    else if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.Added)
                    {
                        UpdateCache();
                        Remove(old);
                        RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                    }
                    else if (itemOfChoice.Status.LocalNodeStatus == SvnStatus.Normal)
                    {
                        UpdateCache();
                    }
                }
            }
        }

        public void AddToCache(string path)
        {
            using (var svnClient = new SvnClient())
            {
                repo = path;
                try
                {
                    if (svnClient.GetStatus(repo, new SvnStatusArgs
                {
                    Depth = SvnDepth.Infinity,
                    RetrieveAllEntries = true
                }, out Collection<SvnStatusEventArgs> statusContents))
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
                    //TODO: confirm this is ok
                }
            }
        }

        /// <summary>
        /// Get the status of each known SVN Item in the working local copy.
        /// </summary>
        /// <returns></returns>
        public Collection<SvnStatusEventArgs> GetStatus()
        {
            using (var svnClient = new SvnClient())
            {
                svnClient.GetStatus(repo, new SvnStatusArgs
                {
                    Depth = SvnDepth.Infinity,
                    RetrieveAllEntries = true
                }, out Collection<SvnStatusEventArgs> statusContents);
                return statusContents;
            }
        }

        /// <summary>
        /// Remove item from status cache and viewer
        /// </summary>
        public void Remove(string path)
        {
            if (statusCache._map.ContainsKey(path))
            {
                if (statusCache._map.TryGetValue(path, out SvnItem itemOfChoice))
                {
                    RemoveItemFromViewer?.Invoke(itemOfChoice, new EventArgs());
                    statusCache._map.Remove(path);
                }
            }
        }

        /// <summary>
        /// Update the items in the status cache when their is an SVN changed event.
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
            using (var svnClient = new SvnClient())
            {
                var args = new SvnAddArgs();
                args.Depth = SvnDepth.Empty;
                Console.Out.WriteLine(path);
                args.AddParents = true;

                try
                {
                    return svnClient.Add(path, args);
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
            using (var svnClient = new SvnClient())
            {
                //try
                //{
                return svnClient.Lock(target, comment);

                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw e;
                //}
            }
        }



        /// <summary>
        /// Unlock a locked file.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool SvnUnlock(string target)
        {
            using (var svnClient = new SvnClient())
            {
                //try
                //{
                return svnClient.Unlock(target);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw e;
                //}
            }
        }

        public bool SvnRename(string oldPath, string newPath)
        {
            using (var svnClient = new SvnClient())
            {
                //try
                //{
                return svnClient.Move(oldPath, newPath);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw e;
                //}
            }

        }

        public bool CommitChosenFiles(string path, string message)
        {
            using (var svnClient = new SvnClient())
            {
                var successfulCommit = false;


                var args = new SvnCommitArgs();

                args.LogMessage = message;
                args.ThrowOnError = true;
                args.ThrowOnCancel = true;
                args.Depth = SvnDepth.Empty;


                try
                {
                    return svnClient.Commit(path, args);
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
            using (var svnClient = new SvnClient())
            {
                var args = new SvnCommitArgs();

                args.LogMessage = message;
                args.ThrowOnError = true;
                args.ThrowOnCancel = true;

                try
                {
                    var sa = new SvnStatusArgs();
                    sa.Depth = SvnDepth.Infinity;
                    sa.RetrieveAllEntries = true; //the new line
                    Collection<SvnStatusEventArgs> statuses;

                    svnClient.GetStatus(repo, sa, out statuses);
                    foreach (var item in statuses)
                    {
                        if (item.LocalContentStatus == SvnStatus.Added || item.LocalContentStatus == SvnStatus.Modified)
                        {
                            svnClient.Commit(item.FullPath, args);
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
            using (var svnClient = new SvnClient())
            {
                var uri = svnClient.GetUriFromWorkingCopy(path);
                return uri != null;
            }
        }

        /// <summary>
        /// Checks out the latest from the Server, and updates the local working copy.
        /// </summary>
        /// <returns></returns>
        public bool CheckOut(string localRepo)
        {
            using (var svnClient = new SvnClient())
            {
                var localUri = svnClient.GetUriFromWorkingCopy(localRepo);
                var svnUriTarget = new SvnUriTarget(localUri);   //TODO: why is this the only place URI is used?
                repo = localRepo;
                return svnClient.CheckOut(svnUriTarget, repo);
            }
        }
    }
}
