using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Domain.DependencyInjection
{
    public class ScopedDependencyAttribute : DependencyAttribute
    {
        public ScopedDependencyAttribute() : base(ServiceLifetime.Scoped)
        {
        }
    }
}