using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewpointSystems.Svn.Core.ViewModels;

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

            //// Start the app with the Main View Model.
            this.RegisterAppStart<MainViewModel>();
        }
    }
}
