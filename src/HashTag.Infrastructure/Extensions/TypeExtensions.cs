using System;
using System.Linq;
using System.Reflection;

namespace HashTag.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface<T>(this TypeInfo typeInfo)
        {
            return typeInfo.GetInterfaces()
                .Any(implementedType => implementedType == typeof(T));
        }

        public static bool ImplementsGenericInterface(this TypeInfo typeInfo, Type interfaceType)
        {
            return typeInfo.GetInterfaces()
                .Where(implementedInterface => implementedInterface.IsConstructedGenericType)
                .Any(implementedInterface => implementedInterface.GetGenericTypeDefinition() == interfaceType);
        }

        public static Type GetGenericTypeOfImplementedInterface(this TypeInfo typeInfo, Type interfaceType)
        {
            return typeInfo
                .GetInterfaces()
                .First(implementedType => implementedType.GetGenericTypeDefinition() == interfaceType)
                .GenericTypeArguments[0];
        }
    }
}