using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Text;
using System;

namespace Atimo.Controllers
{
    public class OssbRecorrenciaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Create(int id = 0)
        {
            var loja = await _db.LOJA.FirstOrDefaultAsync(l => l.ID == id);

            if (loja == null)
                return HttpNotFound();

            ViewBag.PROJETO = new SelectList(await _db
                .PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.CONTRATO = new SelectList(await _db
                .CONTRATO.Where(l => l.SITUACAO == "A" && l.CLIENTE == loja.CLIENTE)
                .ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.UNIDADE = new SelectList(await _db.
                UNIDADE.ToArrayAsync(), "ID", "SIGLA");

            return View(new RecorrenciaViewModel() { LOJA = id, CLIENTE = loja.CLIENTE });

        }

        //
        // POST: /ContratoLoja/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RecorrenciaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    OSSB ossb = null;

                    var contrato = await _db.CONTRATO.FirstOrDefaultAsync(ct => ct.ID == viewModel.CONTRATO);



                    foreach (var item in viewModel.RECORRENCIA.OrderBy(r => r))
                    {
                        if (ossb == null || new DateTime(item.Year, item.Month, 1) != new DateTime(ossb.DATA_CADASTRO.Year, ossb.DATA_CADASTRO.Month, 1))
                        {
                            ossb = new OSSB()
                            {
                                SITUACAO = "E",
                                CLIENTE = viewModel.CLIENTE,
                                CONTRATO = viewModel.CONTRATO,
                                LOJA = viewModel.LOJA,
                                OBSERVACAO = "",
                                OCORRENCIA = "",
                                PORTE = "G",
                                AMBIENTE = "I",
                                TIPO = "P",
                                PROJETO = viewModel.PROJETO,
                                DATA_CADASTRO = item.Date,
                            };

                            ossb.OSSB_SERVICO.Add(new OSSB_SERVICO()
                            {
                                DESCRICAO = contrato.DESCRICAO,
                                SUBDIVISAO = "",
                                OSSB = ossb.ID,
                                QUANTIDADE = 1,
                                UNIDADE = viewModel.UNIDADE,
                                VALOR_MO = Convert.ToDecimal(viewModel.VALOR),
                                VALOR_MA = 0,
                            });

                            _db.OSSB.Add(ossb);

                            await _db.SaveChangesAsync();
                        }

                        ossb.OSSB_CHECK_LIST.Add(new OSSB_CHECK_LIST() {  OSSB = ossb.ID, AGENDADO = item.Add(TimeSpan.Parse(viewModel.DE)) });

                        await _db.SaveChangesAsync();
                    }

                    return RedirectToAction("Index", "Loja", new { id = viewModel.CLIENTE });
                }


                /*
                foreach (var item in viewModel.RECORRENCIA)
                {
                    var ossb = new OSSB()
                    {
                        SITUACAO = "I",
                        CLIENTE = viewModel.CLIENTE,
                        CONTRATO = viewModel.CONTRATO,
                        LOJA = viewModel.LOJA,
                        OBSERVACAO = "",
                        OCORRENCIA = "",
                        PORTE = "G",
                        AMBIENTE = "I",
                        TIPO = "C",
                        PROJETO = viewModel.PROJETO,
                        DATA_CADASTRO = DateTime.Parse(item.DATA),
                    };

                    _db.OSSB.Add(ossb);

                    await _db.SaveChangesAsync();

                    var servico = new OSSB_SERVICO()
                    {
                        OSSB = ossb.ID,
                        SERVICO = int.Parse(item.SERVICO),
                        VALOR_MO = decimal.Parse(viewModel.VALOR),
                        QUANTIDADE = decimal.Parse(viewModel.QUANTIDADE),
                        AREA_MANUTENCAO = int.Parse(item.AREA_MANUTENCAO),
                        UNIDADE = viewModel.UNIDADE,
                    };

                    _db.OSSB_SERVICO.Add(servico);

                    await _db.SaveChangesAsync();
                }
            }
            */
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

            ViewBag.PROJETO = new SelectList(await _db
                .PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO", viewModel.PROJETO);

            ViewBag.CONTRATO = new SelectList(await _db
                .CONTRATO.Where(l => l.SITUACAO == "A" && l.CLIENTE == viewModel.CLIENTE)
                .ToArrayAsync(), "ID", "DESCRICAO", viewModel.CONTRATO);

            ViewBag.UNIDADE = new SelectList(await _db.
                UNIDADE.ToArrayAsync(), "ID", "SIGLA", viewModel.UNIDADE);

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();

            base.Dispose(disposing);
        }

        public bool VerificaContratoLoja(int CONTRATO, int LOJA)
        {
            return _db.CONTRATO_LOJA.Any(cl => cl.CONTRATO == CONTRATO && cl.LOJA == LOJA);
        }


        [HttpGet]
        public async Task<JsonResult> GetLoja(int cliente, string cnpj)
        {
            var proxy = _db.Configuration.ProxyCreationEnabled;
            var lazy = _db.Configuration.LazyLoadingEnabled;

            _db.Configuration.ProxyCreationEnabled = false;
            _db.Configuration.LazyLoadingEnabled = false;

            var loja = await _db.LOJA
                .Where(l => l.SITUACAO == "A" && l.CLIENTE == cliente && l.NUM_DOC == cnpj)
                .FirstOrDefaultAsync();

            _db.Configuration.ProxyCreationEnabled = proxy;
            _db.Configuration.LazyLoadingEnabled = lazy;

            if (loja != null)
            {
                return Json(loja, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ID = -1 }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}