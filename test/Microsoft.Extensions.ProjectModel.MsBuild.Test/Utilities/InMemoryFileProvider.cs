using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.ProjectModel.MsBuild.Test
{
    internal class TemporaryFileProvider : PhysicalFileProvider, IDisposable
    {
        public TemporaryFileProvider()
            :base(Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "pmtests", Guid.NewGuid().ToString())).FullName)
        {
        }

        public void Add(string filename, string contents)
        {
            File.WriteAllText(Path.Combine(this.Root, filename), contents, Encoding.UTF8);
        }

        public void Dispose()
        {
            base.Dispose();
            Directory.Delete(this.Root, recursive: true);
        }
    }
}