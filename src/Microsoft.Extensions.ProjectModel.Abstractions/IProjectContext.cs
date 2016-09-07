using NuGet.Frameworks;
using System;

namespace Microsoft.Extensions.ProjectModel
{
    public interface IProjectContext
    {
        string ProjectName { get; }
        string Configuration { get; }
        string Platform { get; }
        string ProjectFullPath { get; }
        string RootNamespace { get; }
        bool IsClassLibrary { get; }
        NuGetFramework TargetFramework { get; }
        string Config { get; }
        string DepsJson { get; }
        string RuntimeConfigJson { get; }
        string PackagesDirectory { get; }
        string TargetDirectory { get; }
        string AssemblyFullPath { get; }
        string FindProperty(string propertyName, StringComparison propertyNameComparer);
    }
}