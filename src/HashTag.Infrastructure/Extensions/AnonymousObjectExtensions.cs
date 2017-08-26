using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HashTag.Infrastructure.Extensions
{
    public static class AnonymousObjectExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            ValidateAnonymous(obj);

            var type = obj.GetType();
            var props = type.GetProperties();

            return props.ToDictionary(x => x.Name, x => x.GetValue(obj, null));
        }

        private static void ValidateAnonymous(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();
            if (type.Name.Contains("AnonymousType") && type.Name.StartsWith("<>"))
                throw new Exception("Not an anonymous type!");
        }
    }
}