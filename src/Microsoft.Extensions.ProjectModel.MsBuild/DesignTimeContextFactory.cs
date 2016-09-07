using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace Microsoft.Extensions.ProjectModel.MsBuild
{
    public class DesignTimeContextFactory : IProjectContextFactory
    {
        private readonly MsBuildContext _msbuildContext;

        public DesignTimeContextFactory(MsBuildContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _msbuildContext = context;
        }

        public IProjectContext Create(string filePath, string configuration)
        {
            var project = CreateProject(filePath, configuration, _msbuildContext);
            var result = RunDesignTimeBuild(project);
            var name = Path.GetFileNameWithoutExtension(filePath);
            return new MsBuildProjectContext(name, configuration, result.ProjectStateAfterBuild);
        }

        private BuildResult RunDesignTimeBuild(Project project)
        {
            var projectInstance = project.CreateProjectInstance();
            var buildRequest = new BuildRequestData(projectInstance, projectInstance.DefaultTargets.ToArray());
            var buildParams = new BuildParameters(project.ProjectCollection);

            var result = BuildManager.DefaultBuildManager.Build(buildParams, buildRequest);

            // this is a hack for failed project builds. ProjectStateAfterBuild == null after a failed build
            // But the properties are still available to be read
            result.ProjectStateAfterBuild = projectInstance;

            return result;
        }

        private static Project CreateProject(string filePath, string configuration, MsBuildContext context)
        {
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", context.MsBuildExecutableFullPath);

            var globalProperties = new Dictionary<string, string>
            {
                { "Configuration", configuration },
                { "GenerateDependencyFile", "true" },
                { "DesignTimeBuild", "true" },
                { "MSBuildExtensionsPath", context.ExtensionsPath }
            };

            var xmlReader = XmlReader.Create(new FileStream(filePath, FileMode.Open));
            var projectCollection = new ProjectCollection();
            var xml = ProjectRootElement.Create(xmlReader, projectCollection);
            xml.FullPath = filePath;

            var project = new Project(xml, globalProperties, /*toolsVersion*/ null, projectCollection);
            return project;
        }

    }
}
