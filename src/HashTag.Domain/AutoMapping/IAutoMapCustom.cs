using AutoMapper;

namespace HashTag.Domain.AutoMapping
{
    public interface IAutoMapCustom : IAutoMap
    {
        void CreateMappings(IMapperConfigurationExpression configuration);
    }
}