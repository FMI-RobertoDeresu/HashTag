using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HashTag.Infrastructure.RouteConstraints
{
    public class GuidConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (route == null)
                throw new ArgumentNullException(nameof(route));

            if (routeKey == null)
                throw new ArgumentNullException(nameof(routeKey));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            object obj;
            Guid guid;

            if (values.TryGetValue(routeKey, out obj) && obj != null)
                return Guid.TryParse(values[routeKey].ToString(), out guid) && guid != Guid.Empty;

            return false;
        }
    }
}