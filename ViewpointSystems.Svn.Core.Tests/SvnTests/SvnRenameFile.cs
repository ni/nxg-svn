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
    public class SvnRenameFile : BaseTest
    {
        [TestMethod]
        public void StatusCache_SvnRenameCommittedFile_IsValid()
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
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "SvnRenameCommittedFile.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(500);
            svnManagement.CommitChosenFiles(fi.ToString(), "Unit Test");
            Thread.Sleep(2000);
            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            FileInfo fiNew = new FileInfo(localWorkingLocation + "SvnRename" + countAfter + "SvnRenameCommittedFile.txt");

            svnManagement.SvnRename(fi.ToString(), fiNew.ToString());
            Thread.Sleep(1000);

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
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
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "SvnRenameAdded.txt");

            var myFile = File.Create(fi.ToString());
            myFile.Close();
            Thread.Sleep(2000);

            svnManagement.Add(fi.ToString());

            Thread.Sleep(1000);

            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            FileInfo fiNew = new FileInfo(localWorkingLocation + "SvnRename" + countBefore + "SvnRenameAdded.txt");

            svnManagement.SvnRename(fi.ToString(), fiNew.ToString());
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
