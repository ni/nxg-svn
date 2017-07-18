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
            SvnManagement.BuildUnitTestRepo(MainViewModel.LocalWorkingLocation, UnitTestFolder);
            // Act
            var isWorkingCopy = SvnManagement.IsWorkingCopy(SvnManagement.GetRoot(UnitTestPath));

            // Assert
            isWorkingCopy.Should().BeTrue();
        }

        [TestMethod]
        public void LoadRepository_Load_IsValid()
        {
            // Arrange
            var rootPath = SvnManagement.GetRoot(UnitTestPath);
            // Act
            var isWorkingCopy = SvnManagement.IsWorkingCopy(rootPath);
            if (isWorkingCopy)
            {
                SvnManagement.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappedItems = SvnManagement.GetMappings();

            // Assert
            isWorkingCopy.Should().BeTrue();
            mappedItems.Count.Should().BeGreaterThan(0);

        }
    }
}
