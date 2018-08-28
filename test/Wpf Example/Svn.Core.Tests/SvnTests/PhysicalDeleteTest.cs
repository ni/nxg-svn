using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using Svn.Core.ViewModels;
using Svn.SvnThings;

namespace Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class PhysicalDeleteTest : BaseTest
    {
        [TestMethod]
        public void StatusCache_PhysicalDeleteNormal_IsValid()
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
            var physicalDeleteNormalFile = new FileInfo(Path.Combine(UnitTestPath, countBefore +  "_PhysicalDeleteNormal.txt"));

            var myFile = File.Create(physicalDeleteNormalFile.ToString());
            myFile.Close();
            Thread.Sleep(500);

            physicalDeleteNormalFile.Delete();
            Thread.Sleep(2000);
            var mappings = SvnManager.GetMappings();

            // Assert

            foreach (var item in mappings)
            {
                if (physicalDeleteNormalFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
            }

        }

        [TestMethod]
        public void StatusCache_PhysicalDeleteAdded_IsValid()
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
            var physicalDeleteAddedFile = new FileInfo(Path.Combine(UnitTestPath, countBefore +  "_PhysicalDeleteAdded.txt"));

            var myFile = File.Create(physicalDeleteAddedFile.ToString());
            myFile.Close();

            SvnManager.Add(physicalDeleteAddedFile.ToString());
            Thread.Sleep(500);

            physicalDeleteAddedFile.Delete();
            Thread.Sleep(2000);
            var mappings = SvnManager.GetMappings();

            // Assert

            foreach (var item in mappings)
            {
                if (physicalDeleteAddedFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
            }

        }
    }
}
