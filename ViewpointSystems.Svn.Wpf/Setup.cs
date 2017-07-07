// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the Setup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using MvvmCross.BindingEx.Wpf;
using MvvmCross.Platform;
using MvvmCross.Platform.Converters;
using MvvmCross.Platform.Platform;
using ViewpointSystems.Svn.SvnThings;
using ViewpointSystems.Svn.Wpf.Properties;

namespace ViewpointSystems.Svn.Wpf
{
    using MvvmCross.Core.ViewModels;
    using MvvmCross.Wpf.Platform;
    using MvvmCross.Wpf.Views;
    using System.Windows.Threading;
    using ViewpointSystems.Svn.Core.Entities;

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

            Mvx.RegisterSingleton<SvnManagement>(new SvnManagement());
            

            return new Core.App();
        }

        protected void RegisterBindingbuilderCallbacks()
        {
            Mvx.CallbackWhenRegistered<IMvxValueConverterRegistry>(this.FillValueConverters);
        }

        protected void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            //registry.AddOrOverwrite("ProgressStyleConverter", new ProgressStyleConverter());
            //registry.AddOrOverwrite("BatteryChargeConverter", new BatteryChargeConverter());
            //registry.AddOrOverwrite("NetworkStatusConverter", new NetworkStatusConverter());
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            var builder = new MvxWindowsBindingBuilder();
            this.RegisterBindingbuilderCallbacks();
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
