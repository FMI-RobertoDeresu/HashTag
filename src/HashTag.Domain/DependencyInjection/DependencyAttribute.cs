using System;
using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Domain.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DependencyAttribute : Attribute
    {
        protected DependencyAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        private ServiceLifetime Lifetime { get; }

        public Type ServiceType { get; set; }

        public ServiceDescriptor BuilServiceDescriptor(Type implementationType)
        {
            var serviceType = ServiceType ?? implementationType;
            return new ServiceDescriptor(serviceType, implementationType, Lifetime);
        }
    }
}