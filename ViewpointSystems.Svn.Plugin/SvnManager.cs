using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Composition;
using NationalInstruments.Core;

namespace ViewpointSystems.Svn
{
    [System.ComponentModel.Composition.Export(typeof(SvnManager))]
    [PartMetadata(ExportIdentifier.RootContainerKey, "")]
    public class SvnManager
    {
        public DateTime TheValue;

        public SvnManager()
        {
            
            TheValue = DateTime.Now;
        }
    }
}
