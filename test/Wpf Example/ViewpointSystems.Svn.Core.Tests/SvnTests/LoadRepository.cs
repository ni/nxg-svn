using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Svn.SvnThings;

namespace Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class LoadRepository : BaseTest
    {

        [TestMethod]
        public void LoadRepository_WorkingCopy_IsValid()
        {
            // Arrange
            SvnManager.BuildUnitTestRepo(MainViewModel.LocalWorkingLocation, UnitTestFolder);
            // Act
            var isWorkingCopy = SvnManager.IsWorkingCopy(SvnManager.GetRoot(UnitTestPath));

            // Assert
            isWorkingCopy.Should().BeTrue();
        }

        [TestMethod]
        public void LoadRepository_Load_IsValid()
        {
            // Arrange
            var rootPath = SvnManager.GetRoot(UnitTestPath);
            // Act
            var isWorkingCopy = SvnManager.IsWorkingCopy(rootPath);
            if (isWorkingCopy)
            {
                SvnManager.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappedItems = SvnManager.GetMappings();

            // Assert
            isWorkingCopy.Should().BeTrue();
            mappedItems.Count.Should().BeGreaterThan(0);

        }
    }
}
