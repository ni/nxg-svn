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
    public class SvnRenameFile : BaseTest
    {
        [TestMethod]
        public void StatusCache_SvnRenameCommittedFile_IsValid()
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
            var svnRenameCommittedFile = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_SvnRenameCommittedFile.txt"));

            var myFile = File.Create(svnRenameCommittedFile.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(svnRenameCommittedFile.ToString());

            Thread.Sleep(500);
            SvnManager.Commit(svnRenameCommittedFile.ToString(), "Unit Test");
            Thread.Sleep(2000);
            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            var fiNew = new FileInfo(Path.Combine(UnitTestPath, countAfter + "SvnRename_SvnRenameCommittedFile.txt"));

            SvnManager.SvnRename(svnRenameCommittedFile.ToString(), fiNew.ToString());
            Thread.Sleep(1000);

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (svnRenameCommittedFile.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Deleted);
                }

                if (fiNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Added);
                }
            }
        }

        [TestMethod]
        public void StatusCache_SvnRenameAdded_IsValid()
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
            var svnRenameAdded = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_SvnRenameAdded.txt"));

            var myFile = File.Create(svnRenameAdded.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(svnRenameAdded.ToString());

            Thread.Sleep(1000);

            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            var fiNew = new FileInfo(Path.Combine(UnitTestPath, countBefore + "SvnRename_SvnRenameAdded.txt"));

            SvnManager.SvnRename(svnRenameAdded.ToString(), fiNew.ToString());
            Thread.Sleep(2000);

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (fiNew.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Added);
                }
            }
        }
    }
}
