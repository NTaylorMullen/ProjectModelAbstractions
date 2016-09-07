using Microsoft.Extensions.ProjectModel;

namespace Microsoft.DotNet.ProjectModel
{
    public static class ProjectContextExtensions
    {
        private const string DefaultConfiguration = "Debug";
        public static IProjectContext AsAbstract(this ProjectContext context)
        {
            return new DotNetProjectContext(context, DefaultConfiguration, outputPath: null);
        }
    }
}
