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
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Atimo.Controllers
{
    public class RelatorioCustosController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Imprimir(string de_pagamento = null, string ate_pagamento = null, int ? ossb = null, String de_vencimento = null, String ate_vencimento = null, String tipo = "", Int32? projeto = null, String protesto = null, Int32 status = 0)
        {
            IQueryable<PAGAMENTO> queryPagamento = from p in _db.PAGAMENTO
                .Include(cp => cp.DESPESA1)
                .Include(cp => cp.PESSOA1)
                                                   select p;


            if (protesto != null)
            {
                switch (protesto)
                {
                    case "S":
                        queryPagamento = queryPagamento.Where(p => p.PROTESTO == true);
                        break;
                    case "N":
                        queryPagamento = queryPagamento.Where(p => p.PROTESTO == false);
                        break;
                }
            }


            switch (status)
            {
                case 1:
                    queryPagamento = queryPagamento.Where(cr => cr.DATA_PAGAMENTO != null);
                    break;
                case 2:
                    queryPagamento = queryPagamento.Where(cr => cr.DATA_PAGAMENTO == null && (cr.OSSB == null || (cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
                default:
                    queryPagamento = queryPagamento.Where(cr => (cr.DATA_PAGAMENTO != null) || (cr.OSSB == null || (cr.DATA_PAGAMENTO == null && cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
            }



            if (tipo != null)
            {
                switch (tipo)
                {
                    case "V":
                        queryPagamento = queryPagamento.Where(p => p.DESPESA1.TIPO == "V");

                        break;
                    case "F":
                        queryPagamento = queryPagamento.Where(p => p.DESPESA1.TIPO == "F");
                        break;
                }
            }

            if (ossb != null)
            {
                queryPagamento = queryPagamento.Where(cp => cp.OSSB == ossb);
            }

            if (de_pagamento != null)
            {
                DateTime deDate = DateTime.Parse(de_pagamento);

                queryPagamento = queryPagamento.Where(cp => cp.DATA_PAGAMENTO >= deDate);
            }


            if (ate_pagamento != null)
            {
                DateTime ateDate = DateTime.Parse(ate_pagamento);

                queryPagamento = queryPagamento.Where(cp => cp.DATA_PAGAMENTO <= ateDate);
            }


            if (de_vencimento != null)
            {
                DateTime deDate = DateTime.Parse(de_vencimento);

                queryPagamento = queryPagamento.Where(cp => cp.DATA_VENCIMENTO >= deDate);
            }


            if (ate_vencimento != null)
            {
                DateTime ateDate = DateTime.Parse(ate_vencimento);

                queryPagamento = queryPagamento.Where(cp => cp.DATA_VENCIMENTO <= ateDate);
            }

            var queryItems = (from p in queryPagamento
                              select new
                              {
                                  OSSB = p.OSSB,
                                  PESSOA = p.PESSOA1,
                                  DESPESA = p.DESPESA1,
                                  DATA_PAGAMENTO = p.DATA_PAGAMENTO,
                                  DATA_VENCIMENTO = p.DATA_VENCIMENTO,
                                  VALOR = p.VALOR
                              });


            var items = await queryItems.ToArrayAsync();

            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            doc.NewPage();


            PdfPTable table = new PdfPTable(6)
            {
                WidthPercentage = 100
            };

            table.SetWidths(new float[] { 12.5f, 22.5f, 12.5f, 12.5f, 27.5f, 12.5f });


            table.AddCell(new PdfPCell(new Phrase(new Chunk("OSSB", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("FORNECEDOR", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE PAGAMENTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE VENCIMENTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DESPESA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });


            foreach (var item in items)
            {

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB?.ToString() ?? "", normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.PESSOA?.RAZAO ?? "", normalFont))));


                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DATA_PAGAMENTO?.ToString("dd/MM/yyyy") ?? "", normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DATA_VENCIMENTO?.ToString("dd/MM/yyyy") ?? "", normalFont))));

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DESPESA.DESCRICAO, normalFont))));

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR.ToString("C"), normalFont))));



            }


            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

            table.AddCell(new PdfPCell() { BackgroundColor = BaseColor.LIGHT_GRAY, Colspan = 4 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(it => it.VALOR).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

            doc.Add(table);


            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=custos.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);

        }
        public async Task<ActionResult> Search()
        {
            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO");

            return View();

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}