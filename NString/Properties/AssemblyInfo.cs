using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using NString.Properties;

[assembly: AssemblyTitle("NString")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NString")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion(VersionInfo.Version)]
[assembly: AssemblyFileVersion(VersionInfo.Version)]

[assembly: InternalsVisibleTo("NString.Tests")]

namespace NString.Properties
{
    static class VersionInfo
    {
        public const string Version = "1.1.8.0";
    }
}