using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace Helpers.ScriptHtmlHelpers
{
    public class ScriptContext : IDisposable
    {
        internal const string ScriptContextItem = "ScriptContext";

        internal const string ScriptContextItems = "ScriptContexts";

        private readonly HttpContextBase _httpContext;

        private readonly IList<string> _scriptBlocks = new List<string>();

        private readonly HashSet<string> _scriptFiles = new HashSet<string>();

        public IList<string> ScriptBlocks
        {
            get
            {
                return this._scriptBlocks;
            }
        }

        public HashSet<string> ScriptFiles
        {
            get
            {
                return this._scriptFiles;
            }
        }

        public ScriptContext(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            this._httpContext = httpContext;
        }

        public void Dispose()
        {
            IDictionary items = this._httpContext.Items;
            Stack<ScriptContext> item = items["ScriptContexts"] as Stack<ScriptContext> ?? new Stack<ScriptContext>();
            foreach (ScriptContext scriptContext in item)
            {
                scriptContext.ScriptFiles.ExceptWith(this.ScriptFiles);
            }
            item.Push(this);
            items["ScriptContexts"] = item;
        }
    }
}