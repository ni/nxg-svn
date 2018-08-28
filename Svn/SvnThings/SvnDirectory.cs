using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Svn.Cache;
using Svn.SccThings;

namespace Svn.SvnThings
{
    public interface ISvnDirectoryUpdate
    {
        void TickAll();

        void Store(SvnItem item);

        bool ScheduleForCleanup { get; }

        void SetNeedsUpgrade();
        void SetNeedsCleanup();
    }
    /// <summary>
    /// Collection of <see cref="SvnItem"/> instances of a specific directory
    /// </summary>
    /// <remarks>
    /// <para>A SvnDirectory contains the directory itself and all files and directories contained directly within</para>
    /// 
    /// <para>Note: This tells us that all subdirectories are contained in the parent and in their own <see cref="SvnDirectory"/></para>
    /// </remarks>
    public sealed class SvnDirectory : SccDirectory<SvnItem>, ISvnDirectoryUpdate
    {
        readonly ISvnStatusCache _context;
        bool _needsUpgrade;
        bool _needsCleanup;

        public SvnDirectory(string fullPath)
            : base(fullPath)
        {

        }

        /// <summary>
        /// Gets the directory item
        /// </summary>
        /// <value>The directory.</value>
        public SvnItem Directory
        {
            [DebuggerStepThrough]
            get
            {
                if (Contains(FullPath))
                    return this[FullPath]; // 99.9% case
                else
                {
                    // Get the item from the status cache
                    var cache = _context;

                    if (cache == null)
                        return null;

                    var item = cache[FullPath];

                    if (item != null)
                    {
                        if (!Contains(FullPath))
                            Add(item); // In most cases the file is added by the cache
                    }

                    return item;
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this directories working copy needs an explicit upgrade
        /// </summary>
        public bool NeedsWorkingCopyUpgrade
        {
            get { return _needsUpgrade; }
        }

        /// <summary>
        /// Tick all items
        /// </summary>
        void ISvnDirectoryUpdate.TickAll()
        {
            _needsUpgrade = false;
            _needsCleanup = false;
            foreach (ISvnItemUpdate item in this)
            {
                item.TickItem();
            }
        }

        void ISvnDirectoryUpdate.Store(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Remove(item.FullPath);
            Add(item);
        }

        bool ISvnDirectoryUpdate.ScheduleForCleanup
        {
            get
            {
                foreach (var item in this)
                {
                    if (((ISvnItemUpdate)item).IsItemTicked())
                        return true;
                }

                return false;
            }
        }

        void ISvnDirectoryUpdate.SetNeedsUpgrade()
        {
            _needsUpgrade = true;
        }

        void ISvnDirectoryUpdate.SetNeedsCleanup()
        {
            _needsUpgrade = true;
        }

        public bool NeedsCleanup
        {
            get { return _needsCleanup; }
        }
    }
}
