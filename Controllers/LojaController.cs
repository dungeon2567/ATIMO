using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using ATIMO;
using System.Data.Entity.Infrastructure;

namespace Atimo.Controllers
{
    public class LojaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        private CookieContainer _cookies;
        public readonly string UrlBaseReceitaFederal;
        public readonly string PaginaValidacao;
        public readonly string PaginaPrincipal;
        public readonly string PaginaCaptcha;

        public LojaController()
        {
            _cookies = new CookieContainer();
            UrlBaseReceitaFederal = "http://www.receita.fazenda.gov.br/pessoajuridica/cnpj/cnpjreva/";
            PaginaValidacao = "valida.asp";
            PaginaPrincipal = "cnpjreva_solicitacao2.asp";
            PaginaCaptcha = "captcha/gerarCaptcha.asp";
        }

        //
        // GET: /Loja/

        public ActionResult Index(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var loja = _db
                .LOJA
                .Where(l => l.CLIENTE == id);

                return View(new LojaIndexViewModel() { LOJAS = loja.ToArray(), CLIENTE = id });
            }
            else
                return RedirectToAction("", "");
        }

        //
        // GET: /Loja/Create

        public ActionResult Create(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                return View(new LOJA() { CLIENTE = id });
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /Loja/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LOJA loja)
        {
            var existe = ((await _db.PESSOA.FirstAsync(p => p.ID == loja.CLIENTE)).NUM_DOC != loja.NUM_DOC && await _db
            .LOJA
            .AnyAsync(x => x.NUM_DOC == loja.NUM_DOC));

            if (existe)
                ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outra Loja!");

            if (loja.NUM_DOC.Length < 14)
                ModelState.AddModelError(string.Empty, "Informe um Número de Documento válido!");

            if ((string.IsNullOrEmpty(loja.TELEFONE1)) && (string.IsNullOrEmpty(loja.TELEFONE2)) &&
                (string.IsNullOrEmpty(loja.TELEFONE3)))
                ModelState.AddModelError(string.Empty, "Informe pelo menos um telefone para contato!");

            if (!ModelState.IsValid)
                return View(loja);

            if (!string.IsNullOrEmpty(loja.APELIDO))
                loja.APELIDO = loja.APELIDO.ToUpper();

            if (!string.IsNullOrEmpty(loja.ENDERECO))
                loja.ENDERECO = loja.ENDERECO.ToUpper();

            if (!string.IsNullOrEmpty(loja.COMPLEMENTO))
                loja.COMPLEMENTO = loja.COMPLEMENTO.ToUpper();

            if (!string.IsNullOrEmpty(loja.BAIRRO))
                loja.BAIRRO = loja.BAIRRO.ToUpper();

            if (!string.IsNullOrEmpty(loja.CIDADE))
                loja.CIDADE = loja.CIDADE.ToUpper();

            if (!string.IsNullOrEmpty(loja.UF))
                loja.UF = loja.UF.ToUpper();

            if (!string.IsNullOrEmpty(loja.ZONA))
                loja.ZONA = loja.ZONA.ToUpper();

            if (!string.IsNullOrEmpty(loja.CONTATO))
                loja.CONTATO = loja.CONTATO.ToUpper();

            if (!string.IsNullOrEmpty(loja.OBSERVACAO))
                loja.OBSERVACAO = loja.OBSERVACAO.ToUpper();

            _db.LOJA.Add(loja);

            try
            {

                _db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                string s = "";

                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    // Get entry

                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    // Display or log error messages

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        string message = string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                        s += message;
                    }
                }


                throw new System.Exception(s);


            }
            return RedirectToAction("Index", new { id = loja.CLIENTE });
        }

        //
        // GET: /Loja/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var loja = _db.LOJA.Find(id);

            if (loja == null)
            {
                return HttpNotFound();
            }


            return View(loja);
        }

        //
        // POST: /Loja/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LOJA loja)
        {

            var existe = ((await _db.PESSOA.FirstAsync(p => p.ID == loja.CLIENTE)).NUM_DOC != loja.NUM_DOC && await _db
.LOJA
.AnyAsync(x => x.NUM_DOC == loja.NUM_DOC && x.ID != loja.ID));


            if (existe)
                ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outra Loja!");

            if (existe)
                ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outra Loja!");

            if (loja.NUM_DOC.Length < 14)
                ModelState.AddModelError(string.Empty, "Informe um Número de Documento válido!");

            if ((string.IsNullOrEmpty(loja.TELEFONE1)) && (string.IsNullOrEmpty(loja.TELEFONE2)) &&
                (string.IsNullOrEmpty(loja.TELEFONE3)))
                ModelState.AddModelError(string.Empty, "Informe pelo menos um telefone para contato!");

            if (!ModelState.IsValid)
                return View(loja);

            if (!string.IsNullOrEmpty(loja.APELIDO))
                loja.APELIDO = loja.APELIDO.ToUpper();

            if (!string.IsNullOrEmpty(loja.ENDERECO))
                loja.ENDERECO = loja.ENDERECO.ToUpper();

            if (!string.IsNullOrEmpty(loja.COMPLEMENTO))
                loja.COMPLEMENTO = loja.COMPLEMENTO.ToUpper();

            if (!string.IsNullOrEmpty(loja.BAIRRO))
                loja.BAIRRO = loja.BAIRRO.ToUpper();

            if (!string.IsNullOrEmpty(loja.CIDADE))
                loja.CIDADE = loja.CIDADE.ToUpper();

            if (!string.IsNullOrEmpty(loja.UF))
                loja.UF = loja.UF.ToUpper();

            if (!string.IsNullOrEmpty(loja.ZONA))
                loja.ZONA = loja.ZONA.ToUpper();

            if (!string.IsNullOrEmpty(loja.CONTATO))
                loja.CONTATO = loja.CONTATO.ToUpper();

            if (!string.IsNullOrEmpty(loja.OBSERVACAO))
                loja.OBSERVACAO = loja.OBSERVACAO.ToUpper();

            _db.Entry(loja)
                .State = EntityState.Modified;

            _db.SaveChanges();

            return RedirectToAction("Index", new { id = loja.CLIENTE });
        }


        public ActionResult NovaLoja(Int32 cliente, string endereco, string apelido)
        {
            LOJA loja = new LOJA()
            {
                CLIENTE = cliente,
                APELIDO = apelido?.ToUpper() ?? "",
                ENDERECO = endereco?.ToUpper() ?? "",
                BAIRRO = "",
                CEP = "",
                NUM_DOC = "",

                CIDADE = "",
                COMPLEMENTO = "",
                CONTATO = "",
                EMAIL = "",
                OBSERVACAO = "",
                SITUACAO = "A",
                TELEFONE1 = "",
                TELEFONE2 = "",
                TELEFONE3 = "",
                UF = "",
                ZONA = "",
            };

            _db.LOJA.Add(loja);

            _db.SaveChanges();

            return Json(new { id = loja.ID, apelido = loja.APELIDO }, JsonRequestBehavior.AllowGet);

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public async Task<ActionResult> BuscarZona(string term)
        {
            var items = (from z in _db.PESSOA
                         where z.ZONA.StartsWith(term)
                         select z.ZONA)
                         .Distinct()
                         .ToListAsync();

            return Json(await items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCaptcha()
        {

            string htmlResult;

            using (var wc = new Infra.CookieAwareWebClient(_cookies))
            {
                wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; Synapse)";
                wc.Headers[HttpRequestHeader.KeepAlive] = "300";
                htmlResult = wc.DownloadString(UrlBaseReceitaFederal + PaginaPrincipal);
            }

            if (htmlResult.Length <= 0)
                return null;

            var wc2 = new Infra.CookieAwareWebClient(_cookies);
            wc2.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; Synapse)";
            wc2.Headers[HttpRequestHeader.KeepAlive] = "300";
            var data = wc2.DownloadData(UrlBaseReceitaFederal + PaginaCaptcha);

            Session["cookies"] = _cookies;

            return Json("data:image/jpeg;base64," + Convert.ToBase64String(data, 0, data.Length), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConsultarDados(string cnpj, string captcha)
        {
            var msg = string.Empty;
            var resp = ObterDados(cnpj, captcha);

            if (resp.Contains("Verifique se o mesmo foi digitado corretamente"))
                msg = "O número do CNPJ não foi digitado corretamente";

            if (resp.Contains("Erro na Consulta"))
                msg += "Os caracteres não conferem com a imagem";


            return Json(
                new
                {
                    erro = msg,
                    dados = resp.Length > 0 ? Infra.FormatarDados.MontarObjEmpresa(cnpj, resp) : null
                },
                JsonRequestBehavior.DenyGet);
        }

        private string ObterDados(string aCnpj, string aCaptcha)
        {
            _cookies = (CookieContainer)Session["cookies"];

            var request = (HttpWebRequest)WebRequest.Create(UrlBaseReceitaFederal + PaginaValidacao);
            request.ProtocolVersion = HttpVersion.Version10;
            request.CookieContainer = _cookies;
            request.Method = "POST";

            var postData = string.Empty;
            postData += "origem=comprovante&";
            postData += "cnpj=" + new Regex(@"[^\d]").Replace(aCnpj, string.Empty) + "&";
            postData += "txtTexto_captcha_serpro_gov_br=" + aCaptcha + "&";
            postData += "submit1=Consultar&";
            postData += "search_type=cnpj";

            var byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var stHtml = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
            return stHtml.ReadToEnd();
        }
    }
}