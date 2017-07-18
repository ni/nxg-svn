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
            var rootPath = SvnManagement.GetRoot(UnitTestPath);
            // Act

            if (SvnManagement.IsWorkingCopy(rootPath))
            {
                SvnManagement.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappingsBefore = SvnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            var svnLock = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_SvnLock.txt"));

            var myFile = File.Create(svnLock.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManagement.Add(svnLock.ToString());

            Thread.Sleep(500);
            SvnManagement.CommitChosenFiles(svnLock.ToString(), "Unit Test lock");
            Thread.Sleep(2000);
            SvnManagement.SvnLock(svnLock.ToString(), "Locking Test");
            var mappingsAfter = SvnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (svnLock.ToString() == item.Key)
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
            var rootPath = SvnManagement.GetRoot(UnitTestPath);
            // Act

            if (SvnManagement.IsWorkingCopy(rootPath))
            {
                SvnManagement.LoadCurrentSvnItemsInLocalRepository(rootPath);
            }

            var mappingsBefore = SvnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            var svnUnLock = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_SvnUnLock.txt"));

            var myFile = File.Create(svnUnLock.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManagement.Add(svnUnLock.ToString());

            Thread.Sleep(500);
            SvnManagement.CommitChosenFiles(svnUnLock.ToString(), "Unit Test lock");
            Thread.Sleep(2000);
            SvnManagement.SvnLock(svnUnLock.ToString(), "Locking Test");
            Thread.Sleep(1000);

            var mappingsAfter = SvnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            foreach (var item in mappingsAfter)
            {
                if (svnUnLock.ToString() == item.Key)
                {
                    if (item.Value.Status.IsLockedLocal)
                    {
                        SvnManagement.SvnUnlock(svnUnLock.ToString());
                    }
                }
            }
            Thread.Sleep(500);

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (svnUnLock.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Normal);
                    item.Value.Status.IsLockedLocal.Should().BeFalse();
                }
            }
        }
    }
}
