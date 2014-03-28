using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AtsAPCC.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
    where TEnum : struct
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var items =
                values.Select(value => new SelectListItem
                {
                    Text = value.GetDescription(),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                });
            return htmlHelper.DropDownListFor(expression, items);
        }

        public static MvcHtmlString SubmitButton(this HtmlHelper htmlHelper, string text,
            string cssClass = "iebutton",
            bool disableOnClick = true,
            HttpContextBase context = null)
        {
            var sb = new StringBuilder();

            if (context == null) context = new HttpContextWrapper(HttpContext.Current);
            var isIe8OrLess = (context.Request.Browser.Browser == "IE") && (context.Request.Browser.MajorVersion <= 8);
            if (isIe8OrLess)
            {
                sb.Append(@"<div class=""rounded_ie");
                if (!string.IsNullOrEmpty(cssClass))
                {
                    sb.Append(@" ");
                    sb.Append(cssClass);
                }
                sb.Append(@""">");
                sb.Append(@"<div class=""tl""></div><div class=""tr""></div>");
            }

            sb.Append(@"<input type=""submit"" value=""");
            sb.Append(text);
            sb.Append(@""" ");
            if (disableOnClick)
            {
                sb.Append(@"onclick=""this.disabled=true;this.form.submit();"" ");
            }
            if (!isIe8OrLess && !string.IsNullOrEmpty(cssClass))
            {
                sb.Append(@"class=""");
                sb.Append(cssClass);
                sb.Append(@""" ");
            }
            sb.Append(@"/>");

            if (isIe8OrLess)
            {
                sb.Append(@"<div class=""bl""></div><div class=""br""></div>");
                sb.Append(@"</div>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ActionLink(this HtmlHelper html, string linkText, string actionName, string controllerName, ViewContext viewContext)
        {
            object htmlAttributes = new { };
            if (actionName == (string)viewContext.RouteData.Values["action"]
                && controllerName == (string)viewContext.RouteData.Values["controller"])
            {
                htmlAttributes = new { @class = "current" };
            }
            return html.ActionLink(linkText, actionName, controllerName, null, htmlAttributes);
        }

        public static string EnumDisplayName(this Enum value)
        {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null)
            {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

    }
}