using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments;
using NationalInstruments.Core;
using NationalInstruments.ProjectExplorer;
using NationalInstruments.SourceModel.Envoys;
using ViewpointSystems.Svn.Plugin.Properties;
using NationalInstruments.Composition;

namespace ViewpointSystems.Svn.Plugin.Overlay
{
    public class ProjectItemOverlayService : EnvoyService, IProjectItemOverlaySupport
    {
        private readonly PlatformImage _lockOverlay;
        private readonly PlatformImage _redOverlay;
        
        
        private readonly SvnManagerPlugin _svnManager;
        private readonly ICompositionHost _host;

        public ProjectItemOverlayService(ICompositionHost host)
        {
            _host = host;
            _lockOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/LockControls.png");
            //_redOverlay = ResourceHelpers.LoadBitmapImage(typeof(ProjectItemOverlayService), "Resources/Red_8x8.png");

            //TODO: proper method to get reference to SVN manager?
            _svnManager = _host.GetSharedExportedValue<SvnManagerPlugin>();
        }

        public PlatformImage TopLeftOverlay => PlatformImage.NullImage;

        //TODO: fix location to another corner once bug fix from NI
        public PlatformImage BottomLeftOverlay
        {
            get
            {
                var returnValue = PlatformImage.NullImage;
                var fileService = AssociatedEnvoy.GetReferencedFileService();               
                if (null != fileService && fileService.HasSetLocation())
                {
                    //TODO: evaluate other icons                    
                    var status = _svnManager.Status(fileService.StoragePath);
                    if (status.IsLocked)
                        returnValue = _lockOverlay;                    
                }
                return returnValue;
            }
        }

        public PlatformImage TopRightOverlay => PlatformImage.NullImage;

        public PlatformImage BottomRightOverlay => PlatformImage.NullImage;
    }
}
