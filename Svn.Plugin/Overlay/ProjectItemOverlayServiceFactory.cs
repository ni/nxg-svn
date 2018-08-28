using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ProjectExplorer;
using NationalInstruments.SourceModel.Envoys;

namespace Svn.Plugin.Overlay
{
    [ExportEnvoyServiceFactory(typeof(IProjectItemOverlaySupport))]
    public class ProjectItemOverlayServiceFactory : EnvoyServiceFactory
    {
        protected override EnvoyService CreateService()
        {
            return new ProjectItemOverlayService(Host);
        }
    }
}
