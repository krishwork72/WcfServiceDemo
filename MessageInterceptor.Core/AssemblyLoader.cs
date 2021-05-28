using Microsoft.Extensions.DependencyModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace MessageInterceptor.Core
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private string folderPath;

        public AssemblyLoader(string folderPath)
        {
            this.folderPath = folderPath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return Load(assemblyName.Name);
        }
        public Assembly Load(string assemblyName)
        {
            const string libraryExtention = ".dll";
            var deps = DependencyContext.Default;
            if (assemblyName != null)
            {
                var libraryName = assemblyName.Replace(libraryExtention, string.Empty);
                var res = deps.CompileLibraries.Where(d => d.Name.Contains(libraryName)).ToList();
                if (res.Count > 0)
                {
                    return Assembly.Load(new AssemblyName(res.First().Name));
                }
                else
                {
                    if (!assemblyName.Contains(libraryExtention) && !assemblyName.Contains(".exe"))
                        assemblyName = $"{assemblyName}{libraryExtention}";

                    var apiApplicationFileInfo = new FileInfo($"{folderPath}{Path.DirectorySeparatorChar}{assemblyName}");
                    if (File.Exists(apiApplicationFileInfo.FullName))
                    {
                        var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
                        return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
                    }
                }
            }
            return Assembly.Load(assemblyName);
        }
    }
}
