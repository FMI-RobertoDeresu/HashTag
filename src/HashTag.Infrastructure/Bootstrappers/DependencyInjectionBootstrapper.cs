using System.Linq;
using System.Reflection;
using HashTag.Domain.DependencyInjection;
using HashTag.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Infrastructure.Bootstrappers
{
    internal class DependencyInjectionBootstrapper : IServiceBootstrapper
    {
        private readonly IServiceCollection _serviceCollection;

        public DependencyInjectionBootstrapper(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public void Apply()
        {
            var dependencyTypes = AssemblyHelper
                .GetSolutionAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => type.GetCustomAttributes<DependencyAttribute>(false).Any());

            foreach (var type in dependencyTypes)
            {
                var dependencyAttributes = type.GetCustomAttributes<DependencyAttribute>();
                foreach (var dependencyAttribute in dependencyAttributes)
                {
                    var serviceDescriptor = dependencyAttribute.BuilServiceDescriptor(type.AsType());
                    _serviceCollection.Add(serviceDescriptor);
                }
            }
        }
    }
}