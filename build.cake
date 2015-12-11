#tool "NUnit.Console"

using System.Xml.Linq;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

// Variable definitions

var projectName = "NString";
var solutionFile = $"./{projectName}.sln";
var outDir = $"./{projectName}/bin/{configuration}";

var unitTestAssemblies = new[] { $"{projectName}.Tests/bin/{configuration}/{projectName}.Tests.dll" };

var nuspecFile = $"./NuGet/{projectName}.nuspec";
var nugetDir = $"./NuGet/{configuration}";
var nupkgDir = $"{nugetDir}/nupkg";
var nugetTargets = new[] { $"dotnet", $"portable-net45+win8+wpa81+wp8" };
var nugetFiles = new[] { $"{projectName}.dll", $"{projectName}.xml" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(outDir);
        CleanDirectory(nugetDir);
    });

Task("Restore")
    .Does(() =>
    {
        NuGetRestore(solutionFile);
    });

Task("JustBuild")
    .Does(() =>
    {
        MSBuild(solutionFile,
            settings => settings.SetConfiguration(configuration));    
    });

Task("JustTest")
    .Does(() =>
    {
        NUnit3(unitTestAssemblies);
    });
    
Task("JustPack")
    .Does(() =>
    {
        CreateDirectory(nupkgDir);
        foreach (var target in nugetTargets)
        {
            string targetDir = $"{nupkgDir}/lib/{target}";
            CreateDirectory(targetDir);
            foreach (var file in nugetFiles)
            {
                CopyFileToDirectory($"{outDir}/{file}", targetDir);
            }
        }
        var packSettings = new NuGetPackSettings
        {
            BasePath = nupkgDir,
            OutputDirectory = nugetDir
        };
        NuGetPack(nuspecFile, packSettings);
    });

Task("JustPush")
    .Does(() =>
    {
        var doc = XDocument.Load(nuspecFile);
        var ns = XNamespace.Get("http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");
        string version = doc.Root.Element(ns + "metadata").Element(ns + "version").Value;
        string package = $"{nugetDir}/{projectName}.{version}.nupkg";
        NuGetPush(package, new NuGetPushSettings());
    });

Task("UploadTestResults")
    .Does(() =>
    {
        using (var wc = new System.Net.WebClient())
        {
            var jobId = EnvironmentVariable("APPVEYOR_JOB_ID");
            var url = $"https://ci.appveyor.com/api/testresults/nunit3/{jobId}";
            var path = MakeAbsolute(File("TestResult.xml")).ToString();
            wc.UploadFile(url, path);
        }
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
    
Task("AppVeyor")
    .IsDependentOn("Test")
    .IsDependentOn("JustPack");
    

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
