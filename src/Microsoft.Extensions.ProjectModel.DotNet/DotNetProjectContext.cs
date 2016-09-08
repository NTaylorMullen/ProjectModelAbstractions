using System;
using System.IO;
using Microsoft.DotNet.ProjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Frameworks;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.ProjectModel
{
    public class DotNetProjectContext : IProjectContext
    {
        private readonly ProjectContext _project;
        private readonly OutputPaths _paths;
        private readonly bool _isExecutable;
        private readonly JObject _rawProject;
        private readonly CommonCompilerOptions _compilerOptions;

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

            using (var stream = new FileStream(wrappedProject.ProjectFile.ProjectFilePath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                _rawProject = JObject.Load(jsonReader);
            }
            _project = wrappedProject;
            _paths = wrappedProject.GetOutputPaths(configuration, /* buildBasePath: */ null, outputPath);

            // Workaround https://github.com/dotnet/cli/issues/3164
            _isExecutable = wrappedProject.ProjectFile.GetCompilerOptions(wrappedProject.TargetFramework, configuration).EmitEntryPoint
                            ?? wrappedProject.ProjectFile.GetCompilerOptions(null, configuration).EmitEntryPoint.GetValueOrDefault();

            _compilerOptions = _project.ProjectFile.GetCompilerOptions(TargetFramework, Configuration);

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
        // TODO read from xproj if available
        public string RootNamespace => _project.ProjectFile.Name;
        public string TargetDirectory => _paths.RuntimeOutputPath;
        public string Platform => _compilerOptions.Platform;

        /// <summary>
        /// Returns string values of top-level keys in the project.json file
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyNameComparer"></param>
        /// <returns></returns>
        public string FindProperty(string propertyName, StringComparison propertyNameComparer)
            => FindProperty<string>(propertyName, propertyNameComparer);

        public T FindProperty<T>(string propertyName, StringComparison propertyNameComparer)
        {
            foreach (var item in _rawProject)
            {
                if (item.Key.Equals(propertyName, propertyNameComparer))
                {
                    return item.Value.Value<T>();
                }
            }
            return default(T);
        }

        public IEnumerable<string> CompilationItems 
            => _compilerOptions.CompileInclude.ResolveFiles();
        public IEnumerable<string> EmbededItems
            => _compilerOptions.EmbedInclude.ResolveFiles();

        public ProjectContext Unwrap() => _project;
    }
}