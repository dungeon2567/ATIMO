using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Charting;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace Atimo.Controllers
{
    public class RelatorioGerencialController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public FileContentResult GetRelatorioPDF(int cliente, String de = null, String ate = null)
        {
            IQueryable<OSSB> queryObject = _db.OSSB;

            queryObject = from os in queryObject
                          where os.CLIENTE == cliente
                          select os;

            if (de != null)
            {
                DateTime deDate = DateTime.ParseExact(de, "dd/MM/yyyy", null);

                if (ate != null)
                {

                    DateTime ateDate = DateTime.ParseExact(ate, "dd/MM/yyyy", null);

                    queryObject = from os in queryObject
                                  where _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO >= deDate && oscl.AGENDADO <= ateDate).Any()
                                  select os;
                }
                else
                {

                    queryObject = from os in queryObject
                                  where _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO >= deDate).Any()
                                  select os;
                }
            }
            else
          if (ate != null)
            {
                DateTime ateDate = DateTime.ParseExact(ate, "dd/MM/yyyy", null);

                queryObject = from os in queryObject
                              where _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO <= ateDate).Any()
                              select os;
            }

            var fs = new MemoryStream();

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();



            XFont titleFont = new XFont("Times New Roman", 20, XFontStyle.Bold);

            XFont normalFont = new XFont("Times New Roman", 14, XFontStyle.Bold);

            ChartFrame cf = new ChartFrame()
            {
                Location = new XPoint(page.Width.Point * .05, 60),
                Size = new XSize(page.Width.Point * .9, page.Width.Point * 0.7)
            };

            Chart chart = new Chart(ChartType.Pie2D);

            Series series = chart.SeriesCollection.AddSeries();

            var queryPreventiva = (from os in queryObject
                                   where os.TIPO == "P"
                                   group os by
                                     (os.SITUACAO == "P" || os.SITUACAO == "K" || os.SITUACAO == "F") ? 0 : 1 into g
                                   select new
                                   {
                                       TIPO = g.Key,
                                       QUANTIDADE = g.Count()
                                   }).ToDictionary(en => en.TIPO, en => en.QUANTIDADE);


            series.Add(new double[] { queryPreventiva.GetValueOrDefault(0, 0), queryPreventiva.GetValueOrDefault(1, 0) });

            XSeries xseries = chart.XValues.AddXSeries();
            xseries.Add("Realizadas", "Pendentes");

            chart.DataLabel.Type = DataLabelType.Value;

            chart.HasDataLabel = true;

            chart.Legend.Docking = DockingType.Left;

            chart.DataLabel.Position = DataLabelPosition.OutsideEnd;

            cf.Add(chart);

            XGraphics gfx = XGraphics.FromPdfPage(page);

            gfx.DrawRectangle(XPens.Black, new XRect(150, 600, (page.Width.Point - 300) * 0.7, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150, 630, (page.Width.Point - 300) * 0.7, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150, 660, (page.Width.Point - 300) * 0.7, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150, 690, (page.Width.Point - 300) * 0.7, 30));



            gfx.DrawRectangle(XPens.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 600, (page.Width.Point - 300) * 0.3, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 630, (page.Width.Point - 300) * 0.3, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 660, (page.Width.Point - 300) * 0.3, 30));

            gfx.DrawRectangle(XPens.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 690, (page.Width.Point - 300) * 0.3, 30));

            gfx.DrawString("Mês", normalFont, XBrushes.Black, new XRect(150, 600, (page.Width.Point - 300) * 0.7, 30), XStringFormats.Center);
            gfx.DrawString("Previstas", normalFont, XBrushes.Black, new XRect(150, 630, (page.Width.Point - 300) * 0.7, 30), XStringFormats.Center);
            gfx.DrawString("Realizadas", normalFont, XBrushes.Black, new XRect(150, 660, (page.Width.Point - 300) * 0.7, 30), XStringFormats.Center);
            gfx.DrawString("Pendentes", normalFont, XBrushes.Black, new XRect(150, 690, (page.Width.Point - 300) * 0.7, 30), XStringFormats.Center);

            gfx.DrawString((queryPreventiva.GetValueOrDefault(0, 0) + queryPreventiva.GetValueOrDefault(1, 0)).ToString(), normalFont, XBrushes.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 630, (page.Width.Point - 300) * 0.3, 30), XStringFormats.Center);
            gfx.DrawString(queryPreventiva.GetValueOrDefault(0, 0).ToString(), normalFont, XBrushes.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 660, (page.Width.Point - 300) * 0.3, 30), XStringFormats.Center);
            gfx.DrawString((queryPreventiva.GetValueOrDefault(1, 0)).ToString(), normalFont, XBrushes.Black, new XRect(150 + (page.Width.Point - 300) * 0.7, 690, (page.Width.Point - 300) * 0.3, 30), XStringFormats.Center);



            gfx.DrawString("1. Manutenção Preventiva.", titleFont, XBrushes.Black, new XPoint(10, 25));

            cf.Draw(gfx);

            page = document.AddPage();

            gfx = XGraphics.FromPdfPage(page);

            cf = new ChartFrame()
            {
                Location = new XPoint(page.Width.Point * .05, 60),
                Size = new XSize(page.Width.Point * .9, page.Width.Point * 0.7)
            };


            chart = new Chart(ChartType.Pie2D);

            var queryCorretiva = (from os in queryObject
                                  where os.TIPO == "C"
                                  select os)
                                .GroupBy(os => (os.SITUACAO == "P" || os.SITUACAO == "K" || os.SITUACAO == "F") ? 0 : (os.SITUACAO == "E" ? 1 : 2))
                               .Select(g => new { TIPO = g.Key, QUANTIDADE = g.Count() })
                               .ToDictionary(os => os.TIPO, os => os.QUANTIDADE);

            xseries = chart.XValues.AddXSeries();

            series = chart.SeriesCollection.AddSeries();

            if (queryCorretiva.ContainsKey(0))
            {
                xseries.Add("Executando");
                series.Add(queryCorretiva[0]);
            }

            if (queryCorretiva.ContainsKey(1))
            {
                xseries.Add("Realizadas");
                series.Add(queryCorretiva[1]);
            }

            if (queryCorretiva.ContainsKey(2))
            {
                xseries.Add("Emergênciais");
                series.Add(queryCorretiva[2]);
            }




            chart.DataLabel.Type = DataLabelType.Value;

            chart.HasDataLabel = true;

            chart.Legend.Docking = DockingType.Left;

            chart.DataLabel.Position = DataLabelPosition.OutsideEnd;

            cf.Add(chart);


            cf.Draw(gfx);

            document.Save(fs, false);

            Response.AppendHeader("Content-Disposition", "inline; filename=relatorio.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        public async Task<ActionResult> Search()
        {

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.ToArrayAsync(), "ID", "DESCRICAO");

            return View();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}