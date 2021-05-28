using MessageInterceptor.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MessageInterceptor.Core
{
    public class AssemblyHelper
    {
        private static IOptions<AssemblyInfo> assemblyOptions;
        public AssemblyHelper(IOptions<AssemblyInfo> options)
        {
            assemblyOptions = options;
        }
        public I CreateInstance<I>() where I : class
        {
            var assemblyRootPath = AppDomain.CurrentDomain.BaseDirectory;
            var assemblyInfo = GetAssemblyInfo();
            if (assemblyInfo == null)
                return null;
            if (string.IsNullOrEmpty(assemblyInfo.AssemblyName))
                return null;
            var assemblyLoader = new AssemblyLoader(assemblyRootPath);
            Assembly assembly = assemblyLoader.Load(assemblyInfo.AssemblyName);
            Type type = assembly.GetType(assemblyInfo.ClassInfo);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as I;
        }
        private static AssemblyInfo GetAssemblyInfo()
        {
            if (assemblyOptions == null)
            {
                return null;
            }
            var assemblyInfo = assemblyOptions.Value;
            if (assemblyInfo == null)
                return null;

            return assemblyInfo;
        }
    }
}
