using System;
using HashTag.Contracts.Services;
using HashTag.Domain.DependencyInjection;

namespace HashTag.Application.Services
{
    [SingletonDependency(ServiceType = typeof(IRandomService))]
    public class RandomService : IRandomService
    {
        private readonly Random _random;

        public RandomService()
        {
            _random = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
        }

        public int Get()
        {
            return _random.Next();
        }
    }
}