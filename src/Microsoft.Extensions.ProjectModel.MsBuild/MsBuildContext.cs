// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        public static MsBuildContext FromDotNetCliContext()
        {
            var sdk = new DotNetSdkResolver().ResolveLatest();
            var msBuildFile = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "MSBuild.exe"
                : "MSBuild";

            return new MsBuildContext
            {
                MsBuildExecutableFullPath = msBuildFile,
                ExtensionsPath = sdk.BasePath
            };
        }
    }
}