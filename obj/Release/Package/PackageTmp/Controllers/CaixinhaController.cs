using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class CaixinhaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Devolvidas(Int32 pessoa, String de = null, String ate = null)
        {

            IQueryable<CAIXINHA_ITEM> query = _db.CAIXINHA_ITEM
                .Include(cx => cx.PESSOA1)
                .Where(cx => cx.PESSOA == pessoa && cx.VALIDACAO_FINANCEIRO && cx.VALIDACAO_OPERACIONAL);


            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                query = query.Where(cx => cx.DATA_QUITADO >= deDate);
            }


            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);

                query = query.Where(cx => cx.DATA_QUITADO <= ateDate);
            }

            ViewBag.DE = de;
            ViewBag.ATE = ate;

            query = query
                .OrderBy(q => q.DATA_QUITADO)
                .ThenBy(q => q.ID);

            return View(new ATIMO.ViewModel.CaixinhaDevolvidaViewModel() { PESSOA = pessoa, ITEMS = await query.ToArrayAsync() });

        }

        public async Task<ActionResult> Pagas(Int32 pessoa, String de = null, String ate = null)
        {
            IQueryable<CAIXINHA> query = _db.CAIXINHA
                .Include(cx => cx.PESSOA1)
                .Where(cx => cx.PESSOA == pessoa);


            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                query = query.Where(cx => cx.DATA_ENTREGA >= deDate);
            }


            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);

                query = query.Where(cx => cx.DATA_ENTREGA <= ateDate);
            }

            ViewBag.DE = de;
            ViewBag.ATE = ate;

            query = query
                .OrderBy(q => q.DATA_ENTREGA)
                .ThenBy(q => q.ID);

            return View(await query.ToArrayAsync());
        }


        public async Task<ActionResult> NaoValidadas(Int32? pessoa = null)
        {
            IQueryable<CAIXINHA_ITEM> query = _db.CAIXINHA_ITEM
                .Include(cxi => cxi.PESSOA1);

            if(pessoa != null)
            {
                query = query.Where(cxi => cxi.PESSOA == pessoa);
            }


            var result = from cxi in query
                        where !cxi.VALIDACAO_FINANCEIRO || !cxi.VALIDACAO_OPERACIONAL
                        select cxi;

            return View(await result.ToArrayAsync());
        }

        private async Task<bool> ChecarValidacao(CAIXINHA_ITEM item)
        {
            if (item.VALIDACAO_FINANCEIRO && item.VALIDACAO_OPERACIONAL)
            {
                var caixinhas = await _db.PAGAMENTO
                    .Where(p => p.PESSOA == item.PESSOA && p.DESPESA == 60)
                    .OrderBy(p => p.DATA_PAGAMENTO)
                    .ToArrayAsync();

                var total = caixinhas.Sum(cx => cx.VALOR);

                if (item.VALOR > total)
                {
                    return false;
                }

                Decimal pagar = item.VALOR;

                Int32 parcela = 0;

                foreach (var caixinha in caixinhas)
                {
                    ++parcela;

                    if (pagar >= caixinha.VALOR)
                    {
                        pagar = pagar - caixinha.VALOR;

                        _db.PAGAMENTO.Add(new PAGAMENTO()
                        {
                            OSSB = item.OSSB,
                            VALOR = caixinha.VALOR,
                            DESPESA = item.DESPESA,
                            DESCRICAO = (item.DESCRICAO ?? "") + " (" + parcela + ")(CAIXINHA)" ,
                            CRITICIDADE = 1,
                            CONTA_BANCARIA = caixinha.CONTA_BANCARIA,
                            PROJETO = item.PROJETO,
                            DATA_PAGAMENTO = caixinha.DATA_PAGAMENTO,
                            DATA_VENCIMENTO = caixinha.DATA_VENCIMENTO,
                            PESSOA = caixinha.PESSOA
                        });

                        _db.Entry(caixinha).State = EntityState.Deleted;


                    }
                    else
                    {
                        _db.PAGAMENTO.Add(new PAGAMENTO()
                        {
                            OSSB = item.OSSB,
                            VALOR = pagar,
                            DESPESA = item.DESPESA,
                            DESCRICAO = (item.DESCRICAO ?? "") + " (" + parcela + ")(CAIXINHA)",
                            CRITICIDADE = 1,
                            CONTA_BANCARIA = caixinha.CONTA_BANCARIA,
                            PROJETO = item.PROJETO,
                            DATA_PAGAMENTO = caixinha.DATA_PAGAMENTO,
                            DATA_VENCIMENTO = caixinha.DATA_VENCIMENTO,
                            PESSOA = caixinha.PESSOA
                        });

                        caixinha.VALOR -= pagar;

                        _db.Entry(caixinha).Property(cx => cx.VALOR).IsModified = true;

                        pagar = 0;
                    }

                    if (pagar == 0)
                    {
                        break;

                        throw new NotSupportedException();
                    }
                }
            }

            return true;
        }


        public async Task<ActionResult> ValidarOperacional(Int32 id = 0)
        {
            var query = from cxi in _db.CAIXINHA_ITEM
                        where cxi.ID == id
                        select cxi;

            var item = await query.FirstOrDefaultAsync();

            if (item == null)
                return HttpNotFound();

            if (item.VALIDACAO_OPERACIONAL == false)
            {

                item.VALIDACAO_OPERACIONAL = true;

                if (await ChecarValidacao(item))
                {
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return Content("Erro: Valor da caixinha maior que o valor total do beneficiario");
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public async Task<ActionResult> ValidarFinanceiro(Int32 id = 0)
        {
            var query = from cxi in _db.CAIXINHA_ITEM
                        where cxi.ID == id
                        select cxi;

            var item = await query.FirstOrDefaultAsync();

            if (item == null)
                return HttpNotFound();

            if (item.VALIDACAO_FINANCEIRO == false)
            {
                item.VALIDACAO_FINANCEIRO = true;

                if (await ChecarValidacao(item))
                {
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return Content("Erro: Valor da caixinha maior que o valor total do beneficiario");
                }

                await _db.SaveChangesAsync();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }



        public async Task<ActionResult> Index()
        {
            var query = from p in _db.PESSOA
                        where p.CAIXINHA.Any() || p.CAIXINHA_ITEM.Any()
                        select new CaixinhaIndexViewModel()
                        {
                            PESSOA = p,
                            RESTANTE = p.CAIXINHA.Select(cx => -cx.VALOR).DefaultIfEmpty().Sum() + p.CAIXINHA_ITEM.Where(cxi => cxi.VALIDACAO_FINANCEIRO && cxi.VALIDACAO_OPERACIONAL).Select(cxi => cxi.VALOR).DefaultIfEmpty().Sum(),
                            LANCADO = p.CAIXINHA_ITEM
                            .Where(ci => (!ci.VALIDACAO_FINANCEIRO) || (!ci.VALIDACAO_OPERACIONAL))
                            .Select(ci => ci.VALOR)
                            .DefaultIfEmpty()
                            .Sum()
                        };


            return View(await query.ToArrayAsync());
        }

        public async Task<ActionResult> Create(Int32? pessoa = null)
        {
            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                .OrderBy(p => p.RAZAO)
                .ToArrayAsync(), "ID", "RAZAO", pessoa);

            ViewBag.CONTA_BANCARIA =
                new SelectList(await _db.CONTA_BANCARIA.Include(cb => cb.BANCO1).ToArrayAsync(), "ID", "DESCRICAO");


            return View(new CAIXINHA() { DATA_ENTREGA = DateTime.Today });
        }

        [HttpGet]
        public async Task<FileContentResult> VisualizarArquivo(Int32 id)
        {

            var item = await _db
                .CAIXINHA_ITEM
                .Include(ci => ci.ANEXO1)
                .FirstOrDefaultAsync(ci => ci.ID == id);

            if (item != null && item.ANEXO1 != null)
            {

                Response.AddHeader("Content-Disposition", "inline; filename=" + item.ANEXO1.NOME);
                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME));
            }

            return null;

        }

        public async Task<ActionResult> Deletar(Int32 id)
        {
            var cr = await _db.CAIXINHA_ITEM
                .FirstOrDefaultAsync(ci => ci.ID == id);

            if (cr == null)
                return HttpNotFound();

            _db.Entry(cr)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CAIXINHA caixinha)
        {
            if (caixinha.PESSOA == 0)
                ModelState.AddModelError("", "Informe uma pessoa!");


            if (caixinha.CONTA_BANCARIA == 0)
            {
                ModelState.AddModelError("", "Informe uma conta bancaria!");
            }

            if (ModelState.IsValid)
            {
                _db.CAIXINHA.Add(caixinha);

                _db.PAGAMENTO.Add(new PAGAMENTO()
                {
                    CONTA_BANCARIA = caixinha.CONTA_BANCARIA,
                    DATA_PAGAMENTO = caixinha.DATA_ENTREGA,
                    DATA_VENCIMENTO = caixinha.DATA_ENTREGA,
                    CRITICIDADE = 1,
                    PROJETO = 4,
                    DESCRICAO = "CAIXINHA",
                    PESSOA = caixinha.PESSOA,
                    VALOR = caixinha.VALOR,
                    DESPESA = 60
                });

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Caixinha");
            }

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                .OrderBy(p => p.RAZAO)
                .ToArrayAsync(), "ID", "RAZAO");

            return View(caixinha);
        }

        public async Task<ActionResult> Quitar(int? despesa = null, int? pessoa = null)
        {

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", despesa);

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", (Object)null);


            if (pessoa != null)
            {
                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO", pessoa);
            }
            else
            {
                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO");
            }

            return View(new CAIXINHA_ITEM() { DATA_QUITADO = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Quitar(CAIXINHA_ITEM item)
        {

            if (item.PESSOA == 0)
                ModelState.AddModelError("", "Informe uma pessoa!");

            if (item.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

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

                TempData["MensagemSucesso"] = "Caixinha devolvida com o valor de " + item.VALOR.ToString("C") + ".";

                return RedirectToAction("Quitar", "Caixinha", new { pessoa = item.PESSOA, despesa = item.DESPESA });
            }

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", item.DESPESA);

            ViewBag.PESSOA = new SelectList(await _db
                .PESSOA
                .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                .OrderBy(p => p.RAZAO)
                .ToArrayAsync(), "ID", "RAZAO", item.PESSOA);

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", item.PROJETO);


            return View(item);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}