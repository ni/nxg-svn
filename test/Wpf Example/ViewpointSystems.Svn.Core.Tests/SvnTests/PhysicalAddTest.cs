using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using System.Threading;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class PhysicalAddTest : BaseTest
    {
        [TestMethod]
        public void StatusCache_PhysicalAdd_IsValid()
        {
            // Arrange
            var rootPath = SvnManagement.GetRoot(UnitTestPath);
            // Act

            if (SvnManagement.IsWorkingCopy(rootPath))
            {
                SvnManagement.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappingsBefore = SvnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            var physicalAddFile = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_PhysicalAdd.txt"));

            physicalAddFile.Create();
            Thread.Sleep(2000);
            var mappingsAfter = SvnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (physicalAddFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }                   
            }
        }
    }
}
