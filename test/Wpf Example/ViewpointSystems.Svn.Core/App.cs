using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using ViewpointSystems.Svn.Core.ViewModels;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core
{
    using MvvmCross.Core.ViewModels;
    using MvvmCross.Platform.IoC;

    /// <summary>
    /// Define the App type.
    /// </summary>
    public class App : MvxApplication
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            this.CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterSingleton<SvnManagement>(new SvnManagement());
            //// Start the app with the Main View Model.
            this.RegisterAppStart<MainViewModel>();
        }
    }
}

