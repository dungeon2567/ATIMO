using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using ATIMO;
using System.Web;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.draw;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace Atimo.Controllers
{
    public class VisitaInicialController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /VisitaInicial/

        public async Task<ActionResult> Search()
        {
            ViewBag.PROJETO = new SelectList(await _db.PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO");

            return View();
        }

        public async Task<ActionResult> Index(string[] situacao = null, string[] tipo = null, Int32? num = null, Int32? cliente = null, Int32? loja = null, string de = null, string ate = null, Int32? projeto = null, string[] ocorrencia = null, int page = 1)
        {
            var queryObject = _db.OSSB
            .Include(o => o.OSSB_CHECK_LIST)
            .Include(o => o.CONTRATO1)
            .Include(o => o.PESSOA)
            .Include(o => o.PROJETO1)
            .Include(o => o.CONTRATO1)
            .Include(o => o.LOJA1);

            ViewBag.QueryParams = new Dictionary<String, Object>();

            if (projeto != null)
            {
                queryObject = queryObject.Where(o => o.PROJETO == projeto);

                ViewBag.QueryParams["projeto"] = projeto;
            }

            if (situacao != null && situacao.Length > 0)
            {
                queryObject = queryObject.Where(o => situacao.Contains(o.SITUACAO));

                ViewBag.QueryParams["situacao"] = situacao;
            }

            if (tipo != null && tipo.Length > 0)
            {
                queryObject = queryObject.Where(o => tipo.Contains(o.TIPO));

                ViewBag.QueryParams["tipo"] = tipo;
            }

            if (num != null)
            {
                queryObject = queryObject.Where(o => o.ID == num.Value);

                ViewBag.QueryParams["num"] = num;
            }

            if (cliente != null)
            {
                queryObject = queryObject.Where(o => o.CLIENTE == cliente.Value);

                ViewBag.QueryParams["cliente"] = cliente;
            }

            if (loja != null)
            {
                queryObject = queryObject.Where(o => o.LOJA == loja.Value);

                ViewBag.QueryParams["loja"] = loja;
            }

            if (de != null)
            {
                DateTime deDate = DateTime.ParseExact(de, "dd/MM/yyyy", null);

                ViewBag.QueryParams["de"] = de;

                if (ate != null)
                {

                    ViewBag.QueryParams["ate"] = ate;

                    DateTime ateDate = DateTime.ParseExact(ate, "dd/MM/yyyy", null);

                    queryObject = from os in queryObject
                                  where (os.SITUACAO == "I" && (os.DATA_VISITA == null || (os.DATA_VISITA >= deDate && os.DATA_VISITA <= ateDate))) || (os.SITUACAO == "E" && _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO >= deDate && oscl.AGENDADO <= ateDate).Any())  || (os.SITUACAO != "I" && os.SITUACAO != "E")
                                  select os;
                }
                else
                {

                    queryObject = from os in queryObject
                                  where (os.SITUACAO == "I" && os.DATA_VISITA == null || os.DATA_VISITA >= deDate) || (os.SITUACAO == "E" && _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO >= deDate).Any()) || (os.SITUACAO != "I" && os.SITUACAO != "E")
                                  select os;
                }
            }
            else
                if (ate != null)
            {
                ViewBag.QueryParams["ate"] = ate;

                DateTime ateDate = DateTime.ParseExact(ate, "dd/MM/yyyy", null);

                queryObject = from os in queryObject
                              where (os.SITUACAO == "I" && os.DATA_VISITA == null || os.DATA_VISITA <= ateDate) || (os.SITUACAO == "E" && _db.OSSB_CHECK_LIST.Where(oscl => oscl.OSSB == os.ID && oscl.AGENDADO <= ateDate).Any()) || (os.SITUACAO != "I" && os.SITUACAO != "E")
                              select os;
            }

            if (ocorrencia != null && ocorrencia.Length > 0)
            {
                queryObject = queryObject.Where(os => ocorrencia.All(oc => os.OCORRENCIA.Contains(oc)));
            }

            queryObject = queryObject.OrderBy(os => os.ID);

            ViewBag.PAGE = page;
            ViewBag.PAGE_COUNT = ((await queryObject.CountAsync() - 1) / 25) + 1;

            queryObject = queryObject
                .Skip((page - 1) * 25)
                .Take(25);

            return View(await queryObject.AsNoTracking().ToArrayAsync());
        }

        //
        // GET: /VisitaInicial/Create

        public async Task<ActionResult> Create()
        {
            ViewBag.CLIENTE = new SelectList(await _db.PESSOA.Where(p => p.CLIENTE == 1)
            .Where(p => p.SITUACAO == "A").OrderBy(p => p.RAZAO).ThenBy(p => p.NOME).ToArrayAsync(), "ID", "NOME_COMPLETO");

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.TECNICO = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1 || p.TERCEIRO == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO");

            ViewBag.RESPONSAVEL = new SelectList(await _db.PESSOA.Where(p => p.RESPONSAVEL == 1)
               .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO");

            ViewBag.LOJA = new SelectList(Enumerable.Empty<LOJA>(), "ID", "APELIDO");


            ViewBag.CONTRATO = new SelectList(Enumerable.Empty<CONTRATO>(), "ID", "DESCRICAO");

            return View(new OSSB() { DATA_CADASTRO = DateTime.Now, SITUACAO = "I" });

        }

        //
        // POST: /VisitaInicial/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OSSB ossb)
        {
            if (ModelState.IsValid)
            {

                ossb.OCORRENCIA = ossb.OCORRENCIA?.ToUpper() ?? "";


                ossb.PRAZO_EXECUCAO = String.IsNullOrWhiteSpace(ossb.PRAZO_EXECUCAO) ? null : ossb.PRAZO_EXECUCAO;
                ossb.PRAZO_PAGAMENTO = String.IsNullOrWhiteSpace(ossb.PRAZO_PAGAMENTO) ? null : ossb.PRAZO_PAGAMENTO;

                var returnCode = new SqlParameter("@ReturnCode", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                var id = await _db.Database.ExecuteSqlCommandAsync("EXEC @ReturnCode = [saobento_sistema].CREATE_ORDEM_SERVICO @RESPONSAVEL, @SUPERVISOR, @CONTRATO, @PROJETO, @CLIENTE, @LOJA, @OCORRENCIA, @PORTE, @AMBIENTE, @TIPO, @SITUACAO, @OBSERVACAO, @ORCAMENTISTA, @PRAZO_PAGAMENTO, @PRAZO_EXECUCAO",
                   returnCode,
                    new SqlParameter("@RESPONSAVEL", (object)ossb.RESPONSAVEL ?? (object)DBNull.Value),
                    new SqlParameter("@SUPERVISOR", (object)ossb.SUPERVISOR ?? (object)DBNull.Value),
                    new SqlParameter("@CONTRATO", (object)ossb.CONTRATO ?? (object)DBNull.Value),
                    new SqlParameter("@PROJETO", (object)ossb.PROJETO ?? (object)DBNull.Value),
                    new SqlParameter("@CLIENTE", (object)ossb.CLIENTE ?? (object)DBNull.Value),
                    new SqlParameter("@LOJA", (object)ossb.LOJA ?? (object)DBNull.Value),
                    new SqlParameter("@OCORRENCIA", (object)ossb.OCORRENCIA ?? (object)DBNull.Value),
                    new SqlParameter("@PORTE", (object)ossb.PORTE ?? (object)DBNull.Value),
                    new SqlParameter("@AMBIENTE", (object)ossb.AMBIENTE ?? (object)DBNull.Value),
                    new SqlParameter("@TIPO", (object)ossb.TIPO ?? (object)DBNull.Value),
                    new SqlParameter("@SITUACAO", (object)ossb.SITUACAO ?? (object)DBNull.Value),
                    new SqlParameter("@OBSERVACAO", (object)ossb.OBSERVACAO ?? (object)DBNull.Value),
                    new SqlParameter("@ORCAMENTISTA", (object)ossb.ORCAMENTISTA ?? (object)DBNull.Value),
                    new SqlParameter("@PRAZO_PAGAMENTO", (object)ossb.PRAZO_PAGAMENTO ?? (object)DBNull.Value),
                    new SqlParameter("@PRAZO_EXECUCAO", (object)ossb.PRAZO_EXECUCAO ?? (object)DBNull.Value)
                    );

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { num = returnCode.Value});
            }


            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", ossb.PROJETO);

            ViewBag.CLIENTE = new SelectList(await _db.PESSOA.Where(p => p.CLIENTE == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.CLIENTE);

            ViewBag.RESPONSAVEL = new SelectList(await _db.PESSOA.Where(p => p.RESPONSAVEL == 1)
               .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.RESPONSAVEL);

            ViewBag.CONTRATO = new SelectList(await _db.CONTRATO.Where(c => c.SITUACAO == "A")
             .Where(c => c.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "DESCRICAO", ossb.CONTRATO);

            ViewBag.LOJA = new SelectList(await _db.LOJA.Where(l => l.SITUACAO == "A")
                .Where(l => l.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "APELIDO", ossb.LOJA);


            return View(ossb);
        }

        //
        // GET: /VisitaInicial/Edit/5

        public async Task<ActionResult> Edit(int id = 0)
        {

            var ossb = await _db.OSSB.FindAsync(id);

            if (ossb == null)
            {
                return HttpNotFound();
            }


            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", ossb.PROJETO);

            ViewBag.CLIENTE = new SelectList(await _db.PESSOA.Where(p => p.CLIENTE == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.CLIENTE);

            ViewBag.RESPONSAVEL = new SelectList(await _db.PESSOA.Where(p => p.RESPONSAVEL == 1)
               .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.RESPONSAVEL);

            ViewBag.ORCAMENTISTA = new SelectList(await _db.PESSOA.Where(p => p.FUNCIONARIO == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.ORCAMENTISTA);

            if (ossb.CONTRATO != null)
            {
                ViewBag.CONTRATO = new SelectList(await _db.CONTRATO.Where(c => c.SITUACAO == "A")
                 .Where(c => c.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "DESCRICAO", ossb.CONTRATO);
            }

            ViewBag.LOJA = new SelectList(await _db.LOJA.Where(l => l.SITUACAO == "A")
                .Where(l => l.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "APELIDO", ossb.LOJA);


            return View(ossb);
        }

        //
        // POST: /VisitaInicial/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OSSB ossb)
        {
            if (ModelState.IsValid)
            {


                _db.Configuration.ValidateOnSaveEnabled = false;

                ossb.OCORRENCIA = ossb.OCORRENCIA?.ToUpper() ?? "";

                ossb.PRAZO_EXECUCAO = String.IsNullOrWhiteSpace(ossb.PRAZO_EXECUCAO) ? null : ossb.PRAZO_EXECUCAO;
                ossb.PRAZO_PAGAMENTO = String.IsNullOrWhiteSpace(ossb.PRAZO_PAGAMENTO) ? null : ossb.PRAZO_PAGAMENTO;

                var entry = _db.Entry(ossb);

                _db.OSSB.Attach(ossb);

                entry.Property(pp => pp.PROJETO).IsModified = true;
                entry.Property(pp => pp.CLIENTE).IsModified = true;
                entry.Property(pp => pp.CONTRATO).IsModified = true;
                entry.Property(pp => pp.LOJA).IsModified = true;
                entry.Property(pp => pp.RESPONSAVEL).IsModified = true;
                entry.Property(pp => pp.OCORRENCIA).IsModified = true;

                entry.Property(pp => pp.PORTE).IsModified = true;
                entry.Property(pp => pp.AMBIENTE).IsModified = true;
                entry.Property(pp => pp.TIPO).IsModified = true;
                entry.Property(pp => pp.OBSERVACAO).IsModified = true;
                entry.Property(pp => pp.PRAZO_EXECUCAO).IsModified = true;
                entry.Property(pp => pp.PRAZO_PAGAMENTO).IsModified = true;


                _db.SaveChanges();

                return RedirectToAction("Index", new
                {
                    num = ossb.ID
                });
            }


            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", ossb.PROJETO);

            ViewBag.CLIENTE = new SelectList(await _db.PESSOA.Where(p => p.CLIENTE == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.CLIENTE);

            ViewBag.RESPONSAVEL = new SelectList(await _db.PESSOA.Where(p => p.RESPONSAVEL == 1)
               .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.RESPONSAVEL);

            ViewBag.ORCAMENTISTA = new SelectList(await _db.PESSOA.Where(p => p.FUNCIONARIO == 1)
                .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME_COMPLETO", ossb.ORCAMENTISTA);

            ViewBag.CONTRATO = new SelectList(await _db.CONTRATO.Where(c => c.SITUACAO == "A")
             .Where(c => c.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "DESCRICAO", ossb.CONTRATO);

            ViewBag.LOJA = new SelectList(await _db.LOJA.Where(l => l.SITUACAO == "A")
                .Where(l => l.CLIENTE == ossb.CLIENTE).ToArrayAsync(), "ID", "APELIDO", ossb.LOJA);


            return View(ossb);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public async Task<ActionResult> EnviarArquivoChecklist(Int32 id, HttpPostedFileBase file)
        {

            var item = await _db
                .OSSB_CHECK_LIST
                .Include(cl => cl.ANEXO1)
                .FirstOrDefaultAsync(checkitem => checkitem.ID == id);

            if (file != null && item != null && file.ContentType == "application/pdf")
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

            return Redirect(Request.UrlReferrer.ToString() + "#ossb_" + item.OSSB);
        }


        [HttpPost]
        public async Task<FileResult> BaixarArquivoChecklist(Int32 ossb, Int32 id)
        {
            var user = Session.Usuario();

            if (user != null && user.FUNCIONARIO == 1)
            {
                var item = await _db
                    .OSSB_CHECK_LIST
                    .Include(cl => cl.ANEXO1)
                    .FirstOrDefaultAsync(checkitem => checkitem.OSSB == ossb && checkitem.ID == id);

                if (item != null && item.ANEXO1 != null)
                {
                    return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME), item.ANEXO1.NOME);
                }

                return null;
            }
            else
                return null;
        }

        public async Task<JsonResult> GetClientes(String query)
        {
            if (Session.IsFuncionario())
            {
                var clientes = await ((from p in _db.PESSOA
                                       where p.SITUACAO == "A" && p.CLIENTE == 1 && (p.RAZAO.StartsWith(query) || p.NOME.StartsWith(query))
                                       select p).ToArrayAsync());

                return Json(new { status = 0, clientes = clientes.Select(p => new { id = p.ID, text = p.NOME_COMPLETO }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetPipeline(int ossb)
        {
            var pipeline = await _db.OSSB.Where(os => os.ID == ossb)
                            .Select(os => new { data_proposta = os.QUANDO_PRECISA_ORCAMENTO, data_followup = os.DATA_FOLLOW_UP })
                            .FirstOrDefaultAsync();

            return Json(new
            {
                data_proposta = pipeline.data_proposta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                data_followup = pipeline.data_followup?.ToString("dd/MM/yyyy HH:mm") ?? ""
            }, JsonRequestBehavior.AllowGet);

        }



        public async Task<JsonResult> GetLojas(String query, Int32? cliente = null)
        {
            if (Session.IsFuncionario())
            {
                LOJA[] lojas;

                if (cliente == null)
                {

                    lojas = await ((from l in _db.LOJA
                                    where l.SITUACAO == "A" && l.APELIDO.Contains(query)
                                    select l)
                                        .ToArrayAsync());
                }
                else
                {
                    lojas = await ((from l in _db.LOJA
                                    where l.SITUACAO == "A" && l.APELIDO.Contains(query) && l.CLIENTE == cliente
                                    select l)
                                       .ToArrayAsync());
                }

                return Json(new { status = 0, lojas = lojas.Select(p => new { id = p.ID, text = p.APELIDO }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<JsonResult> GetComunicacao(int ossb)
        {

            var usuario = Session.UsuarioId();

            var comunicacao = await ((from c in _db.OSSB_COMUNICACAO
                                      .Include(c => c.PESSOA1)
                                      where c.OSSB == ossb
                                      select c)
                                      .OrderByDescending(c => c.ID)
                                     .ToArrayAsync());

            return Json(comunicacao.Select(c => new { id = c.ID, editavel = c.PESSOA == usuario, editing = false, pessoa = c.PESSOA1.RAZAO, texto = c.TEXTO, data = c.DATA.ToString("dd/MM/yyyy") }), JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> AdicionarComunicacao(int ossb, string texto)
        {

            var usuario = Session.UsuarioId();

            var c = new OSSB_COMUNICACAO()
            {
                DATA = DateTime.Today,
                OSSB = ossb,
                TEXTO = texto.ToUpper(),
                TIPO = "I",
                PESSOA = usuario
            };

            _db.OSSB_COMUNICACAO.Add(c);

            await _db.SaveChangesAsync();

            var p = Session.Usuario();

            return Json(new { editavel = true, pessoa = p.RAZAO, texto = c.TEXTO, data = c.DATA.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GetPessoas(String query)
        {

            var pessoas = await (from p in _db.PESSOA
                                 where p.RAZAO.StartsWith(query) || p.NOME.StartsWith(query)
                                 select p)
                                  .ToArrayAsync();

            return Json(new { status = 0, pessoas = pessoas.Select(p => new { id = p.ID, text = p.NOME_COMPLETO }) }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> GetTecnicosAutoComplete(Int32 ossb, String query)
        {
            if (Session.IsFuncionario())
            {
                var tecnicos = await (from p in _db.PESSOA
                                      where !(_db.OSSB_TECNICO.Where(ot => ot.OSSB == ossb).Select(ot => ot.TECNICO).Contains(p.ID)) && (p.RAZAO.StartsWith(query) || p.NOME.StartsWith(query))
                                      select p)
                                      .ToArrayAsync();

                return Json(new { status = 0, tecnicos = tecnicos.Select(p => new { id = p.ID, razao = p.NOME_COMPLETO }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetTecnicos(Int32 ossb)
        {
            if (Session.IsFuncionario())
            {
                var tecnicos = await (from p in _db.PESSOA
                                      join ot in _db.OSSB_TECNICO on p.ID equals ot.TECNICO
                                      where ot.OSSB == ossb
                                      select p)
                               .ToArrayAsync();

                return Json(new { status = 0, tecnicos = tecnicos.Select(p => new { id = p.ID, razao = p.NOME_COMPLETO }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> AdicionarTecnico(Int32 ossb, Int32 tecnico)
        {
            if (Session.IsFuncionario())
            {
                _db.OSSB_TECNICO.Add(new OSSB_TECNICO() { OSSB = ossb, TECNICO = tecnico });

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> RemoverTecnico(Int32 ossb, Int32 tecnico)
        {
            if (Session.IsFuncionario())
            {
                var ossbTecnico = await _db
                  .OSSB_TECNICO
                  .FirstOrDefaultAsync(tc => tc.OSSB == ossb && tc.TECNICO == tecnico);

                if (ossbTecnico == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                _db.Entry(ossbTecnico)
                    .State = EntityState.Deleted;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<JsonResult> MarcarComoResolvido(Int32 ossb, Int32 id, String data)
        {
            var item = await _db.OSSB_CHECK_LIST.FirstOrDefaultAsync(checkitem => checkitem.OSSB == ossb && checkitem.ID == id);

            if (item != null)
            {
                item.VISITADO = DateTime.Parse(data);

                _db.Entry(item).
                    State = EntityState.Modified;

                await _db.SaveChangesAsync();

            }

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> AlterarStatus(int id, string status, string historico)
        {

            if (Session.IsFuncionario())
            {
                var ossb = await _db.OSSB
                .Include(os => os.OSSB_SERVICO)
                .FirstOrDefaultAsync(os => os.ID == id);

                if (ossb != null)
                {
                    if (ossb.SITUACAO == "I" && ossb.CLIENTE == null)
                    {
                        return Json(new { status = 1, mensagem = "Cliente necessario para mudar de status." }, JsonRequestBehavior.AllowGet);
                    }

                    if (!string.IsNullOrEmpty(historico))
                    {

                        OSSB_COMUNICACAO ci = new OSSB_COMUNICACAO()
                        {
                            DATA = DateTime.Today,
                            OSSB = id,
                            TEXTO = historico,
                            TIPO = "I",
                            PESSOA = Session.UsuarioId()
                        };

                        _db.OSSB_COMUNICACAO.Add(ci);

                    }

                    _db.OSSB_SITUACAO_HISTORICO.Add(new OSSB_SITUACAO_HISTORICO()
                    {
                        DATA_ALTERACAO = DateTime.Now,
                        DE = ossb.SITUACAO,
                        PARA = status,
                        PESSOA = Session.UsuarioId(),
                        OSSB = ossb.ID
                    });

                    ossb.SITUACAO = status;

                    _db.Entry(ossb).State =
                        EntityState.Modified;


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
                }

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = 2, mensagem = "Cliente necessario para mudar de status." }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ListaContratosCliente(int id)
        {
            if (Session.IsFuncionario())
            {
                var contrato = _db
                    .CONTRATO
                    .Where(c => c.SITUACAO == "A" && c.CLIENTE == id)
                    .ToArrayAsync();

                return Json(new SelectList(await contrato, "ID", "DESCRICAO"), JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public async Task<JsonResult> AgendarVisita(int ossb, String data, Int32[] equipe)
        {
            var os = await _db
                .OSSB
                .FirstOrDefaultAsync(item => item.ID == ossb);

            if (os == null)
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }

            os.DATA_VISITA = DateTime.Parse(data);

            foreach (var membro in equipe)
            {
                _db.OSSB_TECNICO.Add(new OSSB_TECNICO() { OSSB = ossb, TECNICO = membro });
            }

            await _db.SaveChangesAsync();

            return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> EditarComunicacao(int id, string texto)
        {
            var comunicacao = await _db
               .OSSB_COMUNICACAO
               .FirstOrDefaultAsync(c => c.ID == id);

            if (comunicacao == null)
            {
                return HttpNotFound();
            }

            if (comunicacao.PESSOA == Session.UsuarioId())
            {
                comunicacao.TEXTO = texto;

                await _db.SaveChangesAsync();
            }

            return Json(new object(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditarDataProposta(int ossb, string data)
        {
            var os = await _db
               .OSSB
               .FirstOrDefaultAsync(oss => oss.ID == ossb);

            if (os == null)
            {
                return HttpNotFound();
            }



            os.QUANDO_PRECISA_ORCAMENTO = String.IsNullOrEmpty(data) ? null : (DateTime?)DateTime.Parse(data);

            _db.Entry(os)
                .Property(x => x.QUANDO_PRECISA_ORCAMENTO)
                .IsModified = true;

            await _db.SaveChangesAsync();

            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditarDataFollowup(int ossb, string data)
        {
            var os = await _db
               .OSSB
               .FirstOrDefaultAsync(oss => oss.ID == ossb);

            if (os == null)
            {
                return HttpNotFound();
            }

            os.DATA_FOLLOW_UP = String.IsNullOrEmpty(data) ? null : (DateTime?)DateTime.Parse(data);

            _db.Entry(os)
                .Property(x => x.DATA_FOLLOW_UP)
                .IsModified = true;

            await _db.SaveChangesAsync();

            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> EditarDataAgendado(int id, string data)
        {
            var checklist = await _db
                .OSSB_CHECK_LIST
                .FirstOrDefaultAsync(item => item.ID == id);

            if (checklist == null)
            {
                return HttpNotFound();
            }

            checklist.AGENDADO = DateTime.Parse(data);

            _db.Entry(checklist)
                .State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EditarDataExecutado(int id, string data)
        {
            var checklist = await _db
                .OSSB_CHECK_LIST
                .FirstOrDefaultAsync(item => item.ID == id);

            if (checklist == null)
            {
                return HttpNotFound();
            }

            checklist.VISITADO = string.IsNullOrWhiteSpace(data) ? null : (DateTime?)DateTime.Parse(data);

            _db.Entry(checklist)
                .State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> AgendarExecucao(int ossb, String data)
        {
            var os = await _db
                .OSSB
               .Include(item => item.OSSB_CHECK_LIST)
                .FirstOrDefaultAsync(item => item.ID == ossb);

            if (os == null)
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
            }

            if (!os.OSSB_CHECK_LIST.Any())
            {
                _db.OSSB_CHECK_LIST.Add(new OSSB_CHECK_LIST() { OSSB = ossb, AGENDADO = DateTime.Parse(data) });

                await _db.SaveChangesAsync();
            }

            return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ListaLojasCliente(int id)
        {
            var lojas = _db
                .LOJA
                .Where(l => l.CLIENTE == id && l.SITUACAO == "A")
                .ToArrayAsync();

            return Json(new SelectList(await lojas, "ID", "APELIDO"), JsonRequestBehavior.AllowGet);
        }
    }
}