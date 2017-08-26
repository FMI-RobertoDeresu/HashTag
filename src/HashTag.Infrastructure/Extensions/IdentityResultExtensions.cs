using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace HashTag.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static IEnumerable<string> GetAllErrors(this IdentityResult identityResult)
        {
            var errors = identityResult.Errors.Select(x => $"{x.Code}. ${x.Description}");

            return errors;
        }
    }
}