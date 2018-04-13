#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin "Cake.Powershell"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var Version = Argument("my_version", "2.0.0.0");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////
var buildDir = Directory("./ViewpointSystems.Svn.Plugin/bin/x64") + Directory(configuration);
var niPackDir = Directory("./NIPKG/pkg-ext/ext-src/data/ni-paths-LVNXG200DIR64/Addons/viewpoint/svntoolkit/base-ext/");
var eulaDir = Directory("./NIPKG/pkg-eula/pack-eula.bat");
var extDir = Directory("./NIPKG/pkg-ext/pack-ext.bat");
var niRepo = Directory("./NIPKG/repo");
var niEulaControlFile = Directory("./NIPKG/pkg-eula/eula-src/control/control");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./ViewpointSystems.Svn.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")	
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./ViewpointSystems.Svn.sln", settings =>
        settings.SetConfiguration(configuration)
		.UseToolVersion(MSBuildToolVersion.Default)
		.WithTarget("Rebuild")
		.SetPlatformTarget(PlatformTarget.x64)
		);
    }
    else
    {
      // Use XBuild
      XBuild("./ViewpointSystems.Svn.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Copy-Build-Directory")
    .IsDependentOn("Build")	
    .Does(() =>
{
	CleanDirectory(niPackDir);
	niPackDir = niPackDir + Directory(Version);	
    CopyDirectory(buildDir, niPackDir);
});

Task("Generate-Eula")
    .IsDependentOn("Copy-Build-Directory")	
    .Does(() =>
{
	CleanDirectory(niRepo);
	
	//update Version number 
	using(var process = StartAndReturnProcess("./C:/work/TestSvn/NIPKG/pkg-eula/eula-src/control/test.bat", new ProcessSettings{ Arguments = "-version " + Version } ))
	{
    process.WaitForExit();
    // This should output 0 as valid arguments supplied
    Information("Exit code: {0}", process.GetExitCode());
	}	
	
	
    using(var process = StartAndReturnProcess("C:/work/TestSvn/NIPKG/pkg-eula/pack-eula.bat"))
	{
    process.WaitForExit();
    // This should output 0 as valid arguments supplied
    Information("Exit code: {0}", process.GetExitCode());
	}	
});

Task("Generate-ext")
    .IsDependentOn("Generate-Eula")	
    .Does(() =>
{
	//update Version number 
	using(var process = StartAndReturnProcess("./C:/work/TestSvn/NIPKG/pkg-ext/ext-src/control/test.bat", new ProcessSettings{ Arguments = "-version " + Version } ))
	{
    process.WaitForExit();
    // This should output 0 as valid arguments supplied
    Information("Exit code: {0}", process.GetExitCode());
	}
	
    using(var process = StartAndReturnProcess("C:/work/TestSvn/NIPKG/pkg-ext/pack-ext.bat"))
	{
    process.WaitForExit();
    // This should output 0 as valid arguments supplied
    Information("Exit code: {0}", process.GetExitCode());
	}	
});

Task("Generate-pkg")
    .IsDependentOn("Generate-ext")	
    .Does(() =>
{
    using(var process = StartAndReturnProcess("C:/work/TestSvn/NIPKG/create-repo.bat"))
	{
    process.WaitForExit();
    // This should output 0 as valid arguments supplied
    Information("Exit code: {0}", process.GetExitCode());
	}	
});



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Generate-pkg");

RunTarget(target);