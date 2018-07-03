using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Atimo.Controllers
{
    public class PagamentoTerceiroController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        static readonly String[] Situacoes = new string[]
        {
            "E",
            "P",
            "F",
            "K"
        };

        public async Task<ActionResult> Index(string de_vencimento = null, string ate_vencimento = null, int? terceiro = null, int? ossb = null)
        {
            IQueryable<OSSB_SERVICO_TERCEIRO> query = _db.OSSB_SERVICO_TERCEIRO
                        .Include(st => st.PAGAMENTO1)
                        .Where(ost => Situacoes.Contains(ost.OSSB_SERVICO1.OSSB1.SITUACAO));



            if (terceiro != null)
            {
                query = query.Where(ost => ost.TERCEIRO == terceiro);
            }

            if (ossb != null)
            {
                query = query.Where(ost => ost.OSSB_SERVICO1.OSSB == ossb);
            }

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                query = query.Where(st => st.DATE_VENCIMENTO >= de);
            }

            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                query = query.Where(st => st.DATE_VENCIMENTO <= ate);
            }

            ViewBag.DE_VENCIMENTO = de_vencimento;

            ViewBag.ATE_VENCIMENTO = ate_vencimento;

            ViewBag.OSSB = ossb;

            if (terceiro != null)
            {
                ViewBag.TERCEIRO = await _db
                .PESSOA
                .Where(t => t.ID == terceiro)
                .FirstOrDefaultAsync();
            }

            query = from st in query
                    where (st.PAGAMENTO1
                    .Select(p => p.VALOR)
                    .DefaultIfEmpty()
                    .Sum() < st.VALOR)
                    orderby st.DATE_VENCIMENTO
                    select st;

            return View(await query.ToArrayAsync());
        }

        public ActionResult Create(int ossb_servico = 0, int terceiro = 0)
        {

            return View(new PAGAMENTO_TERCEIRO() { OSSB_SERVICO = ossb_servico, TERCEIRO = terceiro });

        }

        public async Task<ActionResult> Imprimir(string de_vencimento = null, string ate_vencimento = null, int? terceiro = null, int? ossb = null)
        {
            IQueryable<OSSB_SERVICO_TERCEIRO> query = _db.OSSB_SERVICO_TERCEIRO
                        .Include(st => st.PAGAMENTO1)
                        .Where(ost => Situacoes.Contains(ost.OSSB_SERVICO1.OSSB1.SITUACAO));

            if (terceiro != null)
            {
                query = query.Where(ost => ost.TERCEIRO == terceiro);
            }

            if (ossb != null)
            {
                query = query.Where(ost => ost.OSSB_SERVICO1.OSSB == ossb);
            }

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                query = query.Where(st => st.DATE_VENCIMENTO >= de);
            }

            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                query = query.Where(st => st.DATE_VENCIMENTO <= ate);
            }

            if (terceiro != null)
            {
                ViewBag.TERCEIRO = await _db
                .PESSOA
                .Where(t => t.ID == terceiro)
                .FirstOrDefaultAsync();
            }

            query = from st in query
                    where (st.PAGAMENTO1
                    .Select(p => p.VALOR)
                    .DefaultIfEmpty()
                    .Sum() < st.VALOR)
                    orderby st.DATE_VENCIMENTO
                    select st;


            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);


            doc.Open();

            doc.NewPage();

            PdfPTable table = new PdfPTable(7)

            {
                TotalWidth = PageSize.A4.Width
            };


            table.AddCell(new PdfPCell(new Phrase(new Chunk("OSSB", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("SERVIÇO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("TERCEIRO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DATA DE VENCIMENTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("PAGO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("RESTANTE", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY });

            var items = await query.ToArrayAsync();

            foreach (var ost in items)
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.OSSB_SERVICO1.OSSB.ToString(), normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.OSSB_SERVICO1.DESCRICAO, normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.TERCEIRO1.NOME, normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.DATE_VENCIMENTO == null ? "" : ost.DATE_VENCIMENTO.Value.ToString("dd/MM/yyyy"), normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.VALOR.ToString("C"), normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk(ost.PAGAMENTO1.Select(p => p.VALOR).DefaultIfEmpty().Sum().ToString("C"), normalFont))));
                table.AddCell(new PdfPCell(new Phrase(new Chunk((ost.VALOR - ost.PAGAMENTO1.Select(p => p.VALOR).DefaultIfEmpty().Sum()).ToString("C"), normalFont))));
            }


            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });

            table.AddCell(new PdfPCell() { Colspan = 5, BackgroundColor = BaseColor.LIGHT_GRAY });


            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(pg => pg.VALOR - pg.PAGAMENTO1.Select(p => p.VALOR).DefaultIfEmpty().Sum()).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY });


            doc.Add(table);

            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=pagamento.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }





        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Create(PAGAMENTO_TERCEIRO pagamento)
        {

            var adiantamentos = await (
              from adt in _db.PAGAMENTO_TERCEIRO

              where adt.TERCEIRO == pagamento.TERCEIRO && adt.OSSB_SERVICO == pagamento.OSSB_SERVICO
              select adt
            )
            .ToArrayAsync();


            var total = await (
                from ost in _db.OSSB_SERVICO_TERCEIRO
                where ost.OSSB_SERVICO == pagamento.OSSB_SERVICO && ost.TERCEIRO == pagamento.TERCEIRO
                select ost.VALOR
                )
                .FirstOrDefaultAsync();

            if ((adiantamentos.Sum(adt => adt.VALOR) + pagamento.VALOR) > total)
            {
                ModelState.AddModelError("", "Valor ultrapassou o valor maximo!");
            }

            if (ModelState.IsValid)
            {


                HttpPostedFileBase file = Request.Files[0];

                byte[] buffer = new byte[file.ContentLength];

                file.InputStream.Read(buffer, 0, buffer.Length);

                ANEXO anexo = new ANEXO()
                {
                    NOME = file.FileName,
                    BUFFER = buffer
                };


                _db.ANEXO.Add(anexo);

                await _db.SaveChangesAsync();

                pagamento.ANEXO = anexo.ID;

                _db.PAGAMENTO_TERCEIRO.Add(pagamento);

                await _db.SaveChangesAsync();


                return RedirectToAction("Index", "PagamentoTerceiro");
            }
            else
            {


                return View(pagamento);
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Edit(PAGAMENTO_TERCEIRO pagamento)
        {

            if (ModelState.IsValid)
            {
                _db.Entry(pagamento).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "PagamentoTerceiro");
            }
            else
            {
                return View(pagamento);
            }
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}