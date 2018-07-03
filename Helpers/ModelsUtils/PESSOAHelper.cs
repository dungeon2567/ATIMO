using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ATIMO.Helpers.ModelsUtils
{
    public static class PESSOAHelper
    {
        public static void IsAuthenticated(this HtmlHelper htmlHelper)
        {
            var usuario = HttpContext.Current.Session["USUARIO"] as ATIMO.Models.PESSOA;

            if (usuario == null)
            {
                var context = new RequestContext( new HttpContextWrapper(System.Web.HttpContext.Current), new RouteData());
                var urlHelper = new UrlHelper(context);
                var url = urlHelper.Action("Index", "Home");
                System.Web.HttpContext.Current.Response.Redirect(url);
            }
        }
    }
}