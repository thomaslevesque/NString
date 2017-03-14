using System.Xml.Linq;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

// Variable definitions

var projectName = "NString";
var libraryProject = $"{projectName}/{projectName}.csproj";
var testProject = $"{projectName}.Tests/{projectName}.Tests.csproj";
var outDir = $"{projectName}/bin/{configuration}";

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(outDir);
    });

Task("Restore").Does(() => DotNetCoreRestore());

Task("JustBuild")
    .Does(() =>
    {
        DotNetCoreBuild(".", new DotNetCoreBuildSettings { Configuration = configuration });
    });

Task("JustTest")
    .Does(() => DotNetCoreTest(testProject));
    
Task("JustPack")
    .Does(() =>
    {
        DotNetCorePack(libraryProject, new DotNetCorePackSettings
        {
            Configuration = configuration
        });
    });

Task("JustPush")
    .Does(() =>
    {
        var doc = XDocument.Load(libraryProject);
        string version = doc.Root.Elements("PropertyGroup").Elements("Version").First().Value;
        string package = $"{projectName}/bin/{configuration}/{projectName}.{version}.nupkg";
        NuGetPush(package, new NuGetPushSettings());
    });

// Higher level tasks

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("JustBuild");

Task("Test")
    .IsDependentOn("Build")
    .IsDependentOn("JustTest");

Task("Pack")
    .IsDependentOn("Build")
    .IsDependentOn("JustPack");

Task("Push")
    .IsDependentOn("Pack")
    .IsDependentOn("JustPush");

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
