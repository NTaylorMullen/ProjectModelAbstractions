// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Build.Execution;
using NuGet.Frameworks;

namespace Microsoft.Extensions.ProjectModel
{
    public class MsBuildProjectContext : IProjectContext
    {
        private readonly ProjectInstance _project;

        public MsBuildProjectContext(string name, string configuration, ProjectInstance project)
        {
            _project = project;

            Configuration = configuration;
            ProjectName = name;
        }

        private string _assemblyFileName => Path.GetFileNameWithoutExtension(AssemblyFullPath);

        public string FindProperty(string propertyName, StringComparison propertyNameComparer)
            => _project.FindProperty(propertyName, propertyNameComparer);

        public string ProjectName { get; }
        public string Configuration { get; }

        public NuGetFramework TargetFramework => NuGetFramework.Parse(this.FindProperty("NuGetTargetMoniker"));
        public bool IsClassLibrary => this.FindProperty("OutputType").Equals("Library", StringComparison.OrdinalIgnoreCase);

        // TODO get from actual properties according to TFM
        public string Config => AssemblyFullPath + ".config";
        public string DepsJson => Path.Combine(TargetDirectory, _assemblyFileName + ".deps.json");
        public string RuntimeConfigJson => Path.Combine(TargetDirectory, _assemblyFileName + ".runtimeconfig.json");

        public string PackagesDirectory => this.FindProperty("NuGetPackageRoot");
        public string AssemblyFullPath => this.FindProperty("TargetPath");
        public string Platform => this.FindProperty("Platform");
        public string ProjectFullPath => this.FindProperty("ProjectPath");
        public string RootNamespace => this.FindProperty("RootNamespace") ?? ProjectName;
        public string TargetDirectory => this.FindProperty("TargetDir");

        public ProjectInstance Unwrap() => _project;
    }
}