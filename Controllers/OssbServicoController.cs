using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text;
using ATIMO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;
using System.IO;

namespace Atimo.Controllers
{

    public class OssbServicoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /OssbServico/

        public ActionResult Index(int id = 0)
        {

            var ossbServico = _db.OSSB_SERVICO
                .Where(o => o.OSSB == id);

            var unidades = _db.UNIDADE;

            var viewModel = new OssbServicoIndexViewModel { Servicos = ossbServico.ToArray(), Unidades = unidades.ToArray(), Ossb = id };

            return View(viewModel);
        }

        //
        // GET: /OssbServico/Create

        public async Task<ActionResult> Create(Int32 id = 0, decimal ma_bdi = 0, decimal mo_bdi = 0)
        {
            ViewBag.TERCEIROS = await _db.PESSOA.Where(p => p.TERCEIRO == 1)
                .ToDictionaryAsync(p => p.ID.ToString(), p => p.NOME);

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA");

            ViewBag.ADICIONADOS = await _db.OSSB_SERVICO.Where(o => o.OSSB == id)
                .OrderBy(o => o.DESCRICAO)
                .ThenBy(o => o.ID)
                .ToArrayAsync();

            ViewBag.MA_BDI = ma_bdi.ToString("N2");
            ViewBag.MO_BDI = mo_bdi.ToString("N2");


            return View(new OSSB_SERVICO() { OSSB = id });
        }

        //
        // POST: /OssbServico/Create
        [HttpPost]
        public async Task<ActionResult> AdicionarServico(int ossb, string subdivisao)
        {
            var unidade = await _db.UNIDADE.FirstOrDefaultAsync();

            var s = new OSSB_SERVICO()
            {
                OSSB = ossb,
                SUBDIVISAO = subdivisao.ToUpper(),
                UNIDADE = unidade.ID,
                DESCRICAO = "",
                QUANTIDADE = 1

            };

            _db.OSSB_SERVICO.Add(s);

            await _db.SaveChangesAsync();

            return Json(new
            {
                id = s.ID,
                descricao = s.DESCRICAO,
                unidade = s.UNIDADE.ToString(),
                subdivisao = s.SUBDIVISAO,
                quantidade = s.QUANTIDADE.ToString("0.00"),
                valor_ma = s.VALOR_MA.ToString("0.00"),
                valor_mo = s.VALOR_MO.ToString("0.00"),
                valor_mo_bdi = s.VALOR_MO_BDI.ToString("0.00"),
                valor_ma_bdi = s.VALOR_MA_BDI.ToString("0.00"),
                mo_bdi = (s.VALOR_MO == 0 ? 0 : (s.VALOR_MO_BDI / s.VALOR_MO * 100 - 100)).ToString("0.00"),
                ma_bdi = (s.VALOR_MA == 0 ? 0 : (s.VALOR_MA_BDI / s.VALOR_MA * 100 - 100)).ToString("0.00"),
            }, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public async Task<ActionResult> SalvarDescricao(int id, string descricao)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.DESCRICAO = descricao.ToUpper();

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public async Task<ActionResult> SalvarQuantidade(int id, string quantidade)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.QUANTIDADE = decimal.Parse(quantidade);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public async Task<ActionResult> SalvarUnidade(int id, string unidade)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.UNIDADE = int.Parse(unidade);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public async Task<ActionResult> SalvarValorMo(int id, string valor_mo)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.VALOR_MO = decimal.Parse(valor_mo);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public async Task<ActionResult> SalvarValorMoBdi(int id, string valor_mo_bdi)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.VALOR_MO_BDI = decimal.Parse(valor_mo_bdi);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public async Task<ActionResult> SalvarValorMa(int id, string valor_ma)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.VALOR_MA = decimal.Parse(valor_ma);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public async Task<ActionResult> SalvarValorMaBdi(int id, string valor_ma_bdi)
        {
            var s = await _db.OSSB_SERVICO.FirstOrDefaultAsync(serv => serv.ID == id);

            if (s != null)
            {
                s.VALOR_MA_BDI = decimal.Parse(valor_ma_bdi);

                await _db.SaveChangesAsync();
            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OSSB_SERVICO servico)
        {

            if (servico.DESCRICAO == null || servico.DESCRICAO.Length < 2)
            {
                ModelState.AddModelError("", "Informe uma descrição!");
            }

            if (servico.SUBDIVISAO == null || servico.SUBDIVISAO.Length < 2)
            {
                ModelState.AddModelError("", "Informe uma subdivisão!");
            }

            servico.DESCRICAO = servico.DESCRICAO?.ToUpper().Trim() ?? "";

            servico.SUBDIVISAO = servico.SUBDIVISAO?.ToUpper().Trim() ?? "";


            if (System.Text.RegularExpressions.Regex.IsMatch(servico.DESCRICAO, "OR(C|Ç)AD(O|A)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled)
                || System.Text.RegularExpressions.Regex.IsMatch(servico.SUBDIVISAO, "OR(C|Ç)AD(O|A)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                ModelState.AddModelError("", "O.S Orçadas fora do sistema não são permitidas!");
            }

            if (ModelState.IsValid)
            {
                _db.OSSB_SERVICO.Add(servico);

                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sb.AppendLine();

                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);

                            sb.AppendLine();
                        }
                    }

                    throw new Exception(sb.ToString());
                }


                return RedirectToAction("Create", new { id = servico.OSSB, mo_bdi = servico.VALOR_MO == 0 ? 0 : ((servico.VALOR_MO_BDI / servico.VALOR_MO * 100) - 100), ma_bdi = servico.VALOR_MA == 0 ? 0 : ((servico.VALOR_MA_BDI / servico.VALOR_MA * 100) - 100) });
            }

            ViewBag.TERCEIROS = await _db.PESSOA.Where(p => p.TERCEIRO == 1)
                .ToDictionaryAsync(p => p.ID.ToString(), p => p.NOME);

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", servico.UNIDADE);

            ViewBag.ADICIONADOS = await _db.OSSB_SERVICO.Where(os => os.OSSB == servico.OSSB)
                .GroupBy(os => os.SUBDIVISAO)
                .SelectMany(os => os)
                .ToArrayAsync();

            return View(servico);
        }

        //
        // GET: /OssbServico/Edit/5

        public async Task<ActionResult> Edit(int id = 0)
        {
            var servico = await _db.OSSB_SERVICO
                .FirstOrDefaultAsync(os => os.ID == id);

            if (servico == null)
            {
                return HttpNotFound();
            }

            ViewBag.TERCEIROS = await _db.PESSOA.Where(p => p.TERCEIRO == 1).ToArrayAsync();

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", servico.UNIDADE);

            ViewBag.DESCRICOES = await _db.OSSB_SERVICO.Select(os => os.DESCRICAO).Distinct().ToArrayAsync();

            ViewBag.SUBDIVISOES = await _db.OSSB_SERVICO.Select(os => os.SUBDIVISAO).Distinct().ToArrayAsync();

            ViewBag.MA_BDI = (servico.VALOR_MA == 0 ? 0 : ((servico.VALOR_MA_BDI / servico.VALOR_MA * 100) - 100)).ToString("N2");
            ViewBag.MO_BDI = (servico.VALOR_MO == 0 ? 0 : ((servico.VALOR_MO_BDI / servico.VALOR_MO * 100) - 100)).ToString("N2");

            return View(servico);

        }

        [HttpPost]
        public async Task<ActionResult> DeletarServico(int id = 0)
        {
            var ossbServico = await _db.OSSB_SERVICO
                .FirstOrDefaultAsync(os => os.ID == id);


            if (ossbServico != null)
            {
                _db.Entry(ossbServico)
                    .State = EntityState.Deleted;

                await _db.SaveChangesAsync();

            }

            return Json(null, JsonRequestBehavior.DenyGet);
        }

        //
        // POST: /OssbServico/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OSSB_SERVICO servico)
        {

            if (servico.DESCRICAO == null || servico.DESCRICAO.Length < 2)
            {
                ModelState.AddModelError("", "Informe uma descrição!");
            }

            if (servico.SUBDIVISAO == null || servico.SUBDIVISAO.Length < 2)
            {
                ModelState.AddModelError("", "Informe uma subdivisão!");
            }


            servico.DESCRICAO = servico.DESCRICAO?.ToUpper().Trim() ?? "";

            servico.SUBDIVISAO = servico.SUBDIVISAO?.ToUpper().Trim() ?? "";


            if (System.Text.RegularExpressions.Regex.IsMatch(servico.DESCRICAO, "OR(C|Ç)AD(O|A)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled)
                || System.Text.RegularExpressions.Regex.IsMatch(servico.SUBDIVISAO, "OR(C|Ç)AD(O|A)", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled))
            {
                ModelState.AddModelError("", "O.S Orçadas fora do sistema não são permitidas!");
            }


            if (ModelState.IsValid)
            {
                _db.Entry(servico)
                    .State = EntityState.Modified;

                _db.SaveChanges();

                return RedirectToAction("Index", new { id = servico.OSSB });
            }

            ViewBag.TERCEIROS = await _db.PESSOA.Where(p => p.TERCEIRO == 1).ToArrayAsync();

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", servico.UNIDADE);

            return View(servico);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private bool VerificarServico(string servico)
        {
            var existe = _db.SERVICO.Any(s => s.DESCRICAO == servico);

            return existe;
        }

        public ActionResult BuscarServicos(string term)
        {
            var items = (from s in _db.SERVICO
                         where s.DESCRICAO.Contains(term)
                         where s.SITUACAO == "A"
                         select s.DESCRICAO)
                         .Distinct()
                         .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarAreaManutencao(string term)
        {
            var items = (from a in _db.AREA_MANUTENCAOSet
                         where a.DESCRICAO.Contains(term)
                         where a.SITUACAO == "A"
                         select a.DESCRICAO)
                         .Distinct()
                         .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

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

        public static string EscreverExtenso(decimal valor)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;
                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += Escrever_Valor_Extenso(Convert.ToDecimal(strValor.Substring(i, 3)));
                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " TRILHÃO" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " TRILHÕES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                            valor_por_extenso += " BILHÃO" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                            valor_por_extenso += " BILHÕES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILHÃO" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILHÕES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " E " : string.Empty);
                    if (i == 12)
                    {
                        if (valor_por_extenso.Length > 8)
                            if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" | valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                                valor_por_extenso += " DE";
                            else
                                if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES"
| valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                                valor_por_extenso += " DE";
                            else
                                    if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                                valor_por_extenso += " DE";
                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += " REAL";
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += " REAIS";
                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " E ";
                    }
                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }
        static string Escrever_Valor_Extenso(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));
                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";
                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";
                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";
                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";
                return montagem;
            }
        }

        public FileContentResult GetOrcamentoPDF(int id = 0, int materiais = 0)
        {

            if (Session.IsFuncionario())
            {
                OSSB ossb = _db.OSSB
                    .Include(o => o.PESSOA)
                    .Include(o => o.PROJETO1)
                    .Include(o => o.CONTRATO1)
                    .FirstOrDefault(o => o.ID == id);

                if (ossb != null)
                {

                    try
                    {
                        //  var fs = new FileStream(Server.MapPath("../os" + id + ".pdf"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                        var fs = new MemoryStream();

                        decimal total = 0;

                        Document doc = new Document(PageSize.A4, 15, 15, 30, 65);

                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                        var capturarBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/Capturar.PNG"));

                        var backgroundBytes = System.IO.File.ReadAllBytes(Server.MapPath(@"~/Images/background.png"));



                        writer.PageEvent = new PageEventHandler(this);

                        doc.Open();

                        {
                            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 10);
                            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);


                            {
                                Image img = Image.GetInstance(capturarBytes, true);

                                img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                doc.Add(img);
                            }


                            doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });

                            {
                                var cb = writer.DirectContentUnder;

                                cb.SaveState();


                                var img = Image.GetInstance(backgroundBytes, true);

                                Rectangle r = new Rectangle(doc.PageSize);

                                img.SetAbsolutePosition(doc.PageSize.GetRight(doc.RightMargin) - img.Right, doc.PageSize.GetBottom(doc.BottomMargin) - img.Bottom);

                                cb.AddImage(img);

                                cb.RestoreState();
                            }

                            doc.Add(new Paragraph(new Chunk("São Paulo SP, " + System.DateTime.Today.ToLongDateString(), normalFont)));
                            doc.Add(new Paragraph(Environment.NewLine));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk(ossb.PESSOA.RAZAO, normalFont)));

                            if (ossb.LOJA == null)
                            {
                                doc.Add(new Paragraph(new Chunk(ossb.PESSOA.ENDERECO + (String.IsNullOrEmpty(ossb.PESSOA.COMPLEMENTO) ? "" : (", " + ossb.PESSOA.COMPLEMENTO)), normalFont)));
                            }
                            else
                            {
                                doc.Add(new Paragraph(new Chunk(ossb.LOJA1.APELIDO, normalFont)));
                                doc.Add(new Paragraph(new Chunk(ossb.LOJA1.ENDERECO + (String.IsNullOrEmpty(ossb.LOJA1.COMPLEMENTO) ? "" : (", " + ossb.LOJA1.COMPLEMENTO)), normalFont)));
                            }

                            doc.Add(new Paragraph(Environment.NewLine));

                            var phrase = new Phrase();

                            phrase.Add(new Chunk("At: ", normalFont));
                            phrase.Add(new Chunk(ossb.PESSOA.CONTATO, boldFont));


                            doc.Add(new Paragraph(Environment.NewLine));

                            phrase = new Phrase();

                            phrase.Add(new Chunk("Ref.: ", boldFont));
                            phrase.Add(new Chunk(ossb.OCORRENCIA, normalFont));

                            doc.Add(new Paragraph(phrase));

                            doc.Add(new Paragraph(Environment.NewLine));

                            phrase = new Phrase();

                            phrase.Add(new Chunk("O.S: ", boldFont));
                            phrase.Add(new Chunk(ossb.ID.ToString(), normalFont));

                            doc.Add(new Paragraph(phrase));

                            doc.Add(new Paragraph(Environment.NewLine));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("Estamos apresentando nossa Proposta Técnica e Comercial referente à execução dos serviços em pauta.", normalFont)));

                            doc.Add(new Paragraph(Environment.NewLine));
                            doc.Add(new Paragraph(Environment.NewLine));

                            phrase = new Phrase();

                            phrase.Add(new Chunk("Atendendo à solicitação que nos foi formulada, apresentamos nossa ", normalFont));
                            phrase.Add(new Chunk("PROPOSTA TÉCNICA E COMÉRCIAL", boldFont));
                            phrase.Add(new Chunk(" para prestação de serviços.", normalFont));

                            doc.Add(phrase);

                            doc.Add(new Paragraph(Environment.NewLine));

                            phrase = new Phrase();

                            phrase.Add(new Chunk("A nossa atuação será desenvolvida com base nos preceitos estabelecidos na ", normalFont));
                            phrase.Add(new Chunk("POLÍTICA DA QUALIDADE da SÃO BENTO ENGENHARIA", boldFont));
                            phrase.Add(new Chunk(" e as atividades de manutenção e obras serão executadas em conformidade com os Procedimentos de Engenharia e Manutenção, desenvolvidos pela ", normalFont));
                            phrase.Add(new Chunk("SÃO BENTO ENGENHARIA", boldFont));
                            phrase.Add(new Chunk(", ao longo de anos de atuação no mercado.", normalFont));

                            doc.Add(phrase);

                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("Colocamo-nos à disposição para quaisquer esclarecimentos necessários.", normalFont)) { Alignment = Element.ALIGN_CENTER });

                            doc.NewPage();
                        }
                        {
                            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 10);
                            var normalDesc = FontFactory.GetFont(FontFactory.TIMES, 9);
                            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                            var titleBold = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);

                            var albuns = _db.OSSB_ALBUM
                                .Include(ab => ab.OSSB_IMAGEM)
                                .Include(ab => ab.OSSB_IMAGEM.Select(im => im.ANEXO1))
                                .Where(ab => ab.OSSB == id)
                                .ToArray();


                            foreach (var album in albuns)
                            {
                                foreach (var imageSplit in OssbImagemController.TakeChunks(album.OSSB_IMAGEM, 4))
                                {
                                    doc.NewPage();

                                    {
                                        Image img = Image.GetInstance(capturarBytes, true);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                        doc.Add(img);

                                        doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });
                                    }

                                    PdfPTable table;

                                    if (imageSplit.Count() == 1)
                                    {
                                        table = new PdfPTable(imageSplit.Count());
                                    }
                                    else
                                    {
                                        table = new PdfPTable(2);
                                    }

                                    table.WidthPercentage = 100;



                                    foreach (var image in imageSplit)
                                    {


                                        var cell = new PdfPCell()
                                        {
                                            BorderWidth = 1,
                                            Padding = 3
                                        };



                                        Image img = Image.GetInstance(image.ANEXO1.BUFFER, true);
                                        img.ScaleToFit(250, 200);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                                        cell.AddElement(img);

                                        var p = new Paragraph(new Chunk(image.DESCRICAO, normalDesc));

                                        p.Alignment = Element.ALIGN_CENTER;

                                        cell.AddElement(p);

                                        cell.FixedHeight = 300;

                                        table.AddCell(cell);
                                    }

                                    if (imageSplit.Count() % 2 == 1 && imageSplit.Count() > 1)
                                    {
                                        table.AddCell(new PdfPCell());
                                    }

                                    doc.Add(table);
                                }

                                if (!String.IsNullOrWhiteSpace(album.DESCRICAO))
                                {
                                    doc.NewPage();

                                    {
                                        Image img = Image.GetInstance(capturarBytes, true);

                                        img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                        doc.Add(img);

                                        doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });
                                    }

                                    doc.Add(new Paragraph(new Chunk("CONCLUSÃO", titleBold)) { Alignment = Element.ALIGN_CENTER });

                                    doc.Add(new Paragraph(new Chunk(album.DESCRICAO, normalDesc)) { Alignment = Element.ALIGN_CENTER });
                                }
                            }

                            doc.NewPage();
                        }

                        {
                            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);
                            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);

                            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);


                            IEnumerable<OSSB_SERVICO> servicosQuery = ossb.OSSB_SERVICO;

                            if (materiais == 1)
                            {
                                servicosQuery = servicosQuery.Where(s => s.VALOR_MO == 0 && s.VALOR_MO_BDI == 0 && (s.VALOR_MA > 0 || s.VALOR_MA_BDI > 0));
                            }

                            var servicos = servicosQuery
                                .GroupBy(s => s.SUBDIVISAO)
                                .ToArray();

                            if (servicos.Any())
                            {

                                {
                                    var img = Image.GetInstance(capturarBytes, true);

                                    img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                    doc.Add(img);

                                    doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });
                                }

                                {
                                    var cb = writer.DirectContentUnder;

                                    cb.SaveState();

                                    var img = Image.GetInstance(backgroundBytes, true);

                                    Rectangle r = new Rectangle(doc.PageSize);

                                    img.SetAbsolutePosition(doc.PageSize.GetRight(doc.RightMargin) - img.Right, doc.PageSize.GetBottom(doc.BottomMargin) - img.Bottom);

                                    cb.AddImage(img);

                                    cb.RestoreState();
                                }

                                doc.Add(new Paragraph(Environment.NewLine));


                                PdfPTable table = new PdfPTable(9)

                                {
                                    WidthPercentage = 100
                                };

                                table.SetWidths(new float[] { PageSize.A4.Width * 0.08f, PageSize.A4.Width * 0.25f, PageSize.A4.Width * 0.08f, PageSize.A4.Width * 0.08f, PageSize.A4.Width * 0.12f, PageSize.A4.Width * 0.12f, PageSize.A4.Width * 0.12f, PageSize.A4.Width * 0.12f, PageSize.A4.Width * 0.1f });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("ITEM", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("SERVIÇO / PRODUTO", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("QTD", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("UNID", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("UNITÁRIO MÃO OBRA", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("UNITÁRIO MATERIAL", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2,
                                });


                                table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL MÃO OBRA", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2,
                                });


                                table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL MATERIAL", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2,
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", boldWhiteFont)))
                                {
                                    BackgroundColor = BaseColor.DARK_GRAY,
                                    Rowspan = 2,
                                });

                                int cell = 1;


                                foreach (var key in servicos)
                                {

                                    int innerCell = 1;

                                    var first = key.First();

                                    table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString(), boldFont)))
                                    {
                                        BackgroundColor = BaseColor.LIGHT_GRAY
                                    });

                                    table.AddCell(new PdfPCell(new Phrase(new Chunk(first.SUBDIVISAO, boldFont)))
                                    {
                                        BackgroundColor = BaseColor.LIGHT_GRAY
                                    });

                                    table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                                    {
                                        BackgroundColor = BaseColor.LIGHT_GRAY,
                                        Colspan = 6,
                                    });

                                    decimal innerTotal = 0;

                                    foreach (var value in key)
                                    {
                                        innerTotal += ((value.VALOR_MA_BDI + value.VALOR_MO_BDI) * value.QUANTIDADE);
                                    }

                                    table.AddCell(new PdfPCell(new Phrase(new Chunk(innerTotal.ToString("C"), boldFont)))
                                    {
                                        BackgroundColor = BaseColor.LIGHT_GRAY,
                                    });

                                    total += innerTotal;

                                    foreach (var value in key)
                                    {
                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString() + "." + innerCell.ToString(), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(value.DESCRICAO.ToString(), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(value.QUANTIDADE.ToString(), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(value.UNIDADE1.SIGLA, normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MO_BDI.ToString("C"), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MA_BDI.ToString("C"), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MO_BDI * value.QUANTIDADE).ToString("C"), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MA_BDI * value.QUANTIDADE).ToString("C"), normalFont))));

                                        table.AddCell(new PdfPCell(new Phrase(new Chunk(((value.VALOR_MA_BDI + value.VALOR_MO_BDI) * value.QUANTIDADE).ToString("C"), normalFont))));

                                        innerCell += 1;
                                    }


                                    table.AddCell(new PdfPCell(new Phrase("", normalFont))
                                    {
                                        Colspan = 9
                                    });

                                    cell += 1;
                                }

                                table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                                {
                                    BackgroundColor = BaseColor.LIGHT_GRAY,
                                    Colspan = 8,
                                });

                                table.AddCell(new PdfPCell(new Phrase(new Chunk(total.ToString("C"), boldFont)))
                                {
                                    BackgroundColor = BaseColor.LIGHT_GRAY,
                                    Colspan = 1,
                                });


                                doc.Add(table);
                            }

                            doc.NewPage();
                        }

                        {
                            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 10);
                            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);

                            {
                                Image img = Image.GetInstance(capturarBytes, true);

                                img.Alignment = iTextSharp.text.Image.ALIGN_CENTER;

                                doc.Add(img);
                            }


                            doc.Add(new LineSeparator() { LineColor = new BaseColor(98, 36, 35) });

                            {
                                var cb = writer.DirectContentUnder;

                                cb.SaveState();


                                var img = Image.GetInstance(backgroundBytes, true);

                                Rectangle r = new Rectangle(doc.PageSize);

                                img.SetAbsolutePosition(doc.PageSize.GetRight(doc.RightMargin) - img.Right, doc.PageSize.GetBottom(doc.BottomMargin) - img.Bottom);

                                cb.AddImage(img);

                                cb.RestoreState();
                            }

                            doc.Add(new Paragraph(new Chunk("3 – PREÇOS", boldFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk(total.ToString("C") + " (" + EscreverExtenso(total) + ")", normalFont)));

                            doc.Add(new Paragraph(Environment.NewLine));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("3 - CONDIÇÕES E PRAZO DE PAGAMENTO", boldFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk(ossb.PRAZO_PAGAMENTO ?? "   Á combinar.", normalFont)));
                            doc.Add(new Paragraph(Environment.NewLine));


                            doc.Add(new Paragraph(new Chunk("4 - PRAZO DE EXECUÇÃO", boldFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk(ossb.PRAZO_EXECUCAO ?? "   De acordo com a periodicidade e cronograma a ser enviado no acordo.", normalFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("5 - VALIDADE DA PROPOSTA", boldFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("   15 (quinze) dias.", normalFont)));
                            doc.Add(new Paragraph(Environment.NewLine));

                            doc.Add(new Paragraph(new Chunk("Colocamo-nos à disposição para quaisquer esclarecimentos.", normalFont)));
                            doc.Add(new Paragraph(new Chunk("Atenciosamente,", normalFont)));

                            doc.Add(new Paragraph(new Chunk("       São Bento Engenharia", normalFont)));

                            doc.NewPage();
                        }


                        doc.Close();
                        writer.Close();

                        Response.AppendHeader("Content-Disposition", "inline; filename=orçamento - " + ossb.ID + ".pdf;");

                        return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);
                    }
                    catch (Exception)
                    {

                    }
                }

                return null;
            }
            else
                return null;
        }

        public async Task<FileContentResult> GetEscopoPDF(int id = 0)
        {
            var servicos = await _db
                .OSSB_SERVICO
                .Where(oss => oss.OSSB == id)
                .GroupBy(oss => oss.SUBDIVISAO)
                .Select(g => new { SUBDIVISAO = g.Key, SERVICOS = g.OrderBy(s => s.ID) })
                .ToArrayAsync();


            var fs = new MemoryStream();


            Document doc = new Document(PageSize.A4, 15, 15, 15, 15);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            PdfPTable table = new PdfPTable(4);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);
            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);


            table.WidthPercentage = 100;

            table.SetWidths(new float[] { PageSize.A4.Width * 0.1f, PageSize.A4.Width * 0.7f, PageSize.A4.Width * 0.1f, PageSize.A4.Width * 0.1f });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("ITEM", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Rowspan = 2
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("SERVIÇO / PRODUTO", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Rowspan = 2
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("QTD", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Rowspan = 2
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("UNID", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Rowspan = 2
            });

            int cell = 1;


            foreach (var key in servicos.OrderBy(s => s.SERVICOS.FirstOrDefault().ID))
            {

                int innerCell = 1;

                table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString(), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.SUBDIVISAO, boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });


                foreach (var value in key.SERVICOS)
                {
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString() + "." + innerCell.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.DESCRICAO.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.QUANTIDADE.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.UNIDADE1.SIGLA, normalFont))));

                    innerCell += 1;
                }

                cell += 1;
            }

            doc.Add(table);

            doc.Close();
            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=escopo - " + id + ".pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);


        }

        public async Task<FileContentResult> GetComparativoPDF(int id = 0)
        {
            var servicos = await _db
                .OSSB_SERVICO
                .Where(oss => oss.OSSB == id)
                .GroupBy(oss => oss.SUBDIVISAO)
                .ToArrayAsync();


            var fs = new MemoryStream();


            Document doc = new Document(PageSize.A4.Rotate(), 15, 15, 15, 15);

            var pageSizeWidth = PageSize.A4.Rotate().Width;

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            PdfPTable table = new PdfPTable(14);

            var normalFont = FontFactory.GetFont(FontFactory.TIMES, 8);
            var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);

            var boldWhiteFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.WHITE);


            table.WidthPercentage = 100;

            table.SetWidths(new float[] { pageSizeWidth * 0.05f, pageSizeWidth * 0.25f, pageSizeWidth * 0.05f, pageSizeWidth * 0.05f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f, pageSizeWidth * 0.06f });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("ITEM", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("SERVIÇO / PRODUTO", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("QTD", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("UNID", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("UNIT MÃO DE OBRA", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 2,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("UNIT MATERIAL", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 2,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL MÃO DE OBRA", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 2,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL MATERIAL", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 2,
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("TOTAL", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 2,
            });

            //

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldWhiteFont)))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 4,
            });

            for (Int32 i = 0; i < 5; ++i)
            {
                table.AddCell(new PdfPCell(new Phrase(new Chunk("SEM BDI", boldWhiteFont)))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("COM BDI", boldWhiteFont)))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                });

            }

            int cell = 1;


            foreach (var key in servicos.OrderBy(s => s.First().ID))
            {

                int innerCell = 1;


                table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString(), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Key, boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => s.VALOR_MO).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => s.VALOR_MO_BDI).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => s.VALOR_MA).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => s.VALOR_MA_BDI).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });


                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => (s.VALOR_MO * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => (s.VALOR_MO_BDI * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => (s.VALOR_MA * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => (s.VALOR_MA_BDI * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => ((s.VALOR_MA + s.VALOR_MO) * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                table.AddCell(new PdfPCell(new Phrase(new Chunk(key.Select(s => ((s.VALOR_MA_BDI + s.VALOR_MO_BDI) * s.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                });

                

                foreach (var value in key.OrderBy(s => s.ID))
                {
                    table.AddCell(new PdfPCell(new Phrase(new Chunk(cell.ToString() + "." + innerCell.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.DESCRICAO.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.QUANTIDADE.ToString(), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.UNIDADE1.SIGLA, normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MO.ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MO_BDI.ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MA.ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(value.VALOR_MA_BDI.ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MO * value.QUANTIDADE).ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MO_BDI * value.QUANTIDADE).ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MA * value.QUANTIDADE).ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk((value.VALOR_MA_BDI * value.QUANTIDADE).ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(((value.VALOR_MA + value.VALOR_MO) * value.QUANTIDADE).ToString("N2"), normalFont))));

                    table.AddCell(new PdfPCell(new Phrase(new Chunk(((value.VALOR_MA_BDI + value.VALOR_MO_BDI) * value.QUANTIDADE).ToString("N2"), normalFont))));

                    innerCell += 1;
                }

                cell += 1;
            }

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk("", boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MO)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MO_BDI)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MA)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MA_BDI)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MO * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MO_BDI * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MA * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => ss.VALOR_MA_BDI * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => (ss.VALOR_MA + ss.VALOR_MO) * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });

            table.AddCell(new PdfPCell(new Phrase(new Chunk(servicos.SelectMany(s => s.Select(ss => (ss.VALOR_MA_BDI + ss.VALOR_MO_BDI) * ss.QUANTIDADE)).DefaultIfEmpty().Sum().ToString("N2"), boldFont)))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY
            });



            doc.Add(table);

            doc.Close();
            writer.Close();

            Response.AppendHeader("Content-Disposition", "inline; filename=escopo - " + id + ".pdf;");

            return new FileContentResult(fs.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf);


        }


    }

}