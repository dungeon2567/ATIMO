using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System;
using ATIMO;
using ATIMO.ViewModel;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Globalization;
using OfficeOpenXml;

namespace Atimo.Controllers
{
    public class PagamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Imprimir(string de_pagamento = null, int? projeto = null, int[] criticidade = null, string ate_pagamento = null, int? ossb = null, string de_vencimento = null, int? fornecedor = null, string ate_vencimento = null, int status = 2, int[] despesa = null, string tipo = null, int page = 1)
        {

            IQueryable<PAGAMENTO> pagamentos = from p in _db.PAGAMENTO
                             .Include(p => p.PESSOA1)
                             .Include(p => p.DESPESA1)
                             .Include(p => p.PROJETO1)
                                               select p;


            if (ossb != null)
            {
                pagamentos = pagamentos.Where(p => p.OSSB == ossb);
            }

            if (projeto != null)
            {
                pagamentos = pagamentos.Where(p => p.PROJETO == projeto);
            }


            if (despesa != null)
            {
                pagamentos = pagamentos.Where(p => despesa.Contains(p.DESPESA));
            }

            if (fornecedor != null)
            {
                pagamentos = pagamentos.Where(p => p.PESSOA == fornecedor);
            }


            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO != null);
                    break;
                case 2:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO == null && (cr.OSSB == null || (cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
                default:
                    pagamentos = pagamentos.Where(cr => (cr.DATA_PAGAMENTO != null) || (cr.OSSB == null || (cr.DATA_PAGAMENTO == null && cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
            }

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO >= de);
            }


            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO <= ate);
            }

            if (de_pagamento != null)
            {
                DateTime de = DateTime.Parse(de_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO >= de);
            }


            if (ate_pagamento != null)
            {
                DateTime ate = DateTime.Parse(ate_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO <= ate);
            }

            if (criticidade != null)
            {

                pagamentos = pagamentos.Where(p => criticidade.Contains(p.CRITICIDADE));
            }

            switch (tipo)
            {
                case "F":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "F");
                    break;
                case "V":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "V");
                    break;
            }

            ViewBag.DESPESAS = await _db.DESPESA_CLASSE
                .Include(dc => dc.DESPESA)
                .ToArrayAsync();


            ViewBag.DESPESAS_SELECIONADAS = despesa ?? new int[0];

            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA
                .Where(p => p.SITUACAO == "A")
                .OrderBy(p => p.RAZAO)
                .ThenBy(p => p.NOME).ToArrayAsync(), "ID", "NOME_COMPLETO", fornecedor);

            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.OrderBy(p => p.DATA_PAGAMENTO)
                        .ThenBy(p => p.ID);
                    break;
                case 2:
                    pagamentos = pagamentos
                                  .OrderBy(p => p.DATA_VENCIMENTO == null)
                                  .ThenBy(p => p.DATA_VENCIMENTO)
                                  .ThenBy(p => p.PESSOA1.RAZAO)
                                  .ThenBy(p => p.ID);
                    break;
                default:
                    pagamentos = pagamentos.OrderBy(p => p.ID);
                    break;
            }


            var items = await pagamentos.ToArrayAsync();

            var fs = new MemoryStream();

            Document doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 7);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 7, BaseColor.WHITE);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            doc.NewPage();


            PdfPTable table = new PdfPTable(11)
            {
                WidthPercentage = 100
            };

            table.SetWidths(new float[] { 5.0f, 10.0f, 20.0f, 10.0f, 5.0f, 5.0f, 10.0f, 5.0f, 5.0f, 5.0f, 10.0f });


            table.AddCell(new PdfPCell(new Phrase(new Chunk("OSSB", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("PROJETO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("BENEFICIÁRIO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DESCRIÇÃO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("D. DE PGTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("D. DE VENC", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("DESPESA", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("PROV", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("F. DE PGTO", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("CRÍT.", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });
            table.AddCell(new PdfPCell(new Phrase(new Chunk("VALOR", boldWhiteFont))) { BackgroundColor = BaseColor.DARK_GRAY, HorizontalAlignment = 1 });


            foreach (var item in items)
            {

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.OSSB?.ToString() ?? "", normalFont))) { HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.PROJETO1?.DESCRICAO ?? "", normalFont))) { HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.PESSOA1?.RAZAO ?? "", normalFont))) { HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DESCRICAO ?? "", normalFont))) { HorizontalAlignment = 1 });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DATA_PAGAMENTO?.ToString("dd/MM/yyyy") ?? "", normalFont))) { HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DATA_VENCIMENTO?.ToString("dd/MM/yyyy") ?? "", normalFont))) { HorizontalAlignment = 1 });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.DESPESA1.DESCRICAO, normalFont))) { HorizontalAlignment = 1 });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.PROVISIONADO ? "SIM" : "NÃO", normalFont))) { HorizontalAlignment = 1 });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.FORMA_PAGAMENTO == 0 ? "DOC" : (item.FORMA_PAGAMENTO == 1 ? "BOLETO" : "DINHEIRO"), normalFont))) { HorizontalAlignment = 1 });
                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.CRITICIDADE.ToString(), normalFont))) { HorizontalAlignment = 1 });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(item.VALOR.ToString("C"), normalFont))) { HorizontalAlignment = 1 });



            }


            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });

            table.AddCell(new PdfPCell() { BackgroundColor = BaseColor.LIGHT_GRAY, Colspan = 9 });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(items.Select(it => it.VALOR).DefaultIfEmpty().Sum().ToString("C"), normalFont))) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = 1 });

            doc.Add(table);


            doc.Close();

            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=custos.pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        public async Task Excel(string de_pagamento = null, int? projeto = null, int[] criticidade = null, string ate_pagamento = null, int? ossb = null, string de_vencimento = null, int? fornecedor = null, string ate_vencimento = null, int status = 2, int[] despesa = null, string tipo = null, int page = 1)
        {

            IQueryable<PAGAMENTO> pagamentos = from p in _db.PAGAMENTO
                             .Include(p => p.PESSOA1)
                             .Include(p => p.DESPESA1)
                             .Include(p => p.PROJETO1)
                                               select p;


            if (ossb != null)
            {
                pagamentos = pagamentos.Where(p => p.OSSB == ossb);
            }

            if (projeto != null)
            {
                pagamentos = pagamentos.Where(p => p.PROJETO == projeto);
            }


            if (despesa != null)
            {
                pagamentos = pagamentos.Where(p => despesa.Contains(p.DESPESA));
            }

            if (fornecedor != null)
            {
                pagamentos = pagamentos.Where(p => p.PESSOA == fornecedor);
            }


            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO != null);
                    break;
                case 2:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO == null && (cr.OSSB == null || (cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
                default:
                    pagamentos = pagamentos.Where(cr => (cr.DATA_PAGAMENTO != null) || (cr.OSSB == null || (cr.DATA_PAGAMENTO == null && cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
            }

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO >= de);
            }


            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO <= ate);
            }

            if (de_pagamento != null)
            {
                DateTime de = DateTime.Parse(de_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO >= de);
            }


            if (ate_pagamento != null)
            {
                DateTime ate = DateTime.Parse(ate_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO <= ate);
            }

            if (criticidade != null)
            {

                pagamentos = pagamentos.Where(p => criticidade.Contains(p.CRITICIDADE));
            }

            switch (tipo)
            {
                case "F":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "F");
                    break;
                case "V":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "V");
                    break;
            }

            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.OrderBy(p => p.DATA_PAGAMENTO)
                        .ThenBy(p => p.ID);
                    break;
                case 2:
                    pagamentos = pagamentos
                                  .OrderBy(p => p.DATA_VENCIMENTO == null)
                                  .ThenBy(p => p.DATA_VENCIMENTO)
                                  .ThenBy(p => p.PESSOA1.RAZAO)
                                  .ThenBy(p => p.ID);
                    break;
                default:
                    pagamentos = pagamentos.OrderBy(p => p.ID);
                    break;
            }


            var items = await pagamentos.ToArrayAsync();

            ExcelPackage pck = new ExcelPackage();

            var ws = pck.Workbook.Worksheets.Add("Pagamentos");

            ws.Cells[1, 1].Value = "OSSB";
            ws.Cells[1, 2].Value = "PROJETO";
            ws.Cells[1, 3].Value = "BENEFICIÁRIO";
            ws.Cells[1, 4].Value = "DESCRIÇÃO";
            ws.Cells[1, 5].Value = "DATA DE PAGAMENTO";
            ws.Cells[1, 6].Value = "DATA DE VENCIMENTO";
            ws.Cells[1, 7].Value = "DESPESA";
            ws.Cells[1, 8].Value = "PROV";
            ws.Cells[1, 9].Value = "FORMA DE PAGAMENTO";
            ws.Cells[1, 10].Value = "CRIT";
            ws.Cells[1, 11].Value = "VALOR";


            Int32 row = 1;

            foreach (var item in items)
            {
                ++row;

                ws.Cells[row, 1].Value = item.OSSB?.ToString() ?? "";
                ws.Cells[row, 2].Value = item.PROJETO1?.DESCRICAO ?? "";

                ws.Cells[row, 3].Value =  item.PESSOA1?.RAZAO ?? "";
                ws.Cells[row, 4].Value = item.DESCRICAO ?? "";


                ws.Cells[row, 5].Value = item.DATA_PAGAMENTO?.ToString("dd/MM/yyyy") ?? "";
                ws.Cells[row, 6].Value = item.DATA_VENCIMENTO?.ToString("dd/MM/yyyy") ?? "";

                ws.Cells[row, 7].Value = item.DESPESA1.DESCRICAO;


                ws.Cells[row, 8].Value = item.PROVISIONADO ? "SIM" : "NÃO";


                ws.Cells[row, 9].Value = item.FORMA_PAGAMENTO == 0 ? "DOC" : (item.FORMA_PAGAMENTO == 1 ? "BOLETO" : "DINHEIRO");
                ws.Cells[row, 10].Value = item.CRITICIDADE;
                ws.Cells[row, 11].Value = item.VALOR;

            }


            pck.SaveAs(Response.OutputStream);

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.AddHeader("content-disposition", "attachment;  filename=pagamentos.xlsx");
        }


        public async Task<ActionResult> Index(string de_pagamento = null, int? projeto = null, int[] criticidade = null, string ate_pagamento = null, int? ossb = null, string de_vencimento = null, int? fornecedor = null, string ate_vencimento = null, int status = 2, int[] despesa = null, string tipo = null, int page = 1)
        {

            IQueryable<PAGAMENTO> pagamentos = from p in _db.PAGAMENTO
                             .Include(p => p.PESSOA1)
                             .Include(p => p.DESPESA1)
                                               select p;


            if (ossb != null)
            {
                pagamentos = pagamentos.Where(p => p.OSSB == ossb);
            }

            if (projeto != null)
            {
                pagamentos = pagamentos.Where(p => p.PROJETO == projeto);
            }

            if (despesa != null)
            {
                pagamentos = pagamentos.Where(p => despesa.Contains(p.DESPESA));
            }

            if (fornecedor != null)
            {
                pagamentos = pagamentos.Where(p => p.PESSOA == fornecedor);
            }


            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO != null);
                    break;
                case 2:
                    pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO == null && (cr.OSSB == null || (cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
                default:
                    pagamentos = pagamentos.Where(cr => (cr.DATA_PAGAMENTO != null) || (cr.OSSB == null || (cr.DATA_PAGAMENTO == null && cr.OSSB1.SITUACAO == "K" || cr.OSSB1.SITUACAO == "P" || cr.OSSB1.SITUACAO == "F" || cr.OSSB1.SITUACAO == "E")));
                    break;
            }

            if (de_vencimento != null)
            {
                DateTime de = DateTime.Parse(de_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO >= de);
            }


            if (ate_vencimento != null)
            {
                DateTime ate = DateTime.Parse(ate_vencimento);

                pagamentos = pagamentos.Where(cr => cr.DATA_VENCIMENTO <= ate);
            }

            if (de_pagamento != null)
            {
                DateTime de = DateTime.Parse(de_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO >= de);
            }


            if (ate_pagamento != null)
            {
                DateTime ate = DateTime.Parse(ate_pagamento);

                pagamentos = pagamentos.Where(cr => cr.DATA_PAGAMENTO <= ate);
            }

            if (criticidade != null)
            {
                ViewBag.CRITICIDADE = criticidade;

                pagamentos = pagamentos.Where(p => criticidade.Contains(p.CRITICIDADE));
            }



            if (de_vencimento != null)
            {
                ViewBag.DE_VENCIMENTO = de_vencimento;
            }

            if (ate_vencimento != null)
            {
                ViewBag.ATE_VENCIMENTO = ate_vencimento;
            }

            if (ossb != null)
            {
                ViewBag.OSSB = ossb;
            }

            if (de_pagamento != null)
            {
                ViewBag.DE_PAGAMENTO = de_pagamento;
            }

            if (ate_pagamento != null)
            {
                ViewBag.ATE_PAGAMENTO = ate_pagamento;
            }



            ViewBag.DESPESAS = await _db.DESPESA_CLASSE
                .Include(dc => dc.DESPESA)
                .ToArrayAsync();

            ViewBag.PROJETO = projeto;

            ViewBag.PROJETOS = new SelectList(await _db.PROJETO
                .OrderBy(p => p.ID).ToArrayAsync(), "ID", "DESCRICAO", projeto);

            ViewBag.DESPESAS_SELECIONADAS = despesa ?? new int[0];

            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA
                .OrderBy(p => p.RAZAO)
                .ThenBy(p => p.NOME).ToArrayAsync(), "ID", "NOME_COMPLETO", fornecedor);

            switch (tipo)
            {
                case "F":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "F");
                    break;
                case "V":
                    pagamentos = pagamentos.Where(p => p.DESPESA1.TIPO == "V");
                    break;
            }

            switch (status)
            {
                case 1:
                    pagamentos = pagamentos.OrderBy(p => p.DATA_PAGAMENTO)
                        .ThenBy(p => p.ID);
                    break;
                case 2:
                    pagamentos = pagamentos
                                  .OrderBy(p => p.DATA_VENCIMENTO == null)
                                  .ThenBy(p => p.DATA_VENCIMENTO)
                                  .ThenBy(p => p.PESSOA1.RAZAO)
                                  .ThenBy(p => p.ID);
                    break;
                default:
                    pagamentos = pagamentos.OrderBy(p => p.ID);
                    break;
            }

            ViewBag.STATUS = status;

            ViewBag.PAGE = page;

            ViewBag.PAGE_COUNT = ((await pagamentos.CountAsync() - 1) / 10) + 1;


            var j = await pagamentos.Skip((page - 1) * 10)
                .Take(10).ToArrayAsync();


            return View(j);

        }

        public async Task<ActionResult> Create()
        {

            ViewBag.DESPESA = new SelectList(await _db.DESPESA
                .Include(dp => dp.DESPESA_CLASSE)
                .OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO)
                .ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", (Object)null);

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .OrderBy(p => p.RAZAO)
                .ThenBy(p => p.NOME)
                .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                .ToArrayAsync(), "ID", "NOME_COMPLETO");


            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(),
                "ID", "DESCRICAO");

            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO");


            Session["PreviousPage"] = Request.UrlReferrer.ToString();


            return View(new PAGAMENTO() { DATA_VENCIMENTO = DateTime.Today, CRITICIDADE = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PAGAMENTO pagamento)
        {

            if (pagamento.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

            if (pagamento.PESSOA == 0)
                ModelState.AddModelError("", "Informe uma pessoa!");

            if (pagamento.DATA_PAGAMENTO != null && pagamento.DATA_PAGAMENTO < DateTime.Today.AddDays(-1))
                ModelState.AddModelError("", "A data de pagamento não pode ser inferior a data atual.");


            if (ModelState.IsValid)
            {
                var entry = _db.Entry(pagamento);

                _db.PAGAMENTO.Attach(pagamento);

                entry.Property(pp => pp.OSSB).IsModified = true;
                entry.Property(pp => pp.DESCRICAO).IsModified = true;
                entry.Property(pp => pp.PESSOA).IsModified = true;
                entry.Property(pp => pp.PROJETO).IsModified = true;
                entry.Property(pp => pp.DESPESA).IsModified = true;
                entry.Property(pp => pp.CRITICIDADE).IsModified = true;

                entry.Property(pp => pp.DATA_VENCIMENTO).IsModified = true;
                entry.Property(pp => pp.DATA_PAGAMENTO).IsModified = true;
                entry.Property(pp => pp.VALOR).IsModified = true;
                entry.Property(pp => pp.PROTESTO).IsModified = true;
                entry.Property(pp => pp.PROVISIONADO).IsModified = true;
                entry.Property(pp => pp.FORMA_PAGAMENTO).IsModified = true;
                entry.Property(pp => pp.CONTA_BANCARIA).IsModified = true;

                await _db.SaveChangesAsync();


                return Redirect(Session["PreviousPage"].ToString());
            }

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", pagamento.DESPESA);

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .OrderBy(p => p.RAZAO)
                .ThenBy(p => p.NOME)
                .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                .ToArrayAsync(), "ID", "NOME_COMPLETO", pagamento.PESSOA);

            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", pagamento.CONTA_BANCARIA);

            return View(pagamento);
        }

        [HttpPost]
        public async Task<ActionResult> AnexarComprovante(Int32 id, HttpPostedFileBase file)
        {
            var item = await _db
               .PAGAMENTO
                .Include(cl => cl.ANEXO1)
                .FirstOrDefaultAsync(checkitem => checkitem.ID == id);

            if (file != null && item != null)
            {
                byte[] buffer = new byte[file.ContentLength];

                file.InputStream.Read(buffer, 0, buffer.Length);

                if (item.ANEXO1 == null)
                {
                    item.ANEXO1 = new ANEXO() { NOME = file.FileName, BUFFER = buffer };
                }
                else
                {
                    item.ANEXO1.NOME = file.FileName;
                    item.ANEXO1.BUFFER = buffer;
                }

                await _db.SaveChangesAsync();

            }

            return Redirect(Request.UrlReferrer.ToString());

        }


        [HttpGet]
        public async Task<ActionResult> VisualizarComprovante(Int32 id)
        {

            var item = await _db
                .PAGAMENTO
                .Include(ci => ci.ANEXO1)
                .FirstOrDefaultAsync(ci => ci.ID == id);

            if (item != null && item.ANEXO1 != null)
            {
                Response.AddHeader("Content-Disposition", "inline; filename=" + item.ANEXO1.NOME);

                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME));
            }

            return HttpNotFound();
        }
        public async Task<ActionResult> Edit(int id = 0)
        {
            var pagamento = await _db
                .PAGAMENTO
                .FirstOrDefaultAsync(pg => pg.ID == id);

            if (pagamento == null)
                return HttpNotFound();

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", pagamento.DESPESA);

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .OrderBy(p => p.NOME)
                .ToArrayAsync(), "ID", "NOME_COMPLETO", pagamento.PESSOA);

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(),
                "ID", "DESCRICAO", pagamento.PROJETO);

            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", pagamento.CONTA_BANCARIA);


            Session["PreviousPage"] = Request.UrlReferrer.ToString();

            return View(pagamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PAGAMENTO pagamento)
        {
            if (pagamento.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

            if (pagamento.PESSOA == 0)
                ModelState.AddModelError("", "Informe uma pessoa!");

            if (pagamento.PROJETO == null || pagamento.PROJETO == 0)
                ModelState.AddModelError("", "Informe um projeto!");

            if (pagamento.DATA_PAGAMENTO != null && pagamento.DATA_PAGAMENTO < DateTime.Today.AddDays(-1))
                ModelState.AddModelError("", "A data de pagamento não pode ser inferior a data atual.");

            if (ModelState.IsValid)
            {
                _db.PAGAMENTO.Add(pagamento);

                for (Int32 it = 0; it < pagamento.COPIAS; ++it)
                {
                    _db.PAGAMENTO.Add(new PAGAMENTO()
                    {
                        CRITICIDADE = pagamento.CRITICIDADE,
                        DATA_PAGAMENTO = pagamento.DATA_PAGAMENTO?.AddMonths(it + 1),
                        DATA_VENCIMENTO = pagamento.DATA_VENCIMENTO?.AddMonths(it + 1),
                        VALOR = pagamento.VALOR,
                        DESPESA = pagamento.DESPESA,
                        PESSOA = pagamento.PESSOA,
                        DESCRICAO = pagamento.DESCRICAO,
                        PROJETO = pagamento.PROJETO,
                        PROVISIONADO = pagamento.PROVISIONADO,
                        PROTESTO = pagamento.PROTESTO,
                        FORMA_PAGAMENTO = pagamento.FORMA_PAGAMENTO,
                        OSSB = pagamento.OSSB,
                        SERVICO = pagamento.SERVICO,
                        ANEXO = pagamento.ANEXO,
                    });
                }

                await _db.SaveChangesAsync();

                return Redirect(Session["PreviousPage"].ToString());
            }


            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", pagamento.DESPESA);

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .OrderBy(p => p.NOME)
                .ToArrayAsync(), "ID", "NOME_COMPLETO");

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(),
                "ID", "DESCRICAO", pagamento.PROJETO);

            ViewBag.CONTA_BANCARIA = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", pagamento.CONTA_BANCARIA);


            return View(pagamento);
        }

        public async Task<ActionResult> Deletar(Int32 id)
        {
            var p = await _db.PAGAMENTO.FirstOrDefaultAsync(pp => pp.ID == id);

            if (p == null)
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            if (p.DATA_PAGAMENTO != null)
            {
                return Content("Erro ao deletar pagamento");
            }

            _db.Entry(p)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }
        public async Task<ActionResult> Pagar(Int32 id)
        {
            var p = await (from item in _db.PAGAMENTO
                           where item.ID == id
                           select item)
                           .FirstOrDefaultAsync();

            if (p == null)
                return HttpNotFound();

            ViewBag.ContaBancaria = new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO", p.CONTA_BANCARIA);

            Session["PreviousPage"] = Request.UrlReferrer.ToString();

            return View(new PagarViewModel() { DataPagamento = DateTime.Today, FormaPagamento = p.FORMA_PAGAMENTO, Id = id, Valor = p.VALOR });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Pagar(PagarViewModel model)
        {
            if (model.DataPagamento < DateTime.Today.AddDays(-1))
                return Content("A data de pagamento não pode ser inferior a data atual.");

            var p = await (from item in _db.PAGAMENTO
                           where item.ID == model.Id
                           select item).FirstOrDefaultAsync();

            if (p == null)
                return HttpNotFound();

            if (model.Valor > p.VALOR)
            {
                return Content("Erro: valor maior que o total.");
            }
            else
                if (model.Valor == p.VALOR)
            {
                p.FORMA_PAGAMENTO = model.FormaPagamento;
                p.CONTA_BANCARIA = model.ContaBancaria;
                p.DATA_PAGAMENTO = model.DataPagamento;
            }
            else
            {
                p.VALOR -= model.Valor;

                _db.PAGAMENTO.Add(new PAGAMENTO()
                {
                    OSSB = p.OSSB,
                    CRITICIDADE = p.CRITICIDADE,
                    DESCRICAO = p.DESCRICAO,
                    SERVICO = p.SERVICO,
                    DATA_PAGAMENTO = model.DataPagamento,
                    VALOR = model.Valor,
                    FORMA_PAGAMENTO = model.FormaPagamento,
                    PROTESTO = false,
                    DESPESA = p.DESPESA,
                    DATA_VENCIMENTO = p.DATA_VENCIMENTO,
                    PESSOA = p.PESSOA,
                    PROVISIONADO = p.PROVISIONADO,
                    PROJETO = p.PROJETO,
                    CONTA_BANCARIA = model.ContaBancaria,
                });
            };

            await _db.SaveChangesAsync();

            return Redirect(Session["PreviousPage"].ToString());
        }



        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}