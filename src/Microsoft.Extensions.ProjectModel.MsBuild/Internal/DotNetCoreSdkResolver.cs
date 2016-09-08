// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Versioning;

namespace Microsoft.Extensions.ProjectModel.Internal
{
    internal class DotNetSdkResolver
    {
        private readonly string _installationPath;

        public DotNetSdkResolver(string installationDir)
        {
            _installationPath = installationDir;
        }

        private IEnumerable<string> Installed
            => Directory.EnumerateDirectories(Path.Combine(_installationPath, "sdk"));

        /// <summary>
        /// Find the latest SDK installation (according to SemVer 1.0)
        /// </summary>
        /// <returns>Path to SDK root directory</returns>
        public DotNetCoreSdk ResolveLatest()
        {
            var first = Installed.Select(d => new { path = d, version = SemanticVersion.Parse(Path.GetFileName(d)) })
                .OrderByDescending(sdk => sdk.version)
                .First();

            return new DotNetCoreSdk
            {
                BasePath = first.path,
                Version = first.version.ToFullString()
            };
        }

        // TODO look at global.json
        public DotNetCoreSdk ResolveProjectSdk() => ResolveLatest();
    }

}