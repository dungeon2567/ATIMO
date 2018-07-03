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

    public class ContasReceberController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(string de_vencimento = null, int? ossb = null, string ate_vencimento = null, string de_recebimento = null, string ate_recebimento = null, int? loja = null, int? cliente = null, string[] tipo_os = null, string por = null, string ordenar_por = "dv", int status = 2)
        {
            IQueryable<CONTAS_RECEBER> query = _db.CONTAS_RECEBER
                .Include(cr => cr.OSSB1.PESSOA);

            switch (status)
            {
                case 1:
                    query = query.Where(cr => cr.DATA_RECEBIMENTO != null);
                    break;
                case 2:
                    query = query.Where(cr => cr.DATA_RECEBIMENTO == null);
                    break;
            }

            if (ossb != null)
            {
                query = query.Where(cr => cr.OSSB == ossb);
            }


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

            if (de_recebimento != null)
            {
                DateTime de = DateTime.Parse(de_recebimento);

                query = query.Where(cr => cr.DATA_RECEBIMENTO >= de);
            }

            if(ate_recebimento != null)
            {
                DateTime ate = DateTime.Parse(ate_recebimento);

                query = query.Where(cr => cr.DATA_RECEBIMENTO <= ate);
            }

            if (cliente != null)
            {
                query = query.Where(cr => cr.OSSB1.CLIENTE == cliente);
            }

            if (loja != null)
            {
                query = query.Where(cr => cr.OSSB1.LOJA == loja);
            }

            if (tipo_os != null && tipo_os.Length > 0)
            {
                query = query.Where(cr => tipo_os.Contains(cr.OSSB1.TIPO));
            }

            if (de_vencimento != null)
            {
                ViewBag.DE_VENCIMENTO = de_vencimento;
            }

            if (ate_vencimento != null)
            {
                ViewBag.ATE_VENCIMENTO = ate_vencimento;
            }

            if (de_recebimento != null)
            {
                ViewBag.DE_RECEBIMENTO = de_recebimento;
            }

            if (ate_recebimento != null)
            {
                ViewBag.DE_RECEBIMENTO = ate_recebimento;
            }


            ViewBag.OSSB = ossb;

            ViewBag.STATUS = status;

            ViewBag.ORDENAR_POR = ordenar_por;


            switch (ordenar_por)
            {
                case "dv":
                    query = query.OrderBy(crp => crp.DATA_VENCIMENTO);
                    break;
                default:
                    query = query.OrderBy(crp => crp.OSSB1.PESSOA.RAZAO)
                        .ThenBy(crp => crp.OSSB1.LOJA1.APELIDO);
                    break;

            }

            return View(await query.ToArrayAsync());

        }

        public async Task<ActionResult> Imprimir(string de_vencimento = null, int? ossb = null, string ate_vencimento = null, string de_recebimento = null, string ate_recebimento = null, int? loja = null, int? cliente = null, string[] tipo_os = null, int status = 2)
        {

            IQueryable<CONTAS_RECEBER> query = _db.CONTAS_RECEBER
                .Include(cr => cr.OSSB1.PESSOA);


            switch (status)
            {
                case 1:
                    query = query.Where(cr => cr.DATA_RECEBIMENTO != null);
                    break;
                case 2:
                    query = query.Where(cr => cr.DATA_RECEBIMENTO == null);
                    break;
            }


            if (ossb != null)
            {
                query = query.Where(cr => cr.OSSB == ossb);
            }

            if (de_recebimento != null)
            {
                DateTime de = DateTime.Parse(de_recebimento);

                query = query.Where(cr => cr.DATA_RECEBIMENTO >= de);
            }

            if (ate_recebimento != null)
            {
                DateTime ate = DateTime.Parse(ate_recebimento);

                query = query.Where(cr => cr.DATA_RECEBIMENTO <= ate);
            }

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

            if (cliente != null)
            {
                query = query.Where(cr => cr.OSSB1.CLIENTE == cliente);
            }

            if (loja != null)
            {
                query = query.Where(cr => cr.OSSB1.LOJA == loja);
            }

            if (tipo_os != null && tipo_os.Length > 0)
            {
                query = query.Where(cr => tipo_os.Contains(cr.OSSB1.TIPO));
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

            doc.NewPage();


            foreach (var group in from cr in items
                                  group cr by cr.DATA_VENCIMENTO into g
                                  orderby g.Key
                                  select new { DATA = g.Key, ITEMS = g })
            {

                PdfPTable table = new PdfPTable(6)

                {
                    TotalWidth = PageSize.A4.Width
                };

                table.SpacingBefore = 7.5f;
                table.SpacingAfter = 7.5f;


                table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE VENCIMENTO: " + group.DATA.ToString("dd/MM/yyyy"), normalFont))) { Colspan = 6, HorizontalAlignment = Element.ALIGN_CENTER });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("OSSB", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("CLIENTE", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("NOTA FISCAL", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR BRUTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR LÍQUIDO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
                table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE RECEBIMENTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });


                foreach (var item in group.ITEMS)
                {
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB.ToString(), normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB1.PESSOA.NOME, normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.NOTA_FISCAL, normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR_BRUTO.ToString("C"), normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR_LIQUIDO.ToString("C"), normalFont))));
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DATA_RECEBIMENTO.ToStringOrDefault("dd/MM/yyyy", ""), normalFont))));
                }

                table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

                table.AddCell(new PdfPCell() { Colspan = 2, BackgroundColor = BaseColor.LIGHT_GRAY });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(group.ITEMS.Select(it => it.VALOR_BRUTO).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(group.ITEMS.Select(it => it.VALOR_LIQUIDO).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

                table.AddCell(new PdfPCell() { BackgroundColor = BaseColor.LIGHT_GRAY });


                doc.Add(table);

            }


            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=contas_receber.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        public async Task<ActionResult> Deletar(Int32 id)
        {
            var cr = await _db
                .CONTAS_RECEBER
                .FirstOrDefaultAsync(crr => crr.ID == id);

            if (cr == null)
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            _db.Entry(cr)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public async Task< ActionResult> Edit(int id = 0)
        {
                var cr = await _db.CONTAS_RECEBER
                    .FirstOrDefaultAsync(cp => cp.ID == id);

                if (cr != null)
                {
                    ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", cr.CONTA_BANCARIA);


                    return View(cr);
                }
                else
                {
                    return HttpNotFound();
                }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CONTAS_RECEBER cr)
        {
            if(cr.DATA_RECEBIMENTO != null && cr.DATA_RECEBIMENTO < DateTime.Today.AddDays(-1))
            {
                ModelState.AddModelError("", "A data de recebimento não pode ser inferior a data atual.");
            }

            if (ModelState.IsValid)
            {
                _db.Entry(cr).State = EntityState.Modified;

                _db.SaveChanges();

                return RedirectToAction("Index", "ContasReceber");
            }


            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", cr.CONTA_BANCARIA);


            return View(cr);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }

}