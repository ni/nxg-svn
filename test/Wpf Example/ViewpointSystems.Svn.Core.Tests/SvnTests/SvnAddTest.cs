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
    public class SvnAddTest : BaseTest
    {
        [TestMethod]
        public void SvnCache_SvnAdd_IsValid()
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
            var svnAdd = new FileInfo(Path.Combine(SvnManagement.GetRoot(UnitTestPath), countBefore + "_SvnAdd.txt"));

            var myFile = File.Create(svnAdd.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            SvnManagement.Add(svnAdd.ToString());

            Thread.Sleep(3000);

            var mappingsAfter = SvnManagement.GetMappings();
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
