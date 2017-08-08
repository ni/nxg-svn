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
            if (_svnManager.IsWorkingCopy(@"C:\svnTesting\Adder")) //TODO: Fill in with working path...obtain from system project root?
            {
                _svnManager.LoadCurrentSvnItemsInLocalRepository(@"C:\svnTesting\Adder"); //TODO: Fill in with working path
                //TODO: Update UI with latest status of items in SVN
            }            
        }

        //TODO: listen to add file, update cache

        /// <summary>
        /// Lock the file
        /// </summary>
        /// <param name="filename">file to lock</param>
        /// <param name="comment">optional lock comment</param>
        /// <returns></returns>
        public bool Lock(string filename, string comment = "")
        {
            return _svnManager.Lock(filename, comment);
        }
    }
}
