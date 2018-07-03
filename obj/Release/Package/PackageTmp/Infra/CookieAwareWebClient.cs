using System;
using System.Net;

namespace Atimo.Infra
{
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer _mContainer;

        public CookieAwareWebClient(CookieContainer container)
        {
            _mContainer = container;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            
            var webRequest = request as HttpWebRequest;

            if (webRequest == null) 
                return request;

            webRequest.CookieContainer = _mContainer;
            webRequest.KeepAlive = true;
            webRequest.ProtocolVersion = HttpVersion.Version10;

            return request;
        }
    }
}
