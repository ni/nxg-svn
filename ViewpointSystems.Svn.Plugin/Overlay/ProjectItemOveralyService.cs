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
using NationalInstruments.Shell;
using NationalInstruments.ProjectExplorer.Design;

namespace ViewpointSystems.Svn.Plugin.Overlay
{
    public class ProjectItemOverlayService : EnvoyService, IProjectItemOverlaySupport
    {
        private readonly PlatformImage _lockOverlay;
        private readonly PlatformImage _modifiedOverlay;
        private readonly PlatformImage _unmodifiedOverlay;
        private readonly PlatformImage _lockedModifiedOverlay;
        private readonly PlatformImage _lockedUnmodifiedOverlay;
        private readonly PlatformImage _addedOverlay;

        private readonly SvnManagerPlugin _svnManager;
        private readonly ICompositionHost _host;

        public ProjectItemOverlayService(ICompositionHost host)
        {
            _host = host;
            _lockOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/LockControls.png");
            _modifiedOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/Modified.png");
            _unmodifiedOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/Unmodified.png");

            _lockedModifiedOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/LockedModified.png");
            _lockedUnmodifiedOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/LockedUnmodified.png");

            _addedOverlay = ResourceHelpers.LoadBitmapImage(typeof(SvnManagerPlugin), "Resources/Added.png");
            _svnManager = _host.GetSharedExportedValue<SvnManagerPlugin>();
        }


        public PlatformImage TopLeftOverlay => PlatformImage.NullImage;


        public PlatformImage TopRightOverlay //not implemented in current public build; use BottomLeftOverlay for temp workaround
        {
            get
            {
                var returnValue = PlatformImage.NullImage;
                var fileService = AssociatedEnvoy.GetReferencedFileService();               
                if (null != fileService && fileService.HasSetLocation())
                {
                    //TODO: evaluate other icons                    
                    var status = _svnManager.Status(fileService.StoragePath);
                    if (status.IsVersioned)
                    {
                        if (status.IsLocked)
                        {
                            if (status.IsModified)
                                returnValue = _lockedModifiedOverlay;
                            else
                            {
                                returnValue = _lockedUnmodifiedOverlay;
                            }
                        }
                        else
                        {
                            if (status.IsAdded)
                                returnValue = _addedOverlay;
                            else if (status.IsModified)
                                returnValue = _modifiedOverlay;
                            else
                            {
                                returnValue = _unmodifiedOverlay;
                            }
                        }
                    }                    
                }
                return returnValue;
            }
        }

        public PlatformImage BottomRightOverlay=> PlatformImage.NullImage;

        public PlatformImage BottomLeftOverlay => PlatformImage.NullImage;
        //this is the only implemented overlay in the current public build. Ultimately this won't really be accessible.
        //public PlatformImage BottomLeftOverlay // => PlatformImage.NullImage;
        //{
        //    get
        //    {
        //        var returnValue = PlatformImage.NullImage;
        //        var fileService = AssociatedEnvoy.GetReferencedFileService();               
        //        if (null != fileService && fileService.HasSetLocation())
        //        {
        //            //TODO: evaluate other icons                    
        //            var status = _svnManager.Status(fileService.StoragePath);
        //            if (status.IsVersioned)
        //            {
        //                if (status.IsLocked)
        //                {
        //                    if (status.IsModified)
        //                        returnValue = _lockedModifiedOverlay;
        //                    else
        //                    {
        //                        returnValue = _lockedUnmodifiedOverlay;
        //                    }
        //                }
        //                else
        //                {
        //                    if(status.IsModified)
        //                        returnValue = _modifiedOverlay;
        //                    else
        //                    {
        //                        returnValue = _unmodifiedOverlay;
        //                    }
        //                }
        //            }                        
        //        }
        //        return returnValue;
        //    }
        //}
    }
}
