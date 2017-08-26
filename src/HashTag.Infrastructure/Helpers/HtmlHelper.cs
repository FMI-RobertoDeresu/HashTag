using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HashTag.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HashTag.Infrastructure.Helpers
{
    public static class HtmlHelper
    {
        public static Task RenderPartialAsync<TModel, TProp>(this IHtmlHelper htmlHelper, string partialView,
            TModel currentModel, Expression<Func<TModel, TProp>> prefixedModel)
        {
            var viewData = new ViewDataDictionary(htmlHelper.ViewData);
            var htmlPrefix = viewData.TemplateInfo.HtmlFieldPrefix;
            var prefixStr = prefixedModel.FullName();
            viewData.TemplateInfo.HtmlFieldPrefix += Equals(htmlPrefix, string.Empty) ? prefixStr : $".{prefixStr}";
            return htmlHelper.RenderPartialAsync(partialView, prefixedModel.Compile().Invoke(currentModel), viewData);
        }
    }
}