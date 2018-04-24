using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class PhysicallyRenameFileTests : BaseTest
    {
        [TestMethod]
        public void StatusCache_PhysicallyRenameNormal_isValid()
        {
            // Arrange
            var rootPath = SvnManager.GetRoot(UnitTestPath);
            // Act

            if (SvnManager.IsWorkingCopy(rootPath))
            {
                SvnManager.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappingsBefore = SvnManager.GetMappings();
            int countBefore = mappingsBefore.Count;
            var physicalRenameFile = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_PhysicallyRenameNormal.txt"));
            var physicalRenameFileNew = new FileInfo(Path.Combine(UnitTestPath, (countBefore + 1) + "Renamed" + "_PhysicallyRenameNormalRenamed.txt"));

            var myFile = File.Create(physicalRenameFile.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(physicalRenameFile.ToString());

            Thread.Sleep(500);
            SvnManager.Commit(physicalRenameFile.ToString(), "Unit Test");
            Thread.Sleep(2000);

            System.IO.File.Move(physicalRenameFile.ToString(), physicalRenameFileNew.ToString());
            Thread.Sleep(2000);
            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (physicalRenameFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                    
                }
                else if (physicalRenameFileNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }
            }
        }

        [TestMethod]
        public void StatusCache_PhysicallyRenameAdded_isValid()
        {
            // Arrange
            var rootPath = SvnManager.GetRoot(UnitTestPath);
            // Act

            if (SvnManager.IsWorkingCopy(rootPath))
            {
                SvnManager.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappingsBefore = SvnManager.GetMappings();
            int countBefore = mappingsBefore.Count;
            var physicallyRenameAddedFile = new FileInfo(Path.Combine(UnitTestPath, countBefore + ".txt"));
            var physicallyRenameAddedFileNew = new FileInfo(Path.Combine(UnitTestPath, (countBefore + 1) + ".txt"));

            var myFile = File.Create(physicallyRenameAddedFile.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(physicallyRenameAddedFile.ToString());

            Thread.Sleep(500);

            System.IO.File.Move(physicallyRenameAddedFile.ToString(), physicallyRenameAddedFileNew.ToString());
            Thread.Sleep(2000);
            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (physicallyRenameAddedFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
                else if (physicallyRenameAddedFileNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }
            }
        }
    }
}
