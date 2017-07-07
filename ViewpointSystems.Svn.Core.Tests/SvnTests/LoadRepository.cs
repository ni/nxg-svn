using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class LoadRepository : BaseTest
    {

        [TestMethod]
        public void LoadRepository_WorkingCopy_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";

            // Act
            var IsWorkingCopy = svnManagement.IsWorkingCopy(localWorkingLocation);

            // Assert
            IsWorkingCopy.Should().BeTrue();
        }

        [TestMethod]
        public void LoadRepository_Load_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";

            // Act
            var IsWorkingCopy = svnManagement.IsWorkingCopy(localWorkingLocation);
            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappedItems = svnManagement.GetMappings();

            // Assert
            IsWorkingCopy.Should().BeTrue();
            mappedItems.Count.Should().BeGreaterThan(1);

        }
    }
}
