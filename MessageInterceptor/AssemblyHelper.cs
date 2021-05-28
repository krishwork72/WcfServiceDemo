using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MessageInterceptor
{
    public static class AssemblyHelper
    {
        private const string InterceptorType = "CheckInterceptType";
        private static string interceptor;
        static AssemblyHelper()
        {
            interceptor = ConfigurationManager.AppSettings[InterceptorType];
        }
        public static I CreateInstance<I>() where I : class
        {
            var assemblyRootPath = AppDomain.CurrentDomain.RelativeSearchPath;
            var assemblyInfo = GetAssemblyInfo();
            if (assemblyInfo == null)
                return null;

            Assembly assembly;
            string assemblyPath = $"{assemblyRootPath}{ Path.DirectorySeparatorChar}{assemblyInfo.Item2}";
            if (File.Exists(assemblyPath))
                assembly = Assembly.LoadFrom(assemblyPath);
            else
                assembly = Assembly.GetCallingAssembly();

            Type type = assembly.GetType(assemblyInfo.Item1);
            if (type == null)
                return null;

            return Activator.CreateInstance(type) as I;
        }
        private static Tuple<string, string> GetAssemblyInfo()
        {
            if (string.IsNullOrEmpty(interceptor))
            {
                return null;
            }
            var classInfo = interceptor.Split(',');
            if (classInfo.Length == 0)
                return null;

            var className = classInfo[0];
            if (string.IsNullOrEmpty(className))
                return null;

            string assemblyName = null;
            if (classInfo.Length == 2)
            {
                assemblyName = classInfo[1].Trim();
                if (!assemblyName.Contains(".dll") && !assemblyName.Contains(".exe"))
                {
                    assemblyName = $"{assemblyName}.dll";
                }
            }
            return new Tuple<string, string>(className, assemblyName);
        }
    }
}
