using System.IO;

namespace Microsoft.Extensions.ProjectModel.MsBuild.Test
{
    public class MsBuildFixture
    {
        public MsBuildContext GetMsBuildContext()
        {
            // the prefered way to do this...but this won't work if you run tests from preview2 versions of cli
            // return MsBuildContext.FromDotNetCliContext();
            var home = System.Environment.GetEnvironmentVariable("USERPROFILE")
                ?? System.Environment.GetEnvironmentVariable("HOME");
            var dotnetHome = Path.Combine(home, ".dotnet");
            return MsBuildContext.FromDotNetSdk(dotnetHome);
        }
    }
}