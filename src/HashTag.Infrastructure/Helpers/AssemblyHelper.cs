using System.Collections.Generic;
using System.Reflection;

namespace HashTag.Infrastructure.Helpers
{
    public class AssemblyHelper
    {
        private static Assembly[] _assemblies;

        public static IEnumerable<Assembly> GetSolutionAssemblies()
        {
            return _assemblies ?? (_assemblies = new[]
            {
                Assembly.Load(new AssemblyName("HashTag.Application")),
                Assembly.Load(new AssemblyName("HashTag.Contracts")),
                Assembly.Load(new AssemblyName("HashTag.Data")),
                Assembly.Load(new AssemblyName("HashTag.Domain")),
                Assembly.Load(new AssemblyName("HashTag.Infrastructure")),
                Assembly.Load(new AssemblyName("HashTag.Presentation"))
            });
        }
    }
}