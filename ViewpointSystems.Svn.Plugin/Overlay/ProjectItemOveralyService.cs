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

namespace ViewpointSystems.Svn.Plugin.Overlay
{
    public class ProjectItemOverlayService : EnvoyService, IProjectItemOverlaySupport
    {
        private readonly PlatformImage _lockOverlay;
        private readonly PlatformImage _redOverlay;

        public ProjectItemOverlayService()
        {
            //TODO: Help - cannot seem to properly load icon
            _lockOverlay = ResourceHelpers.LoadBitmapImage(typeof(ProjectItemOverlayService), "Resources/LockControls.png");
            //_redOverlay = ResourceHelpers.LoadBitmapImage(typeof(ProjectItemOverlayService), "Resources/Red_8x8.png");
        }

        public PlatformImage TopLeftOverlay => PlatformImage.NullImage;

        //TODO: fix location to another corner once bug fix from NI
        public PlatformImage BottomLeftOverlay
        {
            get
            {
                IReferencedFileService fileService = AssociatedEnvoy.GetReferencedFileService();
                if (fileService == null)
                {
                    return PlatformImage.NullImage;
                }
                if (fileService.HasSetLocation())
                {
                    //TODO: evaluate what icon should be shown here
                    // fileService.StoragePath is the path to the file 
                    return _lockOverlay;
                }
                return PlatformImage.NullImage;
            }
        }

        public PlatformImage TopRightOverlay => PlatformImage.NullImage;

        public PlatformImage BottomRightOverlay => PlatformImage.NullImage;
    }
}
