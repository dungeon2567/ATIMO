using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ATIMO.Helpers
{
    public static class ViewContextExtensions
    {
        public static RouteValueDictionary ToRouteValues(this NameValueCollection queryString)
        {
            if (queryString == null || queryString.HasKeys() == false) return new RouteValueDictionary();

            var routeValues = new RouteValueDictionary();

            foreach (string key in queryString.AllKeys)
                routeValues.Add(key, queryString[key]);

            return routeValues;
        }

        /// <summary>
        /// Builds a RouteValueDictionary that combines the request route values, the querystring parameters,
        /// and the passed newRouteValues. Values from newRouteValues override request route values and querystring
        /// parameters having the same key.
        /// </summary>
        public static RouteValueDictionary GetCombinedRouteValues(this ViewContext viewContext, object newRouteValues)
        {
            RouteValueDictionary combinedRouteValues = new RouteValueDictionary(viewContext.RouteData.Values);

            NameValueCollection queryString = viewContext.RequestContext.HttpContext.Request.QueryString;
            foreach (string key in queryString.AllKeys.Where(key => key != null))
                combinedRouteValues[key] = queryString[key];

            if (newRouteValues != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(newRouteValues))
                    combinedRouteValues[descriptor.Name] = descriptor.GetValue(newRouteValues);
            }

            return combinedRouteValues;
        }
    }
}