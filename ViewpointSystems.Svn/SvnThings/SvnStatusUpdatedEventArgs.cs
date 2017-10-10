using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewpointSystems.Svn.SvnThings
{
    public class SvnStatusUpdatedEventArgs : EventArgs
    {
        public string FullFilePath { get; set; }

        public SvnStatusUpdatedEventArgs(string fullFilePath)
        {
            FullFilePath = fullFilePath;
        }
    }
}
