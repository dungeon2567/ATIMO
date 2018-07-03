using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;
using System.IO;
using System.Globalization;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace Atimo.Controllers
{
    public class FuncionarioCartaoPontoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public ActionResult Create()
        {
            return View(new FUNCIONARIO_CARTAO_PONTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FUNCIONARIO_CARTAO_PONTO fcp)
        {
            if (ModelState.IsValid)
            {
                fcp.FUNCIONARIO = Session.UsuarioId();

                fcp.MOMENTO = DateTime.Now;

                _db.FUNCIONARIO_CARTAO_PONTO.Add(fcp);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "FuncionarioCartaoPonto");
            }
            else
                return View(fcp);
        }

        public async Task<ActionResult> Search(Int32 funcionario, string de = null, string ate = null)
        {
            DateTime dt_start;
            DateTime dt_end;

            if (de == null)
            {
                dt_start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            else
            {
                dt_start = DateTime.Parse(de);
            }

            if (ate == null)
            {
                dt_end = dt_start.AddMonths(1);
            }
            else
            {
                dt_end = DateTime.Parse(ate);
            }

            ViewBag.DE = dt_start.ToString("dd/MM/yyyy");
            ViewBag.ATE = dt_end.ToString("dd/MM/yyyy");

            dt_end = dt_end.AddDays(1);

            Int32 usuario = Session.UsuarioId();

            var cartao_ponto = await _db.FUNCIONARIO_CARTAO_PONTO
                .Where(cp => cp.FUNCIONARIO == funcionario && cp.MOMENTO >= dt_start && cp.MOMENTO < dt_end)
                .OrderBy(cp => cp.MOMENTO)
                .ToArrayAsync();

            return View(new FuncionarioCartaoPontoSearchViewModel() { Funcionario = funcionario, Items = cartao_ponto });
        }



        public async Task<ActionResult> Index(string de = null, string ate = null)
        {
            DateTime dt_start;
            DateTime dt_end;

            if (de == null)
            {
                dt_start = dt_start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            else
            {
                dt_start = DateTime.Parse(de);
            }

            if (ate == null)
            {
                dt_end = dt_start.AddMonths(1);
            }
            else
            {
                dt_end = DateTime.Parse(ate);
            }

            ViewBag.DE = dt_start.ToString("dd/MM/yyyy");
            ViewBag.ATE = dt_end.ToString("dd/MM/yyyy");

            dt_end = dt_end.AddDays(1);


            Int32 usuario = Session.UsuarioId();

            var cartao_ponto = await _db.FUNCIONARIO_CARTAO_PONTO
                .Where(cp => cp.FUNCIONARIO == usuario && cp.MOMENTO >= dt_start && cp.MOMENTO < dt_end)
                .OrderBy(cp => cp.MOMENTO)
                .ToArrayAsync();

            return View(cartao_ponto);
        }


        public async Task<FileContentResult> GetExcelMensal(string mes, Int32 funcionario)
        {
            DateTime initialDate = DateTime.ParseExact(mes, "MM/yyyy", CultureInfo.InvariantCulture);

            DateTime finalDate = initialDate.AddMonths(1);

            var query = (from fcp in _db.FUNCIONARIO_CARTAO_PONTO
                         where fcp.FUNCIONARIO == funcionario && fcp.MOMENTO >= initialDate && fcp.MOMENTO < finalDate
                         let dt = DbFunctions.TruncateTime(fcp.MOMENTO)
                         group fcp by dt into g
                         select g);

            var cartao_ponto = await query.ToDictionaryAsync(fcp => fcp.Key, fcp => fcp);

            DateTime date = initialDate;


            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4, 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 9);
            var redNormalFont = FontFactory.GetFont(FontFactory.TIMES, 9, BaseColor.RED);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 9, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            doc.NewPage();

            var nome = await _db.PESSOA
                .Where(p => p.ID == funcionario)
                .Select(p => p.NOME)
                .FirstOrDefaultAsync();

            doc.Add(new Paragraph(new Chunk("CONTROLE DE PONTO", normalFont)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(new Chunk("NOME: " + nome, normalFont)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(new Chunk(initialDate.ToString("MMMM/yyyy").ToUpper(), normalFont)) { Alignment = Element.ALIGN_CENTER });



            PdfPTable table = new PdfPTable(8)
            {
                WidthPercentage = 100
            };


            table.AddCell(new PdfPCell(new Phrase(new Chunk("DIA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("ENTRADA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("ENTRADA ALMOÇO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("SAÍDA ALMOÇO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("SAÍDA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("H. DIÁRIA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("ATRASO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("H. EXTRA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });

            TimeSpan totalExtra = TimeSpan.Zero;
            TimeSpan totalAtraso = TimeSpan.Zero;
            TimeSpan total = TimeSpan.Zero;


            while (date < finalDate)
            {
                var cp = cartao_ponto.ContainsKey(date) ? cartao_ponto[date] : null;

                table.AddCell(new PdfPCell(new Phrase(new Chunk(date.ToString("dd - ddd").ToUpper(), normalFont))) { HorizontalAlignment = 1 });

                if (cp != null)
                {
                    TimeSpan horasTrabalhadas = default(TimeSpan);

                    switch (cp.Count())
                    {
                        case 2:
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(0).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(1).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk((horasTrabalhadas = (cp.ElementAt(1).MOMENTO.TimeOfDay - cp.ElementAt(0).MOMENTO.TimeOfDay - TimeSpan.FromHours(1))).ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            break;
                        case 4:
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(0).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(1).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(2).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(cp.ElementAt(3).MOMENTO.TimeOfDay.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });

                            table.AddCell(new PdfPCell(new Phrase(new Chunk((horasTrabalhadas = ((cp.ElementAt(1).MOMENTO.TimeOfDay - cp.ElementAt(0).MOMENTO.TimeOfDay) + (cp.ElementAt(3).MOMENTO.TimeOfDay - cp.ElementAt(2).MOMENTO.TimeOfDay))).ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            break;
                        default:
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(TimeSpan.Zero.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                            break;
                    }

                    if (horasTrabalhadas != TimeSpan.Zero)
                    {
                        total += horasTrabalhadas;

                        TimeSpan hExtra = horasTrabalhadas - (TimeSpan.FromHours(8) + TimeSpan.FromMinutes(48));


                        if (hExtra < TimeSpan.Zero)
                        {
                            totalAtraso -= hExtra;

                            table.AddCell(new PdfPCell(new Phrase(new Chunk((-hExtra).ToString(@"hh\:mm\:ss"), redNormalFont))) { HorizontalAlignment = 1 });
                        }
                        else
                        {
                            totalExtra += hExtra;

                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                        }


                        if (hExtra > TimeSpan.Zero)
                        {
                            table.AddCell(new PdfPCell(new Phrase(new Chunk(hExtra.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                        }
                        else
                        {
                            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                        }
                    }
                    else
                    {
                        table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                        table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    }
                }
                else
                {
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(TimeSpan.Zero.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
                }

                date = date.AddDays(1);
            }

            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk($"{Math.Floor(total.TotalHours).ToString("00")}:{total.Minutes.ToString("00")}:{total.Seconds.ToString("00")}", normalFont))) { HorizontalAlignment = 1 });

            if (totalAtraso > TimeSpan.Zero)
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk(totalAtraso.ToString(@"hh\:mm\:ss"), redNormalFont))) { HorizontalAlignment = 1 });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });

            }

            if (totalExtra > TimeSpan.Zero)
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk(totalExtra.ToString(@"hh\:mm\:ss"), normalFont))) { HorizontalAlignment = 1 });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk("", normalFont))) { HorizontalAlignment = 1 });
            }

            table.SpacingBefore = 10;
            table.SpacingAfter = 80;

            doc.Add(table);

            doc.Add(new Chunk(new LineSeparator(0.0F, 45.0F, BaseColor.BLACK, Element.ALIGN_CENTER, 1)));


            doc.Close();

            writer.Close();



            Response.AppendHeader("Content-Disposition", "inline; filename=controle_ponto.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}