namespace Microsoft.Extensions.ProjectModel
{
    public interface IProjectContextFactory
    {
        IProjectContext Create(string filePath, string configuration);
    }
}
