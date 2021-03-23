using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Pages.Extensions {
    public static class EditorHtml {

        public static IHtmlContent Editor<TModel, TResult>(
            this IHtmlHelper<TModel> h, Expression<Func<TModel, TResult>> e)
            => Editor(h, e, e);
        public static IHtmlContent Editor<TModel, TResult1, TResult2>(
            this IHtmlHelper<TModel> h, Expression<Func<TModel, TResult1>> label,
             Expression<Func<TModel, TResult2>> value) {
            var n = h.DisplayNameFor(label);
            return Editor(h, value, n);
        }
        public static IHtmlContent Editor<TModel, TResult>(this IHtmlHelper<TModel> h,
            Expression<Func<TModel, TResult>> e, string displayName) {
            var s = htmlStrings(h, e, displayName);
            return new HtmlContentBuilder(s);
        }
        internal static List<object> htmlStrings<TModel, TResult>(IHtmlHelper<TModel> h,
            Expression<Func<TModel, TResult>> e, string displayName) {
            return new List<object> {
                new HtmlString("<dt class=\"col-sm-2\">"),
                h.Raw(displayName),
                new HtmlString("</dt>"),
                new HtmlString("<dd class=\"col-sm-10\">"),
                h.EditorFor(e, new {htmlAttributes = new {@class = "form-control"}}),
                h.ValidationMessageFor(e, "", new {@class = "text-danger"}),
                new HtmlString("</dd>")
            };
        }
    }
}
