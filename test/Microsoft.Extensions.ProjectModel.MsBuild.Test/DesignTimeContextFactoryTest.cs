using System.IO;
using System.Linq;
using NuGet.Frameworks;
using Xunit;

namespace Microsoft.Extensions.ProjectModel.MsBuild.Test
{
    public class DesignTimeContextFactoryTest : IClassFixture<MsBuildFixture>
    {
        private readonly MsBuildFixture _fixture;

        public DesignTimeContextFactoryTest(MsBuildFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ExecutesDesignTimeBuild()
        {
            //arrange
            using (var fileProvider = new TemporaryFileProvider())
            {
                fileProvider.Add("test.csproj", @"
<Project ToolsVersion=""14.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" />

  <PropertyGroup>
    <RootNamespace>Microsoft.TestProject</RootNamespace>
    <ProjectName>TestProject</ProjectName>
    <OutputType>Library</OutputType>
    <TargetFrameworkIdentifier>.NETCoreApp</TargetFrameworkIdentifier>
    <TargetFrameworkVersion>v1.0</TargetFrameworkVersion>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include=""**\*.cs"" Exclude=""Excluded.cs"" />
  </ItemGroup>

  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
");
                fileProvider.Add("One.cs", "public class Abc {}");
                fileProvider.Add("Two.cs", "public class Abc2 {}");
                fileProvider.Add("Excluded.cs", "public class Abc {}");

                var testContext = _fixture.GetMsBuildContext();

                var factory = new MsBuildDesignTimeContextFactory(testContext, fileProvider);
                var expectedCompileItems = new[] { "One.cs", "Two.cs" }.Select(p => Path.Combine(fileProvider.Root, p)).ToArray();
                //act
                var context = (MsBuildProjectContext)factory.Create("test.csproj", "Debug");

                //assert
                Assert.False(fileProvider.GetFileInfo("bin").Exists);
                Assert.False(fileProvider.GetFileInfo("obj").Exists);
                Assert.Equal(expectedCompileItems, context.CompilationItems.OrderBy(i => i).ToArray());
                Assert.Equal(Path.Combine(fileProvider.Root, "bin", "Debug", "test.dll"), context.AssemblyFullPath);
                Assert.True(context.IsClassLibrary);
                Assert.Equal("TestProject", context.ProjectName);
                Assert.Equal(FrameworkConstants.CommonFrameworks.NetCoreApp10, context.TargetFramework);
                Assert.Equal("Microsoft.TestProject", context.RootNamespace);
            }
        }
    }
}
