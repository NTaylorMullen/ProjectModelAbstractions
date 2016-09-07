using System;
using System.Linq;

namespace Microsoft.Build.Execution
{
    public static class MsBuildExtensions
    {
        public static string FindProperty(this ProjectInstance projectInstance, string propertyName, StringComparison comparer)
            => projectInstance.Properties.FirstOrDefault(p => p.Name.Equals(propertyName, comparer))?.EvaluatedValue;
    }
}
