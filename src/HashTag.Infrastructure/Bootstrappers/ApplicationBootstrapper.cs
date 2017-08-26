using System;
using System.Linq;
using HashTag.Infrastructure.Extensions;
using HashTag.Infrastructure.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Infrastructure.Bootstrappers
{
    public static class ApplicationBootstrapper
    {
        public static void ApplyAllBootstrappers(IServiceCollection serviceCollection)
        {
            var bootrappers = AssemblyHelper
                .GetSolutionAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes
                    .Where(type => type.IsClass)
                    .Where(type => type.ImplementsInterface<IBootstrapper>()))
                .ToList();

            foreach (var type in bootrappers)
            {
                IBootstrapper bootstrapper;

                if (type.ImplementsInterface<IServiceBootstrapper>())
                    bootstrapper = (IBootstrapper) Activator.CreateInstance(type.AsType(), serviceCollection);
                else
                    bootstrapper = (IBootstrapper) Activator.CreateInstance(type.AsType());

                bootstrapper.Apply();
            }
        }
    }
}