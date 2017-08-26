using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HashTag.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static ModelStateDictionary Clear(this ModelStateDictionary modelState, string exceptStartsWith)
        {
            var keys = modelState.Keys.Where(x => !x.StartsWith(exceptStartsWith)).ToList();
            keys.ForEach(key => modelState.ClearKey(key));
            return modelState;
        }

        public static ModelStateDictionary ClearKey(this ModelStateDictionary modelState, string key)
        {
            modelState.ClearValidationState(key);
            modelState.MarkFieldValid(key);
            return modelState;
        }

        public static ModelError FirstError(this ModelStateDictionary modelState)
        {
            var result = modelState.GetErrors().FirstOrDefault();
            return result;
        }

        public static IEnumerable<ModelError> GetErrors(this ModelStateDictionary modelState)
        {
            var result = modelState.SelectMany(x => x.Value.Errors).ToList();
            return result;
        }

        public static IEnumerable<string> GetErrorMessages(this ModelStateDictionary modelState)
        {
            var result = modelState.GetErrors().Select(x => x.ErrorMessage);
            return result;
        }
    }
}