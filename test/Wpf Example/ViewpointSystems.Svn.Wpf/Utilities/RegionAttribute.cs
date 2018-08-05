using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svn.Wpf.Utilities
{
    public class RegionAttribute : Attribute
    {
        public Region Region { get; private set; }

        public RegionAttribute(Region region)
        {
            Region = region;
        }
    }
}
