using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using Svn.SvnThings;

namespace Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class SvnAddTest : BaseTest
    {
        [TestMethod]
        public void SvnCache_SvnAdd_IsValid()
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
            var svnAdd = new FileInfo(Path.Combine(SvnManager.GetRoot(UnitTestPath), countBefore + "_SvnAdd.txt"));

            var myFile = File.Create(svnAdd.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManager.Add(svnAdd.ToString());

            Thread.Sleep(3000);

            var mappingsAfter = SvnManager.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (svnAdd.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Added);
                }
            }
        }
    }
}
