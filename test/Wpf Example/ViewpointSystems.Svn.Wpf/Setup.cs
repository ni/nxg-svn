using System;
using System.IO;
using MvvmCross.Binding.Wpf;
using MvvmCross.Platform;
using MvvmCross.Platform.Converters;
using MvvmCross.Platform.Platform;
using ViewpointSystems.Svn.Wpf.Properties;
using MvvmCross.Core.ViewModels;
using MvvmCross.Wpf.Platform;
using MvvmCross.Wpf.Views;
using System.Windows.Threading;
using ViewpointSystems.Svn.Core.Entities;

namespace ViewpointSystems.Svn.Wpf
{    
    /// <summary>
    ///  Defines the Setup type.
    /// </summary>
    public class Setup : MvxWpfSetup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Setup"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="presenter">The presenter.</param>
        public Setup(Dispatcher dispatcher, IMvxWpfViewPresenter presenter)
            : base(dispatcher, presenter)
        {
        }

        /// <summary>
        /// Creates the app.
        /// </summary>
        /// <returns>An instance of MvxApplication</returns>
        protected override IMvxApplication CreateApp()
        {
            var appSetting = new AppSettings() { Name = Settings.Default.Name, Version = Settings.Default.Version };
            Mvx.RegisterSingleton<AppSettings>(appSetting);
            
            return new Core.App();
        }

        protected void RegisterBindingbuilderCallbacks()
        {
            Mvx.CallbackWhenRegistered<IMvxValueConverterRegistry>(FillValueConverters);
        }

        protected void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            var builder = new MvxWindowsBindingBuilder();
            RegisterBindingbuilderCallbacks();
            builder.DoRegistration();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            //#if DEBUG
            return base.CreateDebugTrace();
            //#endif
            //  return new ProductionTrace();
        }

    }
}
