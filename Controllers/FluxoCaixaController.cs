using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using ATIMO;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Atimo.Controllers
{
    public class FluxoCaixaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Search()
        {

                return View();

        }

        public async Task<ActionResult> Index(String de = null, String ate = null, int? projeto = null, int[] criticidade = null)
        {


            IQueryable<CONTAS_RECEBER> receberPrevistoQuery = _db.CONTAS_RECEBER;

            IQueryable<CONTAS_RECEBER> receberRealizadoQuery = _db.CONTAS_RECEBER
                .Where(pr => pr.DATA_RECEBIMENTO != null);

            IQueryable<PAGAMENTO> pagamentoPrevistoQuery = _db.PAGAMENTO
                .Where(pr => pr.DATA_PAGAMENTO == null);

            IQueryable<PAGAMENTO> pagamentoRealizadoQuery = _db.PAGAMENTO
                .Where(pr => pr.DATA_PAGAMENTO != null);

            IQueryable<CAIXINHA> caixinhaQuery = _db.CAIXINHA;

            DateTime deDate;
            DateTime ateDate;


            if (String.IsNullOrEmpty(de) || !DateTime.TryParse(de, out deDate))
            {
                deDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (String.IsNullOrEmpty(ate) || !DateTime.TryParse(ate, out ateDate))
            {
                ateDate = deDate.AddMonths(1).AddDays(-1);
            }


            pagamentoPrevistoQuery = from p in pagamentoPrevistoQuery
                                     where p.DATA_VENCIMENTO >= deDate && p.DATA_VENCIMENTO <= ateDate
                             select p;

            pagamentoRealizadoQuery = from p in pagamentoRealizadoQuery
                                      where p.DATA_PAGAMENTO >= deDate && p.DATA_PAGAMENTO <= ateDate
                                      select p;

            receberPrevistoQuery = from r in receberPrevistoQuery
                                   where r.DATA_VENCIMENTO >= deDate && r.DATA_VENCIMENTO <= ateDate
                                   select r;

            receberRealizadoQuery = from r in receberRealizadoQuery
                                    where r.DATA_RECEBIMENTO >= deDate && r.DATA_RECEBIMENTO <= ateDate
                                    select r;

            caixinhaQuery = from c in caixinhaQuery
                            where c.DATA_ENTREGA >= deDate && c.DATA_ENTREGA <= ateDate
                            select c;

            if(projeto != null)
            {
                pagamentoPrevistoQuery = pagamentoPrevistoQuery.Where(p => p.PROJETO == projeto);

                pagamentoRealizadoQuery = pagamentoRealizadoQuery.Where(p => p.PROJETO == projeto);

                receberPrevistoQuery = receberPrevistoQuery.Where(r => r.OSSB1.PROJETO == projeto);

                receberRealizadoQuery = receberRealizadoQuery.Where(r => r.OSSB1.PROJETO == projeto);

                caixinhaQuery = caixinhaQuery.Where(cq => cq.PESSOA1.PROJETO == projeto);
            }

            if(criticidade != null)
            {
                pagamentoPrevistoQuery = pagamentoPrevistoQuery.Where(p => criticidade.Contains(p.CRITICIDADE));

                pagamentoRealizadoQuery = pagamentoRealizadoQuery.Where(p => criticidade.Contains(p.CRITICIDADE));
            }


            var items = from p in (from p in pagamentoPrevistoQuery
                                   select new
                                   {
                                       DATA = (DateTime)p.DATA_VENCIMENTO,
                                       SAIDA_PREVISTO = p.VALOR,
                                       SAIDA_REALIZADO = 0m,
                                       ENTRADA_PREVISTO = 0m,
                                       ENTRADA_REALIZADO = 0m,
                                   })
                                                            .Concat(from p in pagamentoRealizadoQuery
                                                                    select new
                                                                    {
                                                                        DATA = (DateTime)p.DATA_PAGAMENTO,
                                                                        SAIDA_PREVISTO = 0m,
                                                                        SAIDA_REALIZADO = p.VALOR,
                                                                        ENTRADA_PREVISTO = 0m,
                                                                        ENTRADA_REALIZADO = 0m,
                                                                    }).Concat(from r in receberPrevistoQuery
                                                                              select new
                                                                              {
                                                                                  DATA = r.DATA_VENCIMENTO,
                                                                                  SAIDA_PREVISTO = 0m,
                                                                                  SAIDA_REALIZADO = 0m,
                                                                                  ENTRADA_PREVISTO = r.VALOR_LIQUIDO,
                                                                                  ENTRADA_REALIZADO = 0m,
                                                                              }).Concat(from r in receberRealizadoQuery
                                                                                        select new
                                                                                        {
                                                                                            DATA = (DateTime)r.DATA_RECEBIMENTO,
                                                                                            SAIDA_PREVISTO = 0m,
                                                                                            SAIDA_REALIZADO = 0m,
                                                                                            ENTRADA_PREVISTO = 0m,
                                                                                            ENTRADA_REALIZADO = r.VALOR_LIQUIDO,
                                                                                        })
                                                                                   .Concat(from c in caixinhaQuery
                                                                                           select new
                                                                                           {
                                                                                               DATA = (DateTime)c.DATA_ENTREGA,
                                                                                               SAIDA_PREVISTO = c.VALOR,
                                                                                               SAIDA_REALIZADO = c.VALOR,
                                                                                               ENTRADA_PREVISTO = 0m,
                                                                                               ENTRADA_REALIZADO = 0m,
                                                                                           })
                        group p by p.DATA into g
                        select new FluxoCaixaModel()
                        {
                            Data = g.Key,

                            SaidaPrevisto = g.Sum(p => p.SAIDA_PREVISTO),
                            SaidaRealizado = g.Sum(p => p.SAIDA_REALIZADO),

                            EntradaPrevisto = g.Sum(p => p.ENTRADA_PREVISTO),
                            EntradaRealizado = g.Sum(p => p.ENTRADA_REALIZADO),
                        };
            ViewBag.DE = deDate.ToString("dd/MM/yyyy");
            ViewBag.ATE = ateDate.ToString("dd/MM/yyyy");


            ViewBag.PROJETO = new SelectList(await _db.PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO");


            return View(await items.ToArrayAsync());
        }


        public async Task<ActionResult> Imprimir(String de = null, String ate = null, int? projeto = null, int[] criticidade = null)
        {
            IQueryable<CONTAS_RECEBER> receberPrevistoQuery = _db.CONTAS_RECEBER;

            IQueryable<CONTAS_RECEBER> receberRealizadoQuery = _db.CONTAS_RECEBER
                .Where(pr => pr.DATA_RECEBIMENTO != null);

            IQueryable<PAGAMENTO> pagamentoPrevistoQuery = _db.PAGAMENTO
                .Where(pr => pr.DATA_PAGAMENTO == null);

            IQueryable<PAGAMENTO> pagamentoRealizadoQuery = _db.PAGAMENTO
                .Where(pr => pr.DATA_PAGAMENTO != null);

            IQueryable<CAIXINHA> caixinhaQuery = _db.CAIXINHA;

            DateTime deDate;
            DateTime ateDate;


            if (String.IsNullOrEmpty(de) || !DateTime.TryParse(de, out deDate))
            {
                deDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (String.IsNullOrEmpty(ate) || !DateTime.TryParse(ate, out ateDate))
            {
                ateDate = deDate.AddMonths(1).AddDays(-1);
            }


            pagamentoPrevistoQuery = from p in pagamentoPrevistoQuery
                                     where p.DATA_VENCIMENTO >= deDate && p.DATA_VENCIMENTO <= ateDate
                                     select p;

            pagamentoRealizadoQuery = from p in pagamentoRealizadoQuery
                                      where p.DATA_PAGAMENTO >= deDate && p.DATA_PAGAMENTO <= ateDate
                                      select p;

            receberPrevistoQuery = from r in receberPrevistoQuery
                                   where r.DATA_VENCIMENTO >= deDate && r.DATA_VENCIMENTO <= ateDate
                                   select r;

            receberRealizadoQuery = from r in receberRealizadoQuery
                                    where r.DATA_RECEBIMENTO >= deDate && r.DATA_RECEBIMENTO <= ateDate
                                    select r;

            caixinhaQuery = from c in caixinhaQuery
                            where c.DATA_ENTREGA >= deDate && c.DATA_ENTREGA <= ateDate
                            select c;

            if (projeto != null)
            {
                pagamentoPrevistoQuery = pagamentoPrevistoQuery.Where(p => p.PROJETO == projeto);

                pagamentoRealizadoQuery = pagamentoRealizadoQuery.Where(p => p.PROJETO == projeto);

                receberPrevistoQuery = receberPrevistoQuery.Where(r => r.OSSB1.PROJETO == projeto);

                receberRealizadoQuery = receberRealizadoQuery.Where(r => r.OSSB1.PROJETO == projeto);

                caixinhaQuery = caixinhaQuery.Where(cq => cq.PESSOA1.PROJETO == projeto);
            }

            if (criticidade != null)
            {
                pagamentoPrevistoQuery = pagamentoPrevistoQuery.Where(p => criticidade.Contains(p.CRITICIDADE));

                pagamentoRealizadoQuery = pagamentoRealizadoQuery.Where(p => criticidade.Contains(p.CRITICIDADE));
            }


            var items = await (from p in (from p in pagamentoPrevistoQuery
                                          select new
                                          {
                                              DATA = (DateTime)p.DATA_VENCIMENTO,
                                              SAIDA_PREVISTO = p.VALOR,
                                              SAIDA_REALIZADO = 0m,
                                              ENTRADA_PREVISTO = 0m,
                                              ENTRADA_REALIZADO = 0m,
                                          })
                                                            .Concat(from p in pagamentoRealizadoQuery
                                                                    select new
                                                                    {
                                                                        DATA = (DateTime)p.DATA_PAGAMENTO,
                                                                        SAIDA_PREVISTO = 0m,
                                                                        SAIDA_REALIZADO = p.VALOR,
                                                                        ENTRADA_PREVISTO = 0m,
                                                                        ENTRADA_REALIZADO = 0m,
                                                                    }).Concat(from r in receberPrevistoQuery
                                                                              select new
                                                                              {
                                                                                  DATA = r.DATA_VENCIMENTO,
                                                                                  SAIDA_PREVISTO = 0m,
                                                                                  SAIDA_REALIZADO = 0m,
                                                                                  ENTRADA_PREVISTO = r.VALOR_LIQUIDO,
                                                                                  ENTRADA_REALIZADO = 0m,
                                                                              }).Concat(from r in receberRealizadoQuery
                                                                                        select new
                                                                                        {
                                                                                            DATA = (DateTime)r.DATA_RECEBIMENTO,
                                                                                            SAIDA_PREVISTO = 0m,
                                                                                            SAIDA_REALIZADO = 0m,
                                                                                            ENTRADA_PREVISTO = 0m,
                                                                                            ENTRADA_REALIZADO = r.VALOR_LIQUIDO,
                                                                                        })
                                                                                   .Concat(from c in caixinhaQuery
                                                                                           select new
                                                                                           {
                                                                                               DATA = (DateTime)c.DATA_ENTREGA,
                                                                                               SAIDA_PREVISTO = c.VALOR,
                                                                                               SAIDA_REALIZADO = c.VALOR,
                                                                                               ENTRADA_PREVISTO = 0m,
                                                                                               ENTRADA_REALIZADO = 0m,
                                                                                           })
                               group p by p.DATA into g
                               select new FluxoCaixaModel()
                               {
                                   Data = g.Key,

                                   SaidaPrevisto = g.Sum(p => p.SAIDA_PREVISTO),
                                   SaidaRealizado = g.Sum(p => p.SAIDA_REALIZADO),

                                   EntradaPrevisto = g.Sum(p => p.ENTRADA_PREVISTO),
                                   EntradaRealizado = g.Sum(p => p.ENTRADA_REALIZADO),
                               }).ToArrayAsync();



            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 9);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD,9, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            doc.NewPage();


            PdfPTable table = new PdfPTable(7)
            {
                WidthPercentage = 100
            };

            table.SetWidths(new float[] { 100.0f / 7, 100.0f / 7, 100.0f / 7, 100.0f / 7, 100.0f / 7, 100.0f / 7, 100.0f / 7 });


            table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("ENTRADA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1, Colspan = 2 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("SAÍDA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1, Colspan = 2 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("RESULTADO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1, Colspan = 2 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("PREVISTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("REALIZADO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("PREVISTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("REALIZADO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("PREVISTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("REALIZADO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });



            foreach (var week in from item in items
                                 group item by item.Data.GetWeekOfMonth() into g
                                 select g)
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk("SEMANA: " + week.Key, normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.EntradaPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.EntradaRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.SaidaPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.SaidaRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.ResultadoPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(week.Select(item => item.ResultadoRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });

                foreach (var item in week)
                {


                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.Data.ToString("dd/MM/yyyy"), normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.EntradaPrevisto.ToString("C"), normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.EntradaRealizado.ToString("C"), normalFont))) { HorizontalAlignment = 1 });

                    table.AddCell(new PdfPCell(new Phrase(new Chunk((-item.SaidaPrevisto).ToString("C"), normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk((-item.SaidaRealizado).ToString("C"), normalFont))) { HorizontalAlignment = 1 });


                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.ResultadoPrevisto.ToString("C"), normalFont))) { HorizontalAlignment = 1 });
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.ResultadoRealizado.ToString("C"), normalFont))) { HorizontalAlignment = 1 });
                }

            }


            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.EntradaPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.EntradaRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.SaidaPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.SaidaRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.ResultadoPrevisto).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(item => item.ResultadoRealizado).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });
            doc.Add(table);


            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=fluxo-caixa.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}