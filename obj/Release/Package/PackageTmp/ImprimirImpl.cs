using System;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace ATIMO
{
    public static class ImprimirImpl
    {

        sealed class PageEventHandler : PdfPageEventHelper
        {
            public Controller Controller;

            public PageEventHandler(Controller controller)
            {
                Controller = controller;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                var cb = writer.DirectContent;

                Font normalFont = FontFactory.GetFont(FontFactory.TIMES, 9);

                Font boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9);

                float y = document.PageSize.GetBottom(document.BottomMargin);

                cb.MoveTo(document.LeftMargin, y);

                cb.SetColorStroke(new BaseColor(98, 36, 35));

                cb.LineTo(document.PageSize.GetRight(document.RightMargin), y);

                cb.Stroke();

                String[] texts = new String[]
                {
                    "São Bento Engenharia | Tel. +55 11 3331-1897",
                    "Rua Ipojuca, 54 | CEP 03304-050 – São Paulo – SP",
                    "ATENDIMENTO@SAOBENTOSERVICOS.COM.BR",
                    "WWW.SAOBENTOSERVICOS.COM.BR"
                };

                Font[] fonts = new Font[]
                {
                    boldFont,normalFont,boldFont,boldFont
                };

                y -= 10;

                for (Int32 it = 0; it < 4; ++it)
                {
                    String text = texts[it];
                    Font font = fonts[it];

                    y -= 2;

                    cb.BeginText();

                    cb.SetFontAndSize(font.BaseFont, 9);

                    float x = document.LeftMargin + (document.PageSize.GetRight(document.RightMargin) - document.LeftMargin) / 2 - (font.BaseFont.GetWidthPoint(text, 9) / 2);

                    cb.SetTextMatrix(x, y);

                    cb.ShowText(text);

                    cb.EndText();

                    y -= font.Size;
                }

            }

        }

        public static Object GetPDF(this Controller controller, ATIMOEntities _db, int id = 0)
        {
            return null;
        }
    }
}