using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ATIMO.Models;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using ATIMO;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using ATIMO.Models.Faturamento;

namespace Atimo.Controllers
{
    public class PessoaController : FuncionarioController
    {

        private readonly ATIMOEntities _db = new ATIMOEntities();
        private CookieContainer _cookies;
        public readonly string UrlBaseReceitaFederal;
        public readonly string PaginaValidacao;
        public readonly string PaginaPrincipal;
        public readonly string PaginaCaptcha;

        public PessoaController()
        {
            _cookies = new CookieContainer();
            UrlBaseReceitaFederal = "http://www.receita.fazenda.gov.br/pessoajuridica/cnpj/cnpjreva/";
            PaginaValidacao = "valida.asp";
            PaginaPrincipal = "cnpjreva_solicitacao2.asp";
            PaginaCaptcha = "captcha/gerarCaptcha.asp";
        }

        //
        // GET: /Fornecedor/

        public async Task<ActionResult> SatisfacaoPorCliente(int cliente = 0, String mes_ano = null)
        {
            if (mes_ano == null)
               mes_ano =  DateTime.Today.ToString("MM/yyyy");

            Int32 MES = Int32.Parse(mes_ano.Substring(0, 2));

            Int32 ANO = Int32.Parse(mes_ano.Substring(3, 4));

            if (cliente == 0)
                return HttpNotFound();

            ViewBag.MES_ANO = mes_ano;

            ViewBag.CLIENTE = cliente;

            DateTime start = new DateTime(ANO, MES, 1);

            DateTime end = start.AddMonths(1);

            var query = await (from cs in _db.CLIENTE_SATISFACAO.Where(cs => cs.OSSB1.CLIENTE == cliente && cs.OSSB1.OSSB_CHECK_LIST.Any() && cs.OSSB1.OSSB_CHECK_LIST.Any(ocl => ocl.VISITADO != null && ocl.VISITADO >=  start && ocl.VISITADO < end))
                               group cs by cs.MEDIA into g
                               select new
                               {
                                   VALOR = g.Key,
                                   QUANTIDADE = g.Count()
                               })
                .ToDictionaryAsync(cs => cs.VALOR, cs => cs.QUANTIDADE);

            return View(query);
        }

        public async Task<ActionResult> Index(string nome = null, string doc = null, int page = 1)
        {

            IQueryable<PESSOA> pessoas = _db
            .PESSOA;

            ViewBag.QueryParams = new Dictionary<string, object>();

            if (nome != null)
            {
                ViewBag.QueryParams["nome"] = nome;

                pessoas = pessoas.Where(p => p.NOME.Contains(nome) || p.RAZAO.Contains(nome));
            }

            if (doc != null)
            {

                ViewBag.QueryParams["doc"] = doc;

                pessoas = pessoas.Where(p => p.NUM_DOC.StartsWith(doc));
            }


            pessoas = pessoas.OrderBy(p => p.ID);

            ViewBag.PAGE = page;
            ViewBag.PAGE_COUNT = ((await pessoas.CountAsync() - 1) / 10) + 1;

            pessoas = pessoas
                .Skip((page - 1) * 10)
                .Take(10);

            return View(await pessoas.ToArrayAsync());

        }

        public ActionResult Search()
        {
            if (Session.IsFuncionario())
            {
                return View();
            }
            else
                return RedirectToAction("", "");
        }

        //
        // GET: /Fornecedor/Create

        public ActionResult Create()
        {
            if (Session.IsFuncionario())
            {
                ViewBag.COMO_CONHECEU = new SelectList(_db
                .COMO_CONHECEU
                .Where(c => c.SITUACAO == "A"), "ID", "DESCRICAO");

                return View();
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /Fornecedor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PESSOA pessoa)
        {
                var existe = _db
                .PESSOA
                .Any(x => x.NUM_DOC == pessoa.NUM_DOC);

                if (existe)
                    ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outro Fornecedor!");

                if (pessoa.CLIENTE == 0 && pessoa.FUNCIONARIO == 0 && pessoa.FORNECEDOR == 0 && pessoa.TERCEIRO == 0)
                    ModelState.AddModelError(string.Empty, "A pessoa deve ter pelo menos um tipo!");

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
                {
                    ViewBag.COMO_CONHECEU = new SelectList(_db
                        .COMO_CONHECEU
                        .Where(c => c.SITUACAO == "A"), "ID", "DESCRICAO", pessoa.COMO_CONHECEU);

                    return View(pessoa);
                }

                switch (pessoa.NUM_DOC.Length)
                {
                    case 18:
                        pessoa.TIPO_DOC = "J";
                        break;
                    case 14:
                        pessoa.TIPO_DOC = "F";
                        break;
                }

                if (pessoa.TIPO_PESSOA_TRIBUTACAO == null || pessoa.TIPO_PESSOA_TRIBUTACAO == 0)
                    pessoa.TIPO_PESSOA_TRIBUTACAO = Convert.ToInt32((pessoa.TIPO_DOC == "F" ? enmTipoPessoaTrib.ePF : enmTipoPessoaTrib.ePJ_Comum));

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

                /*
                if (pessoa.IS_CLIENTE && pessoa.QUANTIDADE_LOJAS > 0)
                {
                    for (Int32 it = 0; it < pessoa.QUANTIDADE_LOJAS; ++it)
                    {
                        LOJA loja = new LOJA()
                        {
                            CLIENTE = pessoa.ID,
                            DESCRICAO = it.ToString(),
                            BAIRRO = "",
                            CEP = "",
                            CIDADE = "",
                            EMAIL = "",
                            FATURAR_EM = "",
                            CONTATO = "",
                            COMPLEMENTO = "",
                            SITUACAO = "A",
                            TELEFONE2 = "",
                            TELEFONE1 = "",
                            TELEFONE3 = "",
                            ZONA = "",
                            UF = "",
                            ENDERECO = "",
                            NUM_DOC = "",
                            OBSERVACAO = "",
                            APELIDO="",
                        };

                        _db.LOJA.Add(loja);
                    }
                }
                */

                _db.SaveChanges();

                return RedirectToAction("Index", new { doc = pessoa.NUM_DOC });
        }


        //
        // GET: /Fornecedor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var pessoa = _db.PESSOA.Find(id);

                if (pessoa == null)
                {
                    return HttpNotFound();
                }



                return View(pessoa);
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /Fornecedor/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PESSOA pessoa)
        {
            if (Session.IsFuncionario())
            {
                var existe = _db
                .PESSOA
                .Any(x => x.NUM_DOC == pessoa.NUM_DOC && x.ID != pessoa.ID);

                if (existe)
                    ModelState.AddModelError(string.Empty, "Este documento já esta cadastrado para outro Fornecedor!");

                if (pessoa.CLIENTE == 0 && pessoa.FUNCIONARIO == 0 && pessoa.FORNECEDOR == 0 && pessoa.TERCEIRO == 0)
                    ModelState.AddModelError(string.Empty, "A pessoa deve ter pelo menos um tipo!");

                if (pessoa.NUM_DOC.Length < 14)
                    ModelState.AddModelError(string.Empty, "Informe um Número de Documento válido!");

                if ((string.IsNullOrEmpty(pessoa.TELEFONE1)) && (string.IsNullOrEmpty(pessoa.TELEFONE2)) && (string.IsNullOrEmpty(pessoa.TELEFONE3)))
                    ModelState.AddModelError(string.Empty, "Informe um pelo menos um número de telefone!");

                if ((!string.IsNullOrEmpty(pessoa.TELEFONE1)) && (pessoa.TELEFONE1.Length != 14))
                    ModelState.AddModelError(string.Empty, "Telefone Inválido!");

                if ((!string.IsNullOrEmpty(pessoa.TELEFONE2)) && (pessoa.TELEFONE2.Length != 15))
                    ModelState.AddModelError(string.Empty, "Celular Inválido!");

                if ((!string.IsNullOrEmpty(pessoa.TELEFONE3)) && (pessoa.TELEFONE3.Length != 14))
                    ModelState.AddModelError(string.Empty, "Tel. Contato Inválido!");

                if (!ModelState.IsValid)
                {
                    return View(pessoa);
                }

                switch (pessoa.NUM_DOC.Length)
                {
                    case 18:
                        pessoa.TIPO_DOC = "J";
                        break;
                    case 14:
                        pessoa.TIPO_DOC = "F";
                        break;
                }

                if (pessoa.TIPO_PESSOA_TRIBUTACAO == null || pessoa.TIPO_PESSOA_TRIBUTACAO == 0)
                    pessoa.TIPO_PESSOA_TRIBUTACAO = Convert.ToInt32((pessoa.TIPO_DOC == "F" ? enmTipoPessoaTrib.ePF : enmTipoPessoaTrib.ePJ_Comum));

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


                _db.Configuration.ValidateOnSaveEnabled = false;

                _db.PESSOA.Attach(pessoa);

                var entry = _db.Entry(pessoa);

                entry.Property(p => p.NOME).IsModified = true;
                entry.Property(p => p.NUM_DOC).IsModified = true;
                entry.Property(p => p.TIPO_DOC).IsModified = true;
                entry.Property(p => p.TELEFONE1).IsModified = true;
                entry.Property(p => p.TELEFONE2).IsModified = true;
                entry.Property(p => p.TELEFONE3).IsModified = true;
                entry.Property(p => p.NOME).IsModified = true;
                entry.Property(p => p.RAZAO).IsModified = true;
                entry.Property(p => p.UF).IsModified = true;
                entry.Property(p => p.ZONA).IsModified = true;
                entry.Property(p => p.SITUACAO).IsModified = true;
                entry.Property(p => p.RESPONSAVEL).IsModified = true;

                if (Session.IsAdministrador())
                {
                    entry.Property(p => p.ADMINISTRADOR).IsModified = true;
                }

                entry.Property(p => p.RESPONSAVEL).IsModified = true;
                entry.Property(p => p.TERCEIRO).IsModified = true;
                entry.Property(p => p.FUNCIONARIO).IsModified = true;
                entry.Property(p => p.FORNECEDOR).IsModified = true;
                entry.Property(p => p.CLIENTE).IsModified = true;
                entry.Property(p => p.COMPLEMENTO).IsModified = true;
                entry.Property(p => p.CEP).IsModified = true;
                entry.Property(p => p.ENDERECO).IsModified = true;
                entry.Property(p => p.BAIRRO).IsModified = true;
                entry.Property(p => p.CIDADE).IsModified = true;
                entry.Property(p => p.CONTATO).IsModified = true;
                entry.Property(p => p.EMAIL).IsModified = true;
                entry.Property(p => p.OBSERVACAO).IsModified = true;
                entry.Property(p => p.TIPO_PESSOA_TRIBUTACAO).IsModified = true;

                try
                {
                    _db.SaveChanges();
                }
                catch (FileLoadException ex)
                {
                    DbUpdateException dbu = ex.InnerException.InnerException as DbUpdateException;

                    var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

                    try
                    {
                        foreach (var result in dbu.Entries)
                        {
                            builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name);
                        }
                    }
                    catch (Exception e)
                    {
                        builder.Append("Error parsing DbUpdateException: " + e.ToString());
                    }

                    string message = builder.ToString();

                    throw new Exception(message, dbu);
                }

                return RedirectToAction("Index", new { doc = pessoa.NUM_DOC });
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult NovoCliente(string endereco, string email, string nome)
        {
            if (Session.IsFuncionario())
            {
                PESSOA cliente = new PESSOA()
                {
                    CLIENTE = 1,
                    NOME = nome.ToUpper(),
                    RAZAO = nome.ToUpper(),
                    ENDERECO = endereco.ToUpper(),
                    EMAIL = email,
                    BAIRRO = "",
                    TELEFONE1 = "",
                    TELEFONE2 = "",
                    TELEFONE3 = "",
                    NUM_DOC = "",
                    CIDADE = "",
                    INSC_ESTADUAL = "",
                    COMO_CONHECEU = 0,
                    CEP = "",
                    COMPLEMENTO = "",
                    OBSERVACAO = "",
                    CONTATO = "",
                    SITUACAO = "A",
                    TIPO_DOC = "J",
                    UF = "",

                };

                _db.PESSOA.Add(cliente);

                _db.SaveChanges();

                return Json(new { id = cliente.ID, nome = cliente.RAZAO }, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
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
                         select z.ZONA)
                         .Distinct()
                         .ToList();

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

        [HttpGet]
        public JsonResult PesquisaClientes(string search)
        {
            try
            {
                IEnumerable<PESSOA> l_List = _db.PESSOA.Where(p => p.NOME.IndexOf(search) > -1 || p.RAZAO.IndexOf(search) > -1);

                List<PESSOA> l_SearchResult = l_List.ToList();

                var l_JsonReturn = l_SearchResult.Select(p => new
                {
                    Id = p.ID,
                    Nome = p.NOME,
                    Razao = p.RAZAO,
                    Cidade = p.CIDADE,
                    Uf = p.UF
                });

                return Json(l_JsonReturn, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
    }
}