using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using HashTag.Domain.AutoMapping;
using HashTag.Infrastructure.Extensions;
using HashTag.Infrastructure.Helpers;

namespace HashTag.Infrastructure.Bootstrappers
{
    internal class AutoMapperBootstrapper : IBootstrapper
    {
        public void Apply()
        {
            var autoMapTypes = AssemblyHelper
                .GetSolutionAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(type => type.ImplementsInterface<IAutoMap>())
                .ToArray();
            var mapperConfiguration = new MapperConfigurationExpression();

            CreateStandardMappings(autoMapTypes, mapperConfiguration);
            CreateCustomMappings(autoMapTypes, mapperConfiguration);

            Mapper.Initialize(mapperConfiguration);
            Mapper.AssertConfigurationIsValid();
        }

        private static void CreateStandardMappings(TypeInfo[] types, MapperConfigurationExpression mapperConfiguration)
        {
            var autoMapsFrom = types
                .Where(type => type.ImplementsGenericInterface(typeof(IMapFrom<>)))
                .Select(type => new
                {
                    Source = type.GetGenericTypeOfImplementedInterface(typeof(IMapFrom<>)),
                    Destination = type.AsType()
                })
                .ToArray();

            var autoMapsTo = types
                .Where(type => type.ImplementsGenericInterface(typeof(IMapTo<>)))
                .Select(type => new
                {
                    Source = type.AsType(),
                    Destination = type.GetGenericTypeOfImplementedInterface(typeof(IMapTo<>))
                })
                .ToArray();

            var autoMapsMutual = types
                .Where(type => type.ImplementsGenericInterface(typeof(IMutualMap<>)))
                .Select(type => new
                {
                    Source = type.AsType(),
                    Destination = type.GetGenericTypeOfImplementedInterface(typeof(IMutualMap<>))
                })
                .SelectMany(map => new[]
                {
                    new { map.Source, map.Destination },
                    new { Source = map.Destination, Destination = map.Source }
                })
                .ToArray();

            var autoMaps = autoMapsFrom.Union(autoMapsTo).Union(autoMapsMutual).Distinct().ToArray();

            foreach (var map in autoMaps)
                mapperConfiguration.CreateMap(map.Source, map.Destination);
        }

        private static void CreateCustomMappings(TypeInfo[] types, MapperConfigurationExpression mapperConfiguration)
        {
            types
                .Where(type => type.ImplementsInterface<IAutoMapCustom>())
                .Select(type => (IAutoMapCustom) Activator.CreateInstance(type.AsType()))
                .ForEach(instance => instance.CreateMappings(mapperConfiguration));
        }
    }
}