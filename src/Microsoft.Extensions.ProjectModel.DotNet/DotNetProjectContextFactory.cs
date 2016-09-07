using Microsoft.DotNet.ProjectModel;

namespace Microsoft.Extensions.ProjectModel
{
    public class DotNetProjectContextFactory : IProjectContextFactory
    {
        public IProjectContext Create(string filePath, string configuration)
        {
            var project = new ProjectContextBuilder()
                .AsDesignTime()
                .WithProject(ProjectReader.GetProject(filePath))
                .Build();

            return new DotNetProjectContext(project, configuration, outputPath: null);
        }
    }
}
