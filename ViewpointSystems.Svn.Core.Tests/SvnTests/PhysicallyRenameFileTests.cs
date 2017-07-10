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
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "PhysicallyRenameNormal.txt");
            FileInfo fiNew = new FileInfo(localWorkingLocation + "PhysicallyRenameNormalRenamed" + countBefore + 1 + "PhysicallyRenameNormalRenamed.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);
            svnManagement.CommitChosenFiles(fi.ToString(), "Unit Test");
            Thread.Sleep(2000);

            System.IO.File.Move(fi.ToString(), fiNew.ToString());
            Thread.Sleep(2000);
            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                    
                }
                else if (fiNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }
            }
        }

        [TestMethod]
        public void StatusCache_PhysicallyRenameAdded_isValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + ".txt");
            FileInfo fiNew = new FileInfo(localWorkingLocation + countBefore + 1 + ".txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);

            System.IO.File.Move(fi.ToString(), fiNew.ToString());
            Thread.Sleep(2000);
            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
                else if (fiNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }
            }
        }
    }
}
