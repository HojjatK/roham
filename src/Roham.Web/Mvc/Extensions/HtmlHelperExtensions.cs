using System.Linq.Expressions;
using Roham.Resources;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RequiredLabel(this HtmlHelper html, string expression)
        {
            var labelText = $"{Labels.ResourceManager.GetString(expression) ?? expression} *";
            return html.Label(labelText);
        }

        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var labelText = $"{GetLabelText(expression)} *";
            return html.LabelFor(expression, labelText);
        }

        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var labelText = $"{GetLabelText(expression)} *";
            return html.LabelFor(expression, labelText, htmlAttributes);
        }

        public static MvcHtmlString LabelForExt<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var labelText = GetLabelText(expression);
            return html.LabelFor(expression, labelText);
        }

        public static MvcHtmlString LabelForExt<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var labelText = GetLabelText(expression);
            return html.LabelFor(expression, labelText, htmlAttributes);
        }

        private static string GetLabelText<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {   
            var propInfo = expression.GetPropertyInfo();
            var propName = propInfo.Name;
            return $"{Labels.ResourceManager.GetString(propName) ?? propName}";
        }
    }
}
