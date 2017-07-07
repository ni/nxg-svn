using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class CheckoutTest : BaseTest
    {

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void SvnManagement_Checkout_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";
            bool assertVal = false;

            // Act
            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                if (svnManagement.CheckOut(localWorkingLocation))
                {
                    svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
                    assertVal = true;
                }
            }

            // Assert
            assertVal.Should().BeTrue();
        }
    }
}
