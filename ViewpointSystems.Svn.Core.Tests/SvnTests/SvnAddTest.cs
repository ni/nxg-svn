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
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "SvnAdd.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);

            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Added);
                }
            }
        }
    }
}
