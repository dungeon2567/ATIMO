using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;
using System.Collections.Generic;

namespace Atimo.Controllers
{
    public class AreaDoFuncionarioController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public async Task<ActionResult> Executando()
        {
            DateTime dt_start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            DateTime dt_end = dt_start.AddMonths(1);


            var ossb = from os in _db.OSSB.Include(os => os.PESSOA).Include(os => os.LOJA1)
                       where os.SITUACAO == "E" && os.OSSB_CHECK_LIST.Any(ocl => ocl.AGENDADO >= dt_start && ocl.AGENDADO < dt_end)
                       select os;

            return View(await ossb.ToArrayAsync());
        }

        public async Task<ActionResult> Resultado(string de = null, string ate = null)
        {
            DateTime deDate = de == null ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : DateTime.Parse(de);
            DateTime ateDate = ate == null ? DateTime.Today : DateTime.Parse(ate);

            if (deDate < new DateTime(2018, 5, 1))
            {
                deDate = new DateTime(2018, 5, 1);
            }

            ViewBag.DE = deDate.ToString("dd/MM/yyyy");
            ViewBag.ATE = ateDate.ToString("dd/MM/yyyy");


            IQueryable<CONTAS_RECEBER> queryReceita = _db.CONTAS_RECEBER
                .Where(cr => cr.DATA_RECEBIMENTO != null);

            IQueryable<PAGAMENTO> queryPagamento = _db.PAGAMENTO.Where(p => p.DATA_PAGAMENTO != null);


            queryReceita = queryReceita.Where(cr => cr.DATA_RECEBIMENTO >= deDate && cr.DATA_RECEBIMENTO <= ateDate);

            queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO >= deDate && p.DATA_PAGAMENTO <= ateDate);



            switch (Session.UsuarioId())
            {
                case 72: // VANESSA
                    {
                        var receita_m = await queryReceita
                            .Where(r => r.OSSB1.PROJETO == 1)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa_m = await queryPagamento
                            .Where(p => p.PROJETO == 1)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa_csc = await queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        return View(new KeyValuePair<string, decimal>[] { new KeyValuePair<string, decimal>("MANUTENÇÃO", (receita_m - despesa_m - (despesa_csc / 2)) * 0.05m) });
                    }
                case 523:
                    {
                        var receitaObras = await queryReceita
                            .Where(r => r.OSSB1.PROJETO == 2)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var receitaManutencao = await queryReceita
                            .Where(r => r.OSSB1.PROJETO == 1)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .SumAsync();



                        var despesaManutencao = await queryPagamento
                            .Where(p => p.PROJETO == 1)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesaObras = await queryPagamento
                            .Where(p => p.PROJETO == 2)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa_csc = await queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        return View(new KeyValuePair<string, decimal>[] { new KeyValuePair<string, decimal>("(OBRAS)", (receitaObras - despesaObras - (despesa_csc / 2)) * 0.015m), new KeyValuePair<string, decimal>("(MANUTENÇÃO)", (receitaManutencao - despesaManutencao - (despesa_csc / 2)) * 0.015m) });

                    }
                case 83: // BESSA
                case 312: // DIEGO
                case 74: // THAMIRES
                    {

                        var receita = await queryReceita
                            .Where(r => r.OSSB1.PROJETO == 1 || r.OSSB1.PROJETO == 2)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa = await queryPagamento
                            .Where(p => p.PROJETO == 1 || p.PROJETO == 2)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa_csc = await queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        return View(new KeyValuePair<string, decimal>[] { new KeyValuePair<string, decimal>(null, (receita - despesa - despesa_csc) * 0.015m) });
                    }
                case 136: // JUNIOR
                    {

                        var receita = await queryReceita
                            .Where(r => r.OSSB1.PROJETO == 2)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa = await queryPagamento
                            .Where(p => p.PROJETO == 2)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        var despesa_csc = await queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .SumAsync();

                        return View(new KeyValuePair<string, decimal>[] { new KeyValuePair<string, decimal>(null, (receita - despesa - (despesa_csc / 2)) * 0.015m) });
                    }
                default:

                    return HttpNotFound();
            }


        }


        public async Task<ActionResult> Vencendo()
        {
            DateTime dt_end = DateTime.Today.AddDays(3);

            var ossb = from os in _db.OSSB
                       .Include(os => os.PESSOA)
                       .Include(os => os.LOJA1)
                       .Include(os => os.OSSB_CHECK_LIST)
                       where os.SITUACAO == "E" && os.OSSB_CHECK_LIST.Any() && os.OSSB_CHECK_LIST.Any(ocl => ocl.VISITADO == null && ocl.AGENDADO <= dt_end)
                       orderby os.OSSB_CHECK_LIST.Where(ocl => ocl.VISITADO == null).Min(ocl => ocl.AGENDADO)
                       select os;

            return View(await ossb.ToArrayAsync());
        }

        public async Task<ActionResult> Display(int id)
        {
            var ossb = await _db.OSSB
                .Include(os => os.CONTRATO1)
                         .Include(os => os.LOJA1)
                         .Include(os => os.PESSOA)
                         .Include(os => os.PROJETO1)
                         .Include(os => os.RESPONSAVEL1)
                         .Include(os => os.OSSB_COMUNICACAO)
                         .Include(os => os.OSSB_COMUNICACAO.Select(osc => osc.PESSOA1))
                .FirstOrDefaultAsync(os => os.ID == id);

            if (ossb == null)
                return HttpNotFound();

            return View(ossb);
        }

        public async Task<ActionResult> NaoValidadas()
        {
            Int32 funcionarioId = Session.UsuarioId();

            ViewBag.SALDO = await _db.CAIXINHA
                .Where(cx => cx.PESSOA == funcionarioId)
                .Select(cx => -cx.VALOR)
                .Concat(_db.CAIXINHA_ITEM.Where(cx => cx.PESSOA == funcionarioId && cx.VALIDACAO_FINANCEIRO && cx.VALIDACAO_OPERACIONAL)
                .Select(cxi => cxi.VALOR))
                .DefaultIfEmpty()
                .SumAsync();


            var query = from cxi in _db.CAIXINHA_ITEM
                        where cxi.PESSOA == funcionarioId && (!cxi.VALIDACAO_FINANCEIRO || !cxi.VALIDACAO_OPERACIONAL)
                        select cxi;

            return View(await query.ToArrayAsync());
        }

        public async Task<ActionResult> DeletarCaixinha(Int32 id)
        {
            var usuario = Session.UsuarioId();

            var cr = await _db.CAIXINHA_ITEM
                .FirstOrDefaultAsync(ci => ci.ID == id && ci.PESSOA == usuario && !ci.VALIDACAO_FINANCEIRO && !ci.VALIDACAO_OPERACIONAL);

            if (cr == null)
                return HttpNotFound();


            _db.Entry(cr)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Index()
        {

            return View();
        }

        static Int32[] ids = new Int32[] { 34, 31, 44, 13, 42, 12, 35, 59 };

        public async Task<ActionResult> Quitar()
        {
            ViewBag.DESPESA = new SelectList(await _db.DESPESA
                .Where(dp => ids.Contains(dp.ID))
                .Include(dp => dp.DESPESA_CLASSE)
                .OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO)
                .ThenBy(dp => dp.DESCRICAO)
                .ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", (Object)null);

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", (Object)null);

            return View(new CAIXINHA_ITEM() { DATA_QUITADO = DateTime.Today });

        }

        public async Task<ActionResult> CaixinhaEdit(int id = 0)
        {
            var cx = await _db.CAIXINHA_ITEM.FirstOrDefaultAsync(c => c.ID == id);

            if (cx == null || cx.PESSOA != Session.UsuarioId())
                return HttpNotFound();

            ViewBag.DESPESA = new SelectList(await _db.DESPESA
                .Where(dp => ids.Contains(dp.ID))
                .Include(dp => dp.DESPESA_CLASSE)
                .OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO)
                .ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", cx.DESPESA);

            ViewBag.PROJETO = new SelectList(await _db.PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO", cx.PROJETO);

            return View(cx);

        }


        [HttpGet]
        public async Task<ActionResult> VisualizarArquivoCaixinha(Int32 id)
        {

            var item = await _db
                .CAIXINHA_ITEM
                .Include(ci => ci.ANEXO1)
                .FirstOrDefaultAsync(ci => ci.ID == id);

            if (item != null && item.ANEXO1 != null && item.PESSOA == Session.UsuarioId())
            {

                Response.AddHeader("Content-Disposition", "inline; filename=" + item.ANEXO1.NOME);
                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME));
            }

            return HttpNotFound();

        }

        [HttpPost]
        public async Task<ActionResult> EnviarArquivoCaixinha(Int32 id, HttpPostedFileBase file)
        {
            var item = await _db
                .CAIXINHA_ITEM
                .Include(cl => cl.ANEXO1)
                .FirstOrDefaultAsync(cx => cx.ID == id);

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CaixinhaEdit(CAIXINHA_ITEM item)
        {
            if (item.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

            if(await _db.CAIXINHA_ITEM.Where(ci => ci.ID == item.ID).Select(ci => ci.VALIDACAO_FINANCEIRO && ci.VALIDACAO_OPERACIONAL).FirstOrDefaultAsync())
                ModelState.AddModelError("", "Caixinha já validada!");


            item.PESSOA = Session.UsuarioId();

            if (ModelState.IsValid)
            {

                if (!String.IsNullOrEmpty(item.DESCRICAO))
                {
                    item.DESCRICAO = item.DESCRICAO.ToUpper();
                }

                _db.Configuration.ValidateOnSaveEnabled = false;

                _db.CAIXINHA_ITEM.Attach(item);

                var entry = _db.Entry(item);

                entry.Property(e => e.OSSB).IsModified = true;

                entry.Property(e => e.DESCRICAO).IsModified = true;

                entry.Property(e => e.DESPESA).IsModified = true;
                entry.Property(e => e.DATA_QUITADO).IsModified = true;

                entry.Property(e => e.VALOR).IsModified = true;

                entry.Property(e => e.NOTA_FISCAL).IsModified = true;

                await _db.SaveChangesAsync();

                return RedirectToAction("NaoValidadas", "AreaDoFuncionario");
            }

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", item.DESPESA);


            return View(item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Quitar(CAIXINHA_ITEM item)
        {
            if (item.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

            item.PESSOA = Session.UsuarioId();

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files[0];

                if (file.ContentLength > 0)
                {

                    byte[] buffer = new byte[file.ContentLength];

                    file.InputStream.Read(buffer, 0, buffer.Length);

                    ANEXO anexo = new ANEXO()
                    {
                        NOME = file.FileName,
                        BUFFER = buffer
                    };

                    item.ANEXO1 = anexo;
                }

                if (!String.IsNullOrEmpty(item.DESCRICAO))
                {
                    item.DESCRICAO = item.DESCRICAO.ToUpper();
                }

                _db.CAIXINHA_ITEM.Add(item);

                await _db.SaveChangesAsync();

                return RedirectToAction("NaoValidadas", "AreaDoFuncionario");
            }

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", item.DESPESA);


            return View(item);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
