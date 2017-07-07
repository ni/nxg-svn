using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using ViewpointSystems.Svn.Core.ViewModels;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class PhysicalDeleteTest : BaseTest
    {
        [TestMethod]
        public void StatusCache_PhysicalDeleteNormal_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";
            StatusViewModel status = new StatusViewModel();
            status.Initialize();

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "PhysicalDeleteNormal.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(500);

            fi.Delete();
            Thread.Sleep(2000);
            var mappings = svnManagement.GetMappings();

            // Assert

            foreach (var item in mappings)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
            }

        }

        [TestMethod]
        public void StatusCache_PhysicalDeleteAdded_IsValid()
        {
            // Arrange
            SvnManagement svnManagement = new SvnManagement();
            Ioc.RegisterSingleton<SvnManagement>(svnManagement);
            string localWorkingLocation = @"C:\WorkingRepo\";
            StatusViewModel status = new StatusViewModel();
            status.Initialize();

            // Act

            if (svnManagement.IsWorkingCopy(localWorkingLocation))
            {
                svnManagement.LoadCurrentSvnItemsInLocalRepository(localWorkingLocation);
            }

            var mappingsBefore = svnManagement.GetMappings();
            int countBefore = mappingsBefore.Count;
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "PhysicalDeleteAdded.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();

            svnManagement.Add(fi.ToString());
            Thread.Sleep(500);

            fi.Delete();
            Thread.Sleep(2000);
            var mappings = svnManagement.GetMappings();

            // Assert

            foreach (var item in mappings)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.Missing);
                }
            }

        }
    }
}
