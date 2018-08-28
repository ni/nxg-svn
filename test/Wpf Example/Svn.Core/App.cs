using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using Svn.Core.ViewModels;
using Svn.SvnThings;

namespace Svn.Core
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
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterSingleton<SvnManager>(new SvnManager());
            //// Start the app with the Main View Model.
            RegisterAppStart<MainViewModel>();
        }
    }
}

