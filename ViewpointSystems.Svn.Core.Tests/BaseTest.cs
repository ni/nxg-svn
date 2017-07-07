// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the BaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Moq;
using MvvmCross.Core.Platform;
using MvvmCross.Core.Views;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Plugins.Network.Reachability;
using MvvmCross.Test.Core;

using Ploeh.AutoFixture;

namespace ViewpointSystems.Svn.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;
    using Mocks;

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

        public static Fixture Fixture;

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
            var messenger = new MvxMessengerHub();
            Ioc.RegisterSingleton<IMvxMessenger>(messenger);

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
            this.Terminate();
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

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Clear();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
