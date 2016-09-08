using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ProjectModel.Internal;

namespace Microsoft.Extensions.ProjectModel
{
    public class MsBuildDesignTimeContextFactory : IProjectContextFactory
    {
        private const string DesignTimeBuildTarget = "ResolveReferences";
        private readonly MsBuildContext _msbuildContext;
        private readonly IFileProvider _fileProvider;

        public MsBuildDesignTimeContextFactory(MsBuildContext context, IFileProvider fileProvider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _msbuildContext = context;
            _fileProvider = fileProvider;

            // workaround https://github.com/Microsoft/msbuild/issues/999
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", context.MsBuildExecutableFullPath);
        }

        public IProjectContext Create(string filePath, string configuration)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrEmpty(configuration))
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var fileInfo = _fileProvider.GetFileInfo(filePath);
            var projectCollection = new ProjectCollection();
            var project = CreateProject(fileInfo, configuration, projectCollection, _msbuildContext);
            var projectInstance = RunDesignTimeBuild(project);
            projectCollection.UnloadProject(project);

            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            return new MsBuildProjectContext(name, configuration, projectInstance);
        }

        private static ProjectInstance RunDesignTimeBuild(Project project)
        {
            var projectInstance = project.CreateProjectInstance();

            var logger = new InMemoryLogger();
            projectInstance.Build(DesignTimeBuildTarget, new[] { logger });
            // TODO what should we do when there are errors?
            //if (logger.Errors.Count > 0)
            //{
            //    throw new InvalidOperationException(logger.Errors[0].Message);
            //}
            return projectInstance;
        }

        private static Project CreateProject(IFileInfo fileInfo,
            string configuration,
            ProjectCollection projectCollection,
            MsBuildContext context)
        {
            var globalProperties = new Dictionary<string, string>
            {
                // in some test cases, adding configuration property causes MSBuild to not compute OutputPath correctly
                // { "Configuration", configuration },
                { "BuildProjectReferences", "false" },
                { "_ResolveReferenceDependencies", "true" },
                { "GenerateDependencyFile", "true" },
                { "DesignTimeBuild", "true" },
                { "MSBuildExtensionsPath", context.ExtensionsPath }
            };

            using (var stream = fileInfo.CreateReadStream())
            {
                var xmlReader = XmlReader.Create(stream);

                var xml = ProjectRootElement.Create(xmlReader, projectCollection);
                xml.FullPath = fileInfo.PhysicalPath;

                return new Project(xml, globalProperties, /*toolsVersion:*/ null, projectCollection);
            }
        }
    }
}
