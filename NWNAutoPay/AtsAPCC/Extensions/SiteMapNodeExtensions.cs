using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtsAPCC.Extensions
{
    public static class SiteMapNodeExtensions
    {
        const string CSS_CURRENT = "current";

        public static string CssClassForCurrent(this SiteMapNode potentialAncestor, HttpSessionStateBase session)
        {
            if (potentialAncestor.IsCurrent(session))
                return CSS_CURRENT;
            return string.Empty;
        }

        public static bool IsCurrent(this SiteMapNode potentialAncestor, HttpSessionStateBase session)
        {
            return potentialAncestor.Title == "Customer Service" || potentialAncestor.Title == "Pay Your Bill";
        }
    }
}