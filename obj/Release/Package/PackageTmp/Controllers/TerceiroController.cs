using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    /*
    public class TerceiroController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        private CookieContainer _cookies;
        public readonly string UrlBaseReceitaFederal;
        public readonly string PaginaValidacao;
        public readonly string PaginaPrincipal;
        public readonly string PaginaCaptcha;

        public TerceiroController()
        {
            _cookies = new CookieContainer();
            UrlBaseReceitaFederal = "http://www.receita.fazenda.gov.br/pessoajuridica/cnpj/cnpjreva/";
            PaginaValidacao = "valida.asp";
            PaginaPrincipal = "cnpjreva_solicitacao2.asp";
            PaginaCaptcha = "captcha/gerarCaptcha.asp";
        }

        //
        // GET: /Terceiro/

        public ActionResult Index()
        {
            return View(_db.PESSOA.Where(p => p.TERCEIRO == 1).ToList());
        }

        //
        // GET: /Terceiro/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Terceiro/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PESSOA pessoa)
        {
            var existe = _db.PESSOA.Any(x => x.NUM_DOC == pessoa.NUM_DOC && x.TIPO == "T");

            if (existe)
                ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outro Terceiro!");

            if (pessoa.NUM_DOC.Length < 14)
                ModelState.AddModelError(string.Empty, "Informe um Número de Documento válido!");

            if ((string.IsNullOrEmpty(pessoa.TELEFONE1)) && (string.IsNullOrEmpty(pessoa.TELEFONE2)) &&
                (string.IsNullOrEmpty(pessoa.TELEFONE3)))
                ModelState.AddModelError(string.Empty, "Informe pelo menos um telefone para contato!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE1)) && (pessoa.TELEFONE1.Length != 14))
                ModelState.AddModelError(string.Empty, "Telefone Inválido!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE2)) && (pessoa.TELEFONE2.Length != 15))
                ModelState.AddModelError(string.Empty, "Celular Inválido!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE3)) && (pessoa.TELEFONE3.Length != 14))
                ModelState.AddModelError(string.Empty, "Tel. Contato Inválido!");

            if (!ModelState.IsValid) 
                return View(pessoa);

            pessoa.TIPO = "T";

            switch (pessoa.NUM_DOC.Length)
            {
                case 18:
                    pessoa.TIPO_DOC = "J";
                    break;
                case 14:
                    pessoa.TIPO_DOC = "F";
                    break;
            }

            if (!string.IsNullOrEmpty(pessoa.RAZAO))
                pessoa.RAZAO = pessoa.RAZAO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.NOME))
                pessoa.NOME = pessoa.NOME.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.ENDERECO))
                pessoa.ENDERECO = pessoa.ENDERECO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.COMPLEMENTO))
                pessoa.COMPLEMENTO = pessoa.COMPLEMENTO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.BAIRRO))
                pessoa.BAIRRO = pessoa.BAIRRO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.CIDADE))
                pessoa.CIDADE = pessoa.CIDADE.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.UF))
                pessoa.UF = pessoa.UF.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.ZONA))
                pessoa.ZONA = pessoa.ZONA.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.CONTATO))
                pessoa.CONTATO = pessoa.CONTATO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.OBSERVACAO))
                pessoa.OBSERVACAO = pessoa.OBSERVACAO.ToUpper();
            

            _db.PESSOA.Add(pessoa);
            _db.SaveChanges();
           

            return RedirectToAction("Index");
        }

        //
        // GET: /Terceiro/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var pessoa = _db.PESSOA.Find(id);

            if (pessoa == null)
            {
                return HttpNotFound();
            }

            return View(pessoa);
        }

        //
        // POST: /Terceiro/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PESSOA pessoa)
        {
            var existe = _db.PESSOA.Any(x => x.NUM_DOC == pessoa.NUM_DOC && x.TIPO == "T" && x.ID != pessoa.ID);

            if (existe)
                ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outro Fornecedor!");

            if (pessoa.NUM_DOC.Length < 14)
                ModelState.AddModelError(string.Empty, "Informe um Número de Documento válido!");

            if ((string.IsNullOrEmpty(pessoa.TELEFONE1)) && (string.IsNullOrEmpty(pessoa.TELEFONE2)) && (string.IsNullOrEmpty(pessoa.TELEFONE3)))
                ModelState.AddModelError(string.Empty, "Informe pelo menos um número de telefone!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE1)) && (pessoa.TELEFONE1.Length != 14))
                ModelState.AddModelError(string.Empty, "Telefone Inválido!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE2)) && (pessoa.TELEFONE2.Length != 15))
                ModelState.AddModelError(string.Empty, "Celular Inválido!");

            if ((!string.IsNullOrEmpty(pessoa.TELEFONE3)) && (pessoa.TELEFONE3.Length != 14))
                ModelState.AddModelError(string.Empty, "Tel. Contato Inválido!");

            if (!ModelState.IsValid) 
                return View(pessoa);

            pessoa.TIPO = "T";

            switch (pessoa.NUM_DOC.Length)
            {
                case 18:
                    pessoa.TIPO_DOC = "J";
                    break;
                case 14:
                    pessoa.TIPO_DOC = "F";
                    break;
            }

            if (!string.IsNullOrEmpty(pessoa.RAZAO))
                pessoa.RAZAO = pessoa.RAZAO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.NOME))
                pessoa.NOME = pessoa.NOME.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.ENDERECO))
                pessoa.ENDERECO = pessoa.ENDERECO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.COMPLEMENTO))
                pessoa.COMPLEMENTO = pessoa.COMPLEMENTO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.BAIRRO))
                pessoa.BAIRRO = pessoa.BAIRRO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.CIDADE))
                pessoa.CIDADE = pessoa.CIDADE.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.UF))
                pessoa.UF = pessoa.UF.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.ZONA))
                pessoa.ZONA = pessoa.ZONA.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.CONTATO))
                pessoa.CONTATO = pessoa.CONTATO.ToUpper();

            if (!string.IsNullOrEmpty(pessoa.OBSERVACAO))
                pessoa.OBSERVACAO = pessoa.OBSERVACAO.ToUpper();

            _db.Entry(pessoa).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult BuscarZona(string term)
        {
            var items = (from z in _db.PESSOA
                         where z.ZONA.StartsWith(term)
                         select z.ZONA).Distinct().ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
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
    */
}