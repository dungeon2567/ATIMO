using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Helpers.ScriptHtmlHelpers
{
    public static class ScriptHtmlHelperExtensions
    {
        public static void AddScriptBlock(this HtmlHelper htmlHelper, string script)
        {
            ScriptHtmlHelperExtensions.AddToScriptContext(htmlHelper, (ScriptContext context) => context.ScriptBlocks.Add(string.Concat("<script type='text/javascript'>", script, "</script>")));
        }

        public static void AddScriptBlock(this HtmlHelper htmlHelper, Func<dynamic, HelperResult> scriptTemplate)
        {
            ScriptHtmlHelperExtensions.AddToScriptContext(htmlHelper, (ScriptContext context) => context.ScriptBlocks.Add(scriptTemplate(null).ToString()));
        }

        public static void AddScriptFile(this HtmlHelper htmlHelper, string path)
        {
            ScriptHtmlHelperExtensions.AddToScriptContext(htmlHelper, (ScriptContext context) => context.ScriptFiles.Add(path));
        }

        private static void AddToScriptContext(HtmlHelper htmlHelper, Action<ScriptContext> action)
        {
            ScriptContext item = htmlHelper.ViewContext.HttpContext.Items["ScriptContext"] as ScriptContext;
            if (item == null)
            {
                throw new InvalidOperationException("No ScriptContext in HttpContext.Items. Call Html.BeginScriptContext() to create a ScriptContext.");
            }
            action(item);
        }

        public static ScriptContext BeginScriptContext(this HtmlHelper htmlHelper)
        {
            HttpContextBase httpContext = htmlHelper.ViewContext.HttpContext;
            ScriptContext scriptContext = new ScriptContext(httpContext);
            httpContext.Items["ScriptContext"] = scriptContext;
            return scriptContext;
        }

        public static void EndScriptContext(this HtmlHelper htmlHelper)
        {
            ScriptContext item = htmlHelper.ViewContext.HttpContext.Items["ScriptContext"] as ScriptContext;
            if (item != null)
            {
                item.Dispose();
            }
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            return htmlHelper.RenderScripts((string[] paths) => {
                StringBuilder stringBuilder = new StringBuilder((int)paths.Length);
                string[] strArrays = paths;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    stringBuilder.AppendLine(string.Concat("<script type='text/javascript' src='", urlHelper.Content(str), "'></script>"));
                }
                return new HtmlString(stringBuilder.ToString());
            });
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper, Func<string[], IHtmlString> scriptPathResolver)
        {
            Stack<ScriptContext> item = htmlHelper.ViewContext.HttpContext.Items["ScriptContexts"] as Stack<ScriptContext>;
            if (item == null)
            {
                return MvcHtmlString.Empty;
            }
            int count = item.Count;
            StringBuilder stringBuilder = new StringBuilder();
            List<string> strs = new List<string>();
            for (int i = 0; i < count; i++)
            {
                ScriptContext scriptContext = item.Pop();
                stringBuilder.Append(scriptPathResolver(scriptContext.ScriptFiles.ToArray<string>()).ToString());
                strs.AddRange(scriptContext.ScriptBlocks);
                if (i == count - 1 && strs.Any<string>())
                {
                    foreach (string str in strs)
                    {
                        stringBuilder.AppendLine(str);
                    }
                }
            }
            return new HtmlString(stringBuilder.ToString());
        }
    }
}