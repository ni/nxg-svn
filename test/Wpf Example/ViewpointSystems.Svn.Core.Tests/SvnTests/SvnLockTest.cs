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
    public class SvnLockTest : BaseTest
    {
        [TestMethod]
        public void SvnStatus_SvnLock_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\UnitTestRepo\";

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "SvnLock.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);
            svnManagement.CommitChosenFiles(fi.ToString(), "Unit Test lock");
            Thread.Sleep(2000);
            svnManagement.SvnLock(fi.ToString(), "Locking Test");
            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Normal);
                    item.Value.Status.IsLockedLocal.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void SvnStatus_SvnUnLock_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\UnitTestRepo\";

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "SvnUnLock.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);
            svnManagement.CommitChosenFiles(fi.ToString(), "Unit Test lock");
            Thread.Sleep(2000);
            svnManagement.SvnLock(fi.ToString(), "Locking Test");
            Thread.Sleep(1000);

            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    if (item.Value.Status.IsLockedLocal)
                    {
                        svnManagement.SvnUnlock(fi.ToString());
                    }
                }
            }
            Thread.Sleep(500);

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Normal);
                    item.Value.Status.IsLockedLocal.Should().BeFalse();
                }
            }
        }
    }
}
