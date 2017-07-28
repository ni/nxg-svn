using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Composition;
using NationalInstruments.Core;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Plugin
{
    [System.ComponentModel.Composition.Export(typeof(SvnManagerPlugin))]
    [PartMetadata(ExportIdentifier.RootContainerKey, "")]
    public class SvnManagerPlugin
    {
        private SvnManager _svnManager;

        public SvnManagerPlugin()
        {
            _svnManager = new SvnManager();
            if (_svnManager.IsWorkingCopy("")) //TODO: Fill in with working path
            {
                _svnManager.LoadCurrentSvnItemsInLocalRepository(""); //TODO: Fill in with working path
                //TODO: Update UI with latest status of items in SVN
            }
            
        }

        //TODO: listen to add file, update cache
    }
}
