using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Contoso.Pages.Extensions {
    public static class ShowTableHtml {
        public static IHtmlContent ShowTable<TModel, TResult>(
            this IHtmlHelper<TModel> h, Expression<Func<TModel, IEnumerable<TResult>>> e,
            params string[] propertyNames 
        ) => ShowTable(h, e, e, propertyNames);


        public static IHtmlContent ShowTable<TModel, TResult1, TResult2>(
            this IHtmlHelper<TModel> h,
            Expression<Func<TModel, IEnumerable<TResult1>>> label,
            Expression<Func<TModel, IEnumerable<TResult2>>> value,
            params string[] propertyNames) {
            var labelStr = h.DisplayNameFor(label);
            IEnumerable<dynamic> v = (value is null) ? getValue(h, label) : getValue(h, value);
            return h.ShowTable(labelStr, v, propertyNames);
        }

        private static IEnumerable<dynamic> getValue<TModel, TResult>(IHtmlHelper<TModel> h, 
            Expression<Func<TModel, IEnumerable<TResult>>> value) {
            var r = value.Compile();
            var v = r.Invoke(h.ViewData.Model);
            var l = v as IEnumerable<dynamic>;
            return l;
        }

        public static IHtmlContent ShowTable<TModel, TClass>(this IHtmlHelper<TModel> h, string label
            , IEnumerable<TClass> list, params string[] propertyNames) {
            if (h == null) throw new ArgumentNullException(nameof(h));
            var s = htmlStrings(h, label, list, propertyNames);
            return new HtmlContentBuilder(s);
        }

        internal static List<object> htmlStrings<TModel, TClass>(
            IHtmlHelper<TModel> h, string label, IEnumerable<TClass> list, string[] propertyNames) {
            list ??= new List<TClass>();
            var l = new List<object> {
                new HtmlString("<dt class=\"col-sm-2\">"),
                h.Raw(label),
                new HtmlString("</dt>"),
                new HtmlString("<dd class=\"col-sm-10\">"),
                new HtmlString("<table class=\"table\">"),
                new HtmlString("<tr>")
            };
            foreach (var item in list) {
                foreach (var n in propertyNames) {
                    l.Add(new HtmlString($"<th>{getDisplayName(item, n)}</th>"));
                }
                break;
            }
            l.Add(new HtmlString("</tr>"));
            foreach (var item in list) {
                l.Add(new HtmlString("<tr>"));
                foreach (var n in propertyNames) {
                    l.Add(new HtmlString($"<td>{getValue(item, n)}</td>"));
                }
                l.Add(new HtmlString("</tr>"));
            }
            l.Add(new HtmlString("</table>"));
            l.Add(new HtmlString("</dd>"));
            return l;
        }
        internal static string getValue(dynamic item, string propertyName) {
            try {
                var name = propertyName ?? string.Empty;
                var p = item.GetType().GetProperty(name);
                var l = p?.GetCustomAttributes(typeof(DisplayFormatAttribute), true);
                var def = (l?.Length == 1) ? l[0].NullDisplayText : null;
                var v = p?.GetValue(item)?? def;
                return v?.ToString();
            } catch (Exception) {
                return string.Empty;
            }
        }
        internal static string getDisplayName(dynamic item, string propertyName) {
            try {
                var name = propertyName ?? string.Empty;
                var p = item.GetType().GetProperty(name);
                var l = p?.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                if (l is null || l.Length < 1) return name;
                return l[0]?.DisplayName ?? name;
            } catch (Exception) {
                return string.Empty;
            }
        }
    }
}
