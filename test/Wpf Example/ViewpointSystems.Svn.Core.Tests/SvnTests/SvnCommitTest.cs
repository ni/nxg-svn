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
    public class SvnCommitTest : BaseTest
    {
        [TestMethod]
        public void StatusCache_SvnCommit_IsValid()
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
            var svnCommit = new FileInfo(Path.Combine(UnitTestPath, countBefore + "_SvnCommit.txt"));

            var myFile = File.Create(svnCommit.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(svnCommit.ToString());

            Thread.Sleep(500);
            SvnManager.Commit(svnCommit.ToString(), "Unit Test");
            Thread.Sleep(2000);
            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            countBefore.Should().BeLessThan(countAfter);
            foreach (var item in mappingsAfter)
            {
                if (svnCommit.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Normal);
                }
            }
        }
    }
}
