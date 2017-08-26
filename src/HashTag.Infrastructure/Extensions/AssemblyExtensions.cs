using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HashTag.Infrastructure.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Assembly> GetReferencedAssemblies(this Assembly assembly, string prefix)
        {
            return assembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(prefix))
                .Select(Assembly.Load);
        }
    }
}