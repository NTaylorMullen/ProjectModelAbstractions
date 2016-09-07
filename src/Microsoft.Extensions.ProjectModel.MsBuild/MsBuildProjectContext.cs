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
            ProjectFullPath = FindProperty("ProjectPath");
            RootNamespace = FindProperty("RootNamespace") ?? ProjectName;
            TargetFramework = NuGetFramework.Parse(FindProperty("NuGetTargetMoniker"));
            IsClassLibrary = FindProperty("OutputType").Equals("Library", StringComparison.OrdinalIgnoreCase);
            TargetDirectory = FindProperty("TargetDir");
            Platform = FindProperty("Platform");
            AssemblyFullPath = FindProperty("TargetPath");
            PackagesDirectory = FindProperty("NuGetPackageRoot");

            // TODO get from actual properties according to TFM
            Config = AssemblyFullPath + ".config";
            var assemblyFileName = Path.GetFileNameWithoutExtension(AssemblyFullPath);
            RuntimeConfigJson = Path.Combine(TargetDirectory, assemblyFileName + ".runtimeconfig.json");
            DepsJson = Path.Combine(TargetDirectory, assemblyFileName + ".deps.json");
        }

        public string FindProperty(string propertyName)
            => _project.FindProperty(propertyName);

        public NuGetFramework TargetFramework { get; }
        public bool IsClassLibrary { get; }
        public string Config { get; }
        public string DepsJson { get; }
        public string RuntimeConfigJson { get; }
        public string PackagesDirectory { get; }
        public string AssemblyFullPath { get; }
        public string ProjectName { get; }
        public string Configuration { get; }
        public string Platform { get; }
        public string ProjectFullPath { get; }
        public string RootNamespace { get; }
        public string TargetDirectory { get; }

        public ProjectInstance Unwrap() => _project;
    }
}