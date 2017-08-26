using System;
using System.Linq;
using System.Reflection;
using HashTag.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace HashTag.Infrastructure.Attributes
{
    public class ConstrainedRouteAttribute : Attribute, IActionConstraint
    {
        private readonly object _constraints;

        /// <summary>
        ///     Not working!
        /// </summary>
        public ConstrainedRouteAttribute(object constraints)
        {
            _constraints = constraints;
            throw new NotImplementedException();
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var httpContext = context.RouteContext.HttpContext;
            var routeValues = context.RouteContext.RouteData.Values;
            var routeDirection = RouteDirection.IncomingRequest;
            var constraints = _constraints.ToDictionary();

            return constraints
                .Where(constraint => constraint.Value.GetType().GetTypeInfo().ImplementsInterface<IRouteConstraint>())
                .Select(constraint => ((IRouteConstraint) constraint.Value)
                    .Match(httpContext, null, constraint.Key, routeValues, routeDirection))
                .Aggregate(true, (result, next) => result && next);
        }
    }
}