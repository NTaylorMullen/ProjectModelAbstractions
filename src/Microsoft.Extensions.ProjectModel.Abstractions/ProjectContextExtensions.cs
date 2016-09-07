using System;

namespace Microsoft.Extensions.ProjectModel
{
    public static class ProjectContextExtensions
    {
        public static string FindProperty(this IProjectContext project, string propertyName)
            => project.FindProperty(propertyName, StringComparison.OrdinalIgnoreCase);

        // common, well-known properties
        private const string UserSecretsIdPropertyName = "userSecretsId";
        public static string UserSecretsId(this IProjectContext project)
            => project.FindProperty(UserSecretsIdPropertyName);
    }
}
