using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpSvn;
using System.Threading;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class PhysicalAddTest : BaseTest
    {
        [TestMethod]
        public void StatusCache_PhysicalAdd_IsValid()
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
            FileInfo fi = new FileInfo(localWorkingLocation + countBefore + "PhysicalAdd.txt");
            
            fi.Create();
            Thread.Sleep(2000);
            var mappingsAfter = svnManagement.GetMappings();
            int countAfter = mappingsAfter.Count;

            // Assert
            foreach (var item in mappingsAfter)
            {
                if (fi.ToString() == item.Key)
                {
                    item.Value.Status.LocalNodeStatus.Should().Be(SvnStatus.NotVersioned);
                }                   
            }
        }
    }
}
