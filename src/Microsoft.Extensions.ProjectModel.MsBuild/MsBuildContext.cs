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
            => FromDotNetSdk(sdkInstallationPath: Path.GetDirectoryName(new Muxer().MuxerPath));

        internal static MsBuildContext FromDotNetSdk(string sdkInstallationPath)
        {
            var sdk = new DotNetSdkResolver(sdkInstallationPath).ResolveProjectSdk();
            var msBuildFile = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "MSBuild.exe"
                : "MSBuild";

            return new MsBuildContext
            {
                MsBuildExecutableFullPath = Path.Combine(sdk.BasePath, msBuildFile),
                ExtensionsPath = sdk.BasePath
            };
        }
    }
}