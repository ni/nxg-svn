using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvvmCross.Core.Platform;
using MvvmCross.Core.Views;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.Platform;
using MvvmCross.Test.Core;
using ViewpointSystems.Svn.Core.Tests.Mocks;
using ViewpointSystems.Svn.Core.ViewModels;
using ViewpointSystems.Svn.SvnThings;


namespace ViewpointSystems.Svn.Core.Tests
{   
    /// <summary>
    /// Defines the BaseTest type.
    /// </summary>
    [TestClass]
    public abstract class BaseTest : MvxIoCSupportingTest
    {
        /// <summary>
        /// The mock dispatcher.
        /// </summary>
        public MockDispatcher MockDispatcher;  
        
        public SvnManager SvnManager;
        
        public string UnitTestFolder = @"UnitTestRepo\";
        public string UnitTestPath = @"";

        public MainViewModel MainViewModel;

        /// <summary>
        /// Sets up.
        /// </summary>
        [TestInitialize]
        public virtual void SetUp()
        {
            ClearAll();
            MockDispatcher = new MockDispatcher();

            Ioc.RegisterSingleton<IMvxViewDispatcher>(MockDispatcher);
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(MockDispatcher);
            Ioc.RegisterSingleton<IMvxStringToTypeParser>(new MvxStringToTypeParser());
            //var messenger = new MvxMessengerHub();
            //Ioc.RegisterSingleton<IMvxMessenger>(messenger);
            SvnManager = new SvnManager();
            Ioc.RegisterSingleton<SvnManager>(SvnManager);
            MainViewModel = new MainViewModel();
            UnitTestPath = Path.Combine(MainViewModel.LocalWorkingLocation, UnitTestFolder);
            Ioc.RegisterSingleton<IMvxTrace>(new TestTrace());
            Ioc.RegisterSingleton<IMvxSettings>(new MvxSettings());
            Initialize();
            CreateTestableObject();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TestCleanup]
        public virtual void TearDown()
        {
            Terminate();
        }

        /// <summary>
        /// Creates the testable object.
        /// </summary>
        public virtual void CreateTestableObject()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// Any specific setup code for derived classes should override this method.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Terminates this instance.
        /// Any specific termination code for derived classes should override this method.
        /// </summary>
        protected virtual void Terminate()
        {
        }

        
    }
}
