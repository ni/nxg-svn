using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Composition;
using NationalInstruments.Core;

namespace ViewpointSystems.Svn.Plugin
{
    [System.ComponentModel.Composition.Export(typeof(SvnManagerPlugin))]
    [PartMetadata(ExportIdentifier.RootContainerKey, "")]
    public class SvnManagerPlugin
    {
        private ViewpointSystems.Svn.Svn.SvnManagement _svnManager;

        public SvnManagerPlugin()
        {
            
            
        }
    }
}
