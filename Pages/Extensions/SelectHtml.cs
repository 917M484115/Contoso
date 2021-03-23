using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Contoso.Pages.Extensions {
    public static class SelectHtml {
        public static IHtmlContent Select<TModel, TResult>(
            this IHtmlHelper<TModel> h, Expression<Func<TModel, TResult>> e, 
            IEnumerable<SelectListItem> selectList, string optionLabel)
            => Select(h, e, e, selectList, optionLabel);
 
        public static IHtmlContent Select<TModel, TResult1, TResult2>(
            this IHtmlHelper<TModel> h, Expression<Func<TModel, TResult1>> label,
             Expression<Func<TModel, TResult2>> value, IEnumerable<SelectListItem> selectList, string optionLabel) {
            var n = h.DisplayNameFor(label);
            return Select(h, value, selectList, optionLabel, n);
        }
        public static IHtmlContent Select<TModel, TResult>(this IHtmlHelper<TModel> h,
            Expression<Func<TModel, TResult>> e, IEnumerable<SelectListItem> selectList, string optionLabel, string displayName) {
            var s = htmlStrings(h, e, selectList, optionLabel, displayName);
            return new HtmlContentBuilder(s);
        }
        internal static List<object> htmlStrings<TModel, TResult>(IHtmlHelper<TModel> h,
            Expression<Func<TModel, TResult>> e, IEnumerable<SelectListItem> selectList, string optionLabel, string displayName) {
            return new List<object> {
                new HtmlString("<dt class=\"col-sm-2\">"),
                h.Raw(displayName),
                new HtmlString("</dt>"),
                new HtmlString("<dd class=\"col-sm-10\">"),
                h.DropDownListFor(e, selectList, optionLabel, new {@class = "form-control"}),
                h.ValidationMessageFor(e, "", new {@class = "text-danger"}),
                new HtmlString("</dd>")
            };
        }
    }
}
