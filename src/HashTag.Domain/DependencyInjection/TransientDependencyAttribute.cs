using Microsoft.Extensions.DependencyInjection;

namespace HashTag.Domain.DependencyInjection
{
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute() : base(ServiceLifetime.Transient) { }
    }
}