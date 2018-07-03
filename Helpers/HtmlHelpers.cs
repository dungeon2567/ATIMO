using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace ATIMO.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString GraficoQuantidadeOs(this HtmlHelper html,
            String nome,
            String titulo,
                                 int finalizadas, int pendentes,
                                          int width,
                                          int height)
        { 
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<div id={0}></div>", nome);
            sb.AppendLine("<script language='javascript'>");
            sb.AppendLine("google.load('visualization', '1.0', { 'packages': ['corechart'] });");
            sb.AppendLine("google.setOnLoadCallback(drawChart);");
            sb.AppendLine("function drawChart() {");
            sb.AppendLine("var data = new google.visualization.arrayToDataTable([");

            sb.AppendLine(String.Format("['{0}', {1}],","PENDENTES", pendentes));
            sb.AppendLine(String.Format("['{0}', {1}],", "FINALIZADAS", finalizadas));

            sb.AppendLine("]);");

            sb.AppendLine(String.Format("var options = {{ 'title': '{0}',", titulo));
            sb.AppendLine(String.Format("   'pieHole': '{0}',", 0.4));
            sb.AppendLine(String.Format("   'width': '{0}',", width));
            sb.AppendLine(String.Format("   'height': '{0}' ", height));
            sb.AppendLine("   }");
            sb.AppendLine(String.Format("var chart = new google.visualization.PieChart(document.getElementById('{0}'));", nome));
            sb.AppendLine("chart.draw(data, options);");
            sb.AppendLine("}");
            sb.AppendLine("</script>");
            return new MvcHtmlString(sb.ToString());
        }
    }
}