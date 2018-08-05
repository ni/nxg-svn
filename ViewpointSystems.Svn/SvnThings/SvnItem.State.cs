using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSvn;

namespace Svn.SvnThings
{
    public interface ISvnItemStateUpdate
    {
        IList<SvnItem> GetUpdateQueueAndClearScheduled();

        void SetDocumentDirty(bool value);
        void SetSolutionContained(bool inSolution, bool sccExcluded);
    }

    partial class SvnItem : ISvnItemStateUpdate
    {
        SvnItemState _currentState;
        SvnItemState _validState;
        SvnItemState _onceValid;

        const SvnItemState _maskRefreshTo = SvnItemState.Versioned | SvnItemState.HasLockToken | SvnItemState.Obstructed | SvnItemState.Modified | SvnItemState.PropertyModified | SvnItemState.Added | SvnItemState.HasCopyOrigin
            | SvnItemState.Deleted | SvnItemState.Replaced | SvnItemState.HasProperties | SvnItemState.ContentConflicted | SvnItemState.PropertyModified | SvnItemState.SvnDirty | SvnItemState.Ignored | SvnItemState.Conflicted
            | SvnItemState.MovedHere;

        public SvnItemState GetState(SvnItemState flagsToGet)
        {
            var unavailable = flagsToGet & ~_validState;

            if (unavailable == 0)
                return _currentState & flagsToGet; // We have everything we need

            if (0 != (unavailable & _maskRefreshTo))
            {
                Debug.Assert(_statusDirty != XBool.False);
                RefreshStatus();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskRefreshTo) == 0, "RefreshMe() set all attributes it should");
            }

            if (0 != (unavailable & _maskGetAttributes))
            {
                UpdateAttributeInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskGetAttributes) == 0, "UpdateAttributeInfo() set all attributes it should");
            }

            if (0 != (unavailable & _maskUpdateSolution))
            {
                UpdateSolutionInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskUpdateSolution) == 0, "UpdateSolution() set all attributes it should");
            }

            if (0 != (unavailable & _maskDocumentInfo))
            {
                UpdateDocumentInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskDocumentInfo) == 0, "UpdateDocumentInfo() set all attributes it should");
            }

            if (0 != (unavailable & _maskVersionable))
            {
                UpdateVersionable();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskVersionable) == 0, "UpdateVersionable() set all attributes it should");
            }

            if (0 != (unavailable & _maskMustLock))
            {
                UpdateMustLock();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskMustLock) == 0, "UpdateMustLock() set all attributes it should");
            }

            if (0 != (unavailable & _maskTextFile))
            {
                UpdateTextFile();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskTextFile) == 0, "UpdateTextFile() set all attributes it should");
            }

            if (0 != (unavailable & _maskWCRoot))
            {
                UpdateWCRoot();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskWCRoot) == 0, "UpdateWCRoot() set all attributes it should");
            }

            if (0 != (unavailable & _maskIsAdministrativeArea))
            {
                UpdateAdministrativeArea();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & _maskIsAdministrativeArea) == 0, "UpdateIsAdministrativeArea() set all attributes it should");
            }

            if (unavailable != 0)
            {
                Trace.WriteLine(string.Format("Don't know how to retrieve {0:X} state; clearing dirty flag", (int)unavailable));

                _validState |= unavailable;
            }

            return _currentState & flagsToGet;
        }

        IList<SvnItem> ISvnItemStateUpdate.GetUpdateQueueAndClearScheduled()
        {
            lock (_stateChanged)
            {
                _scheduled = false;

                if (_stateChanged.Count == 0)
                    return null;

                var modified = new List<SvnItem>(_stateChanged.Count);
                modified.AddRange(_stateChanged);
                _stateChanged.Clear();

                foreach (var i in modified)
                    i._enqueued = false;


                return modified;
            }
        }

        private void SetDirty(SvnItemState dirty)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            _validState &= ~dirty;
        }

        // Mask of states not to broadcast for
        const SvnItemState _noBroadcastFor = ~(SvnItemState.DocumentDirty | SvnItemState.InSolution);

        void SetState(SvnItemState set, SvnItemState unset)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            var st = (_currentState & ~unset) | set;

            if (st != _currentState)
            {
                // Calculate whether we have a change or just new information
                var changed = (st & _onceValid & _noBroadcastFor) != (_currentState & _onceValid & _noBroadcastFor);

                if (changed && !_enqueued)
                {
                    _enqueued = true;

                    // Schedule a stat changed broadcast
                    lock (_stateChanged)
                    {
                        _stateChanged.Enqueue(this);

                        ScheduleUpdateNotify();
                    }
                }

                _currentState = st;

            }
            _validState |= (set | unset);
            _onceValid |= _validState;
        }

        void ISvnItemUpdate.SetState(SvnItemState set, SvnItemState unset)
        {
            SetState(set, unset);
        }
        void ISvnItemUpdate.SetDirty(SvnItemState dirty)
        {
            SetDirty(dirty);
        }
        bool ISvnItemUpdate.TryGetState(SvnItemState get, out SvnItemState value)
        {
            return TryGetState(get, out value);
        }

        #region Versionable

        const SvnItemState _maskVersionable = SvnItemState.Versionable;

        void UpdateVersionable()
        {
            bool versionable;

            SvnItemState state;

            if (TryGetState(SvnItemState.Versioned, out state) && state != 0)
                versionable = true;
            else if (Exists && SvnTools.IsBelowManagedPath(FullPath)) // Will call GetState again!
                versionable = true;
            else
                versionable = false;

            if (versionable)
                SetState(SvnItemState.Versionable, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.Versionable);
        }

        #endregion

        #region DocumentInfo

        const SvnItemState _maskDocumentInfo = SvnItemState.DocumentDirty;

        void UpdateDocumentInfo()
        {
            //IAnkhOpenDocumentTracker dt = _context.GetService<IAnkhOpenDocumentTracker>();

            //if (dt == null)
            //{
            //    // We /must/ make the state not dirty
            //    SetState(SvnItemState.None, SvnItemState.DocumentDirty);
            //    return;
            //}

            //if (dt.IsDocumentDirty(FullPath, true))
            //    SetState(SvnItemState.DocumentDirty, SvnItemState.None);
            //else
            //    SetState(SvnItemState.None, SvnItemState.DocumentDirty);
        }

        #endregion

        #region Solution Info
        const SvnItemState _maskUpdateSolution = SvnItemState.InSolution;
        void UpdateSolutionInfo()
        {
            //IProjectFileMapper pfm = _context.GetService<IProjectFileMapper>();
            //bool inSolution = false;

            //if (pfm != null)
            //{
            //    inSolution = pfm.ContainsPath(FullPath);
            //    _sccExcluded = pfm.IsSccExcluded(FullPath);
            //}

            //if (inSolution)
            //    SetState(SvnItemState.InSolution, SvnItemState.None);
            //else
            //    SetState(SvnItemState.None, SvnItemState.InSolution);
        }

        void ISvnItemStateUpdate.SetSolutionContained(bool inSolution, bool sccExcluded)
        {
            if (inSolution)
                SetState(SvnItemState.InSolution, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.InSolution);

            _sccExcluded = sccExcluded;
        }

        #endregion

        #region Must Lock
        const SvnItemState _maskMustLock = SvnItemState.MustLock;
        void UpdateMustLock()
        {
            //SvnItemState fastValue = SvnItemState.IsDiskFile | SvnItemState.ReadOnly;
            //SvnItemState slowValue = SvnItemState.Versioned;
            //SvnItemState v;

            //bool mustLock;

            //if (TryGetState(SvnItemState.Versioned, out v) && (v == 0))
            //    mustLock = false;
            //else if (TryGetState(SvnItemState.HasProperties, out v) && (v == 0))
            //    mustLock = false;
            //else if (TryGetState(SvnItemState.ReadOnly, out v) && (v == 0))
            //    mustLock = false;
            //else if (GetState(fastValue) != fastValue)
            //    mustLock = false;
            //else if (GetState(slowValue) != slowValue)
            //    mustLock = false;
            //else
            //{
            //    using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
            //    {
            //        string propVal;

            //        if (client.TryGetProperty(FullPath, SvnPropertyNames.SvnNeedsLock, out propVal))
            //        {
            //            mustLock = propVal != null; // Value should be equal to SvnPropertyNames.SvnBooleanValue
            //        }
            //        else
            //            mustLock = false;
            //    }
            //}

            //if (mustLock)
            //    SetState(SvnItemState.MustLock, SvnItemState.None);
            //else
            //    SetState(SvnItemState.None, SvnItemState.MustLock);
        }
        #endregion

        #region TextFile File
        const SvnItemState _maskTextFile = SvnItemState.IsTextFile;
        void UpdateTextFile()
        {
            //SvnItemState value = SvnItemState.IsDiskFile | SvnItemState.Versioned;
            //SvnItemState v;

            //bool isTextFile;

            //if (TryGetState(SvnItemState.Versioned, out v) && (v == 0))
            //    isTextFile = false;
            //else if (GetState(value) != value)
            //    isTextFile = false;
            //else
            //{
            //    using (SvnWorkingCopyClient client = _context.GetService<ISvnClientPool>().GetWcClient())
            //    {
            //        SvnWorkingCopyStateArgs a = new SvnWorkingCopyStateArgs();
            //        a.ThrowOnError = false;
            //        a.RetrieveFileData = true;
            //        SvnWorkingCopyState state;
            //        if (client.GetState(FullPath, out state))
            //        {
            //            isTextFile = state.IsTextFile;
            //        }
            //        else
            //            isTextFile = false;
            //    }
            //}

            //if (isTextFile)
            //    SetState(SvnItemState.IsTextFile, SvnItemState.None);
            //else
            //    SetState(SvnItemState.None, SvnItemState.IsTextFile);
        }
        #endregion

        #region Attribute Info
        const SvnItemState _maskGetAttributes = SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.IsDiskFile | SvnItemState.IsDiskFolder;

        void UpdateAttributeInfo()
        {
            // One call of the kernel's GetFileAttributesW() gives us most info we need

            var value = NativeMethods.GetFileAttributes(FullPath);

            if (value == NativeMethods.INVALID_FILE_ATTRIBUTES)
            {
                // File does not exist / no rights, etc.

                SetState(SvnItemState.None,
                    SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.Versionable | SvnItemState.IsDiskFolder | SvnItemState.IsDiskFile);

                return;
            }

            var set = SvnItemState.Exists;
            var unset = SvnItemState.None;

            if ((value & NativeMethods.FILE_ATTRIBUTE_READONLY) != 0)
                set |= SvnItemState.ReadOnly;
            else
                unset = SvnItemState.ReadOnly;

            if ((value & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
            {
                unset |= SvnItemState.IsDiskFile | SvnItemState.ReadOnly;
                set = SvnItemState.IsDiskFolder | (set & ~SvnItemState.ReadOnly); // Don't set readonly
            }
            else
            {
                set |= SvnItemState.IsDiskFile;
                unset |= SvnItemState.IsDiskFolder;
            }

            SetState(set, unset);
        }
        #endregion

        #region Nested Info
        const SvnItemState _maskWCRoot = SvnItemState.IsWCRoot;
        public void UpdateWCRoot()
        {
            if (IsDirectory && IsVersioned)
            {
                StatusCache.RefreshWCRoot(this);
                return;
            }

            SetState(SvnItemState.None, SvnItemState.IsWCRoot);
        }
        #endregion

        #region Administrative Area
        const SvnItemState _maskIsAdministrativeArea = SvnItemState.IsAdministrativeArea;
        void UpdateAdministrativeArea()
        {
            if (string.Equals(Name, SvnClient.AdministrativeDirectoryName, StringComparison.OrdinalIgnoreCase))
                SetState(SvnItemState.IsAdministrativeArea, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.IsAdministrativeArea);
        }
        #endregion

        #region ISvnItemStateUpdate Members

        void ISvnItemStateUpdate.SetDocumentDirty(bool value)
        {
            if (value)
                SetState(SvnItemState.DocumentDirty, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.DocumentDirty);
        }

        #endregion
    }
}
