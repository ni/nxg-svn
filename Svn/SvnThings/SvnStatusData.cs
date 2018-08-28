using SharpSvn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn.SvnThings
{
    public sealed class SvnStatusData
    {
        readonly bool _conflicted;
        readonly SvnNodeKind _nodeKind;
        readonly string _changeList;
        readonly SvnStatus _localNodeStatus;
        readonly SvnStatus _localTextStatus;
        readonly SvnStatus _localPropertyStatus;
        readonly bool _localCopied;
        readonly bool _localLocked;
        readonly Uri _uri;

        readonly DateTime _lastChangeTime;
        readonly string _lastChangeAuthor;
        readonly long _lastChangeRevision;
        readonly long _revision;
        readonly bool _localFileExists;
        readonly bool _movedHere;
        readonly bool _movedAway;

        public SvnStatusData(SvnStatusEventArgs status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            _nodeKind = status.NodeKind;
            _localNodeStatus = status.LocalNodeStatus;
            _localTextStatus = status.LocalTextStatus;
            _localPropertyStatus = status.LocalPropertyStatus;
            _localCopied = status.LocalCopied;
            _uri = status.Uri;
            _localFileExists = (status.FileLength >= 0);

            if (status.Versioned)
            {
                _lastChangeTime = status.LastChangeTime;
                _lastChangeRevision = status.LastChangeRevision;
                _lastChangeAuthor = status.LastChangeAuthor;
                _revision = status.Revision;
                _changeList = status.ChangeList;
                _localLocked = status.LocalLock != null;
            }

            _conflicted = status.Conflicted;
            _movedHere = (status.MovedFrom != null);
            _movedAway = (status.MovedTo != null);
        }

        /// <summary>
        /// Create non-locked, non-copied item with status specified
        /// </summary>
        /// <param name="allStatuses"></param>
        private SvnStatusData(SvnStatus allStatuses)
        {
            _localNodeStatus = allStatuses;
            _localTextStatus = SvnStatus.None;
            _localPropertyStatus = SvnStatus.None;
            //_localLocked = false;
            //_localCopied = false;
        }

        #region Static instances
        readonly static SvnStatusData _unversioned = new SvnStatusData(SvnStatus.NotVersioned);
        readonly static SvnStatusData _normal = new SvnStatusData(SvnStatus.Normal);
        readonly static SvnStatusData _none = new SvnStatusData(SvnStatus.None);
        /// <summary>
        /// Default status for nodes which do exist but are not managed
        /// </summary>
        internal static SvnStatusData NotVersioned
        {
            get { return _unversioned; }
        }

        /// <summary>
        /// Default status for nodes which exist and are managed
        /// </summary>
        internal static SvnStatusData Normal
        {
            get { return _normal; }
        }

        /// <summary>
        /// Default status for nodes which don't exist and are not managed
        /// </summary>
        internal static SvnStatusData NotExisting
        {
            get { return _none; }
        }
        #endregion

        /// <summary>
        /// Content status in working copy
        /// </summary>
        public SvnStatus LocalNodeStatus
        {
            get { return _localNodeStatus; }
        }

        /// <summary>
        /// Content status in working copy
        /// </summary>
        public SvnStatus LocalTextStatus
        {
            get { return _localTextStatus; }
        }

        /// <summary>
        /// Property status in working copy
        /// </summary>
        public SvnStatus LocalPropertyStatus
        {
            get { return _localPropertyStatus; }
        }

        public SvnStatus CombinedStatus
        {
            get
            {
                switch (_localNodeStatus)
                {
                    // High priority statuses on the content
                    case SvnStatus.Obstructed:
                    case SvnStatus.Missing:
                    case SvnStatus.Incomplete:
                        return _localNodeStatus;
                }

                switch (_localTextStatus)
                {
                    // High priority on the text
                    case SvnStatus.Conflicted:
                        return _localPropertyStatus;
                }

                switch (_localPropertyStatus)
                {
                    // High priority on the properties
                    case SvnStatus.Conflicted:
                        return _localPropertyStatus;
                }

                if (_localNodeStatus != SvnStatus.Normal)
                    return _localNodeStatus;
                else if (_localTextStatus != SvnStatus.None && _localTextStatus != SvnStatus.Normal)
                    return _localTextStatus;
                else if (_localPropertyStatus != SvnStatus.None && _localPropertyStatus != SvnStatus.Normal)
                    return _localPropertyStatus;
                else
                    return _localTextStatus;
            }
        }

        /// <summary>
        /// Gets the change list in which the file is placed
        /// </summary>
        /// <value>The change list.</value>
        /// <remarks>The changelist value is only valid if the file is modified</remarks>
        public string ChangeList
        {
            get { return _changeList; }
        }

        public SvnNodeKind NodeKind
        {
            get { return _nodeKind; }
        }

        public DateTime LastChangeTime
        {
            get { return _lastChangeTime; }
        }

        public string LastChangeAuthor
        {
            get { return _lastChangeAuthor; }
        }

        public long LastChangeRevision
        {
            get { return _lastChangeRevision; }
        }

        public long Revision
        {
            get { return _revision; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the file is copied in the working copy
        /// </summary>
        public bool IsCopied
        {
            get { return _localCopied; }
        }

        public bool IsMoved
        {
            get { return _movedHere; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the workingcopy is locked in the local working copy
        /// </summary>
        public bool IsLockedLocal
        {
            get { return _localLocked; }
        }

        internal bool LocalFileExists
        {
            get { return _localFileExists; }
        }

        internal Uri Uri
        {
            get { return _uri; }
        }

        internal bool Conflicted
        {
            get { return _conflicted; }
        }
    }
}
