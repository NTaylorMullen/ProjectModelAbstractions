using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.ProjectModel.Internal;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.ProjectModel
{
    /// <summary>
    /// Represents the msbuild context used to parse a project model
    /// </summary>
    public class MsBuildContext
    {
        public string MsBuildExecutableFullPath { get; set; }
        public string ExtensionsPath { get; set; }

        public static MsBuildContext FromDotNetSdk()
            => FromDotNetSdk(dotnetInstallationPath: Path.GetDirectoryName(new Muxer().MuxerPath));

        internal static MsBuildContext FromDotNetSdk(string dotnetInstallationPath)
        {
            var sdk = new DotNetSdkResolver(dotnetInstallationPath).ResolveProjectSdk();
            // Despite what you may think, you need the ".exe" even on Linux.
            // var msBuildFile = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            //     ? "MSBuild.exe"
            //     : "MSBuild";
            var msBuildFile = "MSBuild.exe";

            return new MsBuildContext
            {
                MsBuildExecutableFullPath = Path.Combine(sdk.BasePath, msBuildFile),
                ExtensionsPath = sdk.BasePath
            };
        }
    }
}