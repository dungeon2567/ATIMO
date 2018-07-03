using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ATIMO;
using ATIMO.Models.Faturamento;
using ATIMO.Helpers.ModelsUtils;
using System.IO;

namespace Atimo.Controllers
{

    public class ContasRecebidasController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(string de_vencimento = null, string ate_vencimento = null)
        {
     
                IQueryable<CONTAS_RECEBER> query = _db.CONTAS_RECEBER
                    .Include(cr => cr.OSSB1.PESSOA)
                    .Where(cr => cr.DATA_RECEBIMENTO != null);

                if (de_vencimento != null)
                {
                    DateTime de = DateTime.Parse(de_vencimento);

                    query = query.Where(cr => cr.DATA_VENCIMENTO >= de);
                }


                if (ate_vencimento != null)
                {
                    DateTime ate = DateTime.Parse(ate_vencimento);

                    query = query.Where(cr => cr.DATA_VENCIMENTO <= ate);
                }

                if (de_vencimento != null)
                {
                    ViewBag.DE_VENCIMENTO = de_vencimento;
                }

                if (ate_vencimento != null)
                {
                    ViewBag.ATE_VENCIMENTO = ate_vencimento;
                }


                return View(await query.OrderBy(crp => crp.DATA_VENCIMENTO)
                .ThenBy(crp => crp.OSSB)
                .ToArrayAsync());
        }


        public async Task<ActionResult> Imprimir(string de_vencimento = null, string ate_vencimento = null)
        {

            IQueryable<CONTAS_RECEBER> query = _db.CONTAS_RECEBER
                .Include(cr => cr.OSSB1.PESSOA)
                .Where(cr => cr.DATA_RECEBIMENTO == null);

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                query = query.Where(cr => cr.DATA_VENCIMENTO >= de);
            }


            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                query = query.Where(cr => cr.DATA_VENCIMENTO <= ate);
            }

            var items = await query.OrderBy(crp => crp.DATA_VENCIMENTO)
            .ThenBy(crp => crp.OSSB)
            .ToArrayAsync();

            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();


            foreach (var group in from cr in items
                                  group cr by cr.DATA_VENCIMENTO into g
                                  orderby g.Key
                                  select new { DATA = g.Key, ITEMS = g })
            {
                doc.NewPage();

                PdfPTable table = new PdfPTable(5)

                {
                    TotalWidth = PageSize.A4.Width
                };


                table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE VENCIMENTO: " + group.DATA.ToString("dd/MM/yyyy"), normalFont))) {Colspan = 5, HorizontalAlignment = Element.ALIGN_CENTER });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("OSSB", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("CLIENTE", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("NOTA FISCAL", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR BRUTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR LÍQUIDO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });


                foreach (var item in group.ITEMS)
                {
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB.ToString(), normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB1.PESSOA.NOME, normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.NOTA_FISCAL, normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR_BRUTO.ToString("C"), normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR_LIQUIDO.ToString("C"), normalFont))));
                }

                table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

                table.AddCell(new PdfPCell() { Colspan = 2, BackgroundColor = BaseColor.LIGHT_GRAY });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(group.ITEMS.Select(it => it.VALOR_BRUTO).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(group.ITEMS.Select(it => it.VALOR_LIQUIDO).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });


                doc.Add(table);

            }


            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=contas_receber.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }


        public async Task<ActionResult> Deletar(Int32 id)
        {
            var cr = await _db.CONTAS_RECEBER.FirstOrDefaultAsync(crr => crr.ID == id);

            if (cr == null)
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            _db.Entry(cr)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }


        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var cr = _db.CONTAS_RECEBER
                    .FirstOrDefault(cp => cp.ID == id);

                if (cr != null)
                {

                    return View(cr);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
                return RedirectToAction("", "");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CONTAS_RECEBER cr)
        {
            if (Session.IsFuncionario())
            {

                if (ModelState.IsValid)
                {
                    _db.Entry(cr).State = EntityState.Modified;

                    _db.SaveChanges();

                    return RedirectToAction("Index", "ContasReceber");
                }

                return View(cr);
            }
            else
                return RedirectToAction("", "");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }

}