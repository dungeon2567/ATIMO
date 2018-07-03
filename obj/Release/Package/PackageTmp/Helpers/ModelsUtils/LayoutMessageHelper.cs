using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ATIMO.Helpers.ModelsUtils
{
    public static class LayoutMessageHelper
    {
        private const string LAYOUT_MESSAGE_SESSION_KEY = "MAIN_LAYOUT_MESSAGE";

        public static void SetMessage(string a_Message, LayoutMessageType a_MessageType)
        {
            LayoutMessage l_LayoutMessage = new LayoutMessage(a_Message, a_MessageType);

            try
            {
                HttpContext.Current.Session.Add(LAYOUT_MESSAGE_SESSION_KEY, l_LayoutMessage);
            }
            catch
            {
            }
        }

        public static LayoutMessage GetMessage()
        {
            LayoutMessage l_LayoutMessage = new LayoutMessage();
            if (m_LayoutMessageOverride != null)
                l_LayoutMessage = m_LayoutMessageOverride;
            else
            {
                if (HttpContext.Current.Session[LAYOUT_MESSAGE_SESSION_KEY] != null)
                    l_LayoutMessage = (LayoutMessage)HttpContext.Current.Session[LAYOUT_MESSAGE_SESSION_KEY];
                else
                    l_LayoutMessage = null;
            }

            return l_LayoutMessage;

        }

        public static void ClearMessage()
        {
            try
            {
                HttpContext.Current.Session[LAYOUT_MESSAGE_SESSION_KEY] = null;
            }
            catch (Exception Error)
            {

            }
        }

        public static bool HasMessage()
        {
            try
            {
                return GetMessage() != null;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        private static LayoutMessage m_LayoutMessageOverride = null;

        public static LayoutMessage LayoutMessageOverride
        {
            set { m_LayoutMessageOverride = value; }
            get { return m_LayoutMessageOverride; }
        }

        public static MvcHtmlString LayoutMessage(this HtmlHelper htmlHelper)
        {
            MvcHtmlString mvcHtmlString;
            try
            {
                mvcHtmlString = MvcHtmlString.Create(htmlHelper.BuildLayoutMessageHtml());

                ClearMessage();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return mvcHtmlString;
        }

        private static string BuildLayoutMessageHtml(this HtmlHelper htmlHelper)
        {
            try
            {
                string l_HTML = string.Empty;

                if (LayoutMessageHelper.HasMessage())
                {

                    l_HTML = "<div class=\"alert " + GetMessage().MessageTypeCssClass + "\">" + Environment.NewLine;
                    l_HTML += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button>" + Environment.NewLine;
                    l_HTML += "<span>" + GetMessage().Message + "</span> " + Environment.NewLine;
                    l_HTML += "</div>" + Environment.NewLine;
                }

                return l_HTML;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
    }

    public class LayoutMessage
    {
        public LayoutMessage() { }
        public LayoutMessage(string a_Message, LayoutMessageType a_MessageType)
        {
            this.Message = a_Message;
            this.MessageType = a_MessageType;
        }
        public string Message { get; set; }
        public LayoutMessageType MessageType { get; set; }
        public string MessageTypeCssClass
        {
            get
            {
                string l_MessageTypeClass = string.Empty;
                switch (MessageType)
                {
                    case LayoutMessageType.Success:
                        l_MessageTypeClass = "alert-success";
                        break;
                    case LayoutMessageType.Information:
                        l_MessageTypeClass = "alert-info";
                        break;
                    case LayoutMessageType.Error:
                        l_MessageTypeClass = "alert-error";
                        break;
                    case LayoutMessageType.Alert:
                        l_MessageTypeClass = "alert-warning";
                        break;
                    default:
                        break;
                }
                return l_MessageTypeClass;
            }
        }
    }

    public enum LayoutMessageType
    {
        Success,
        Information,
        Error,
        Alert
    }
}