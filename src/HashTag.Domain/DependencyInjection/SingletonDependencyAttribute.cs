using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Domain.DependencyInjection
{
    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }
}