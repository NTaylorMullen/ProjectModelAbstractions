using System;
using Microsoft.DotNet.ProjectModel;
using NuGet.Frameworks;

namespace Microsoft.Extensions.ProjectModel
{
    public class DotNetProjectContext : IProjectContext
    {
        private readonly ProjectContext _project;
        private readonly OutputPaths _paths;
        private readonly bool _isExecutable;

        public DotNetProjectContext(ProjectContext wrappedProject, string configuration, string outputPath)
        {
            if (wrappedProject == null)
            {
                throw new ArgumentNullException(nameof(wrappedProject));
            }

            if (string.IsNullOrEmpty(configuration))
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _project = wrappedProject;
            _paths = wrappedProject.GetOutputPaths(configuration, /* buildBasePath: */ null, outputPath);

            // Workaround https://github.com/dotnet/cli/issues/3164
            _isExecutable = wrappedProject.ProjectFile.GetCompilerOptions(wrappedProject.TargetFramework, configuration).EmitEntryPoint
                            ?? wrappedProject.ProjectFile.GetCompilerOptions(null, configuration).EmitEntryPoint.GetValueOrDefault();

            Configuration = configuration;
        }

        public bool IsClassLibrary => !_isExecutable;

        public NuGetFramework TargetFramework => _project.TargetFramework;
        public string Config => _paths.RuntimeFiles.Config;
        public string DepsJson => _paths.RuntimeFiles.DepsJson;
        public string RuntimeConfigJson => _paths.RuntimeFiles.RuntimeConfigJson;
        public string PackagesDirectory => _project.PackagesDirectory;

        public string AssemblyFullPath =>
            _isExecutable && (_project.IsPortable || TargetFramework.IsDesktop())
                ? _paths.RuntimeFiles.Executable
                : _paths.RuntimeFiles.Assembly;

        public string Configuration { get; }
        public string ProjectFullPath => _project.ProjectFile.ProjectFilePath;
        public string ProjectName => _project.ProjectFile.Name;
        public string RootNamespace => _project.ProjectFile.Name;
        public string TargetDirectory => _paths.RuntimeOutputPath;
        public string Platform => _project.ProjectFile.GetCompilerOptions(TargetFramework, Configuration).Platform;
    }
}