using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System;
using ATIMO;
using ATIMO.ViewModel;
using System.Threading.Tasks;

namespace Atimo.Controllers
{
    public class CompraController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index()
        {
         
                DateTime now = DateTime.Now;

                return View(await _db.COMPRA.Where(
                        cp => _db.COMPRA_PARCELA.Where(cpp => cpp.COMPRA == cp.ID && cpp.DATA_PAGAMENTO == null)
                        .Any())
                        .ToArrayAsync()
                );

        }

        public async Task<ActionResult> Create()
        {

            ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA
                .Where(p => p.SITUACAO == "A")
                .Where(p => p.FORNECEDOR == 1).ToArrayAsync(), "ID", "RAZAO");

            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", (Object)null);


            ViewBag.UNIDADES = _db.UNIDADE.ToDictionary(u => u.ID.ToString(), u => u.SIGLA);

            COMPRA c = new COMPRA() { DATA_VENCIMENTO = DateTime.Today, PARCELAS = 1 };

            c.COMPRA_ITEM.Add(new COMPRA_ITEM() { DESCRICAO = "", QUANTIDADE = 1, VALOR = 0, UNIDADE = 1 });

            return View(c);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(COMPRA compra)
        {

            if (compra.OSSB != null && !_db.OSSB.Any(ossb => ossb.ID == compra.OSSB))
                ModelState.AddModelError("", "Informe uma ossb válida");

            if (compra.FORNECEDOR == 0)
                ModelState.AddModelError("", "Informe um fornecedor!");

            if (compra.DESPESA == 0)
                ModelState.AddModelError("", "Informe uma despesa!");

            if (compra.PROJETO == 0)
                ModelState.AddModelError("", "Informe um projeto!");

            if (!compra.COMPRA_ITEM.Any())
            {
                ModelState.AddModelError("", "Informe pelo menos um item na lista de items!");
            }

            if (ModelState.IsValid)
            {
                var vencimento = compra.DATA_VENCIMENTO;

                var valor = compra
                    .COMPRA_ITEM
                    .Select(ci => ci.VALOR * ci.QUANTIDADE)
                    .DefaultIfEmpty()
                    .Sum() / compra.PARCELAS;

                for (Int32 it = 0; it < compra.PARCELAS; ++it)
                {
                    var parcela = new COMPRA_PARCELA()
                    {
                        VALOR = valor,
                        DATA_VENCIMENTO = vencimento.AddMonths(it),
                    };

                    compra.COMPRA_PARCELA.Add(parcela);
                }

                _db.COMPRA.Add(compra);


                _db.SaveChanges();



                return RedirectToAction("Index", "Compra");
            }


            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", compra.PROJETO);
            ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", compra.DESPESA);


            ViewBag.FORNECEDOR = new SelectList(_db.PESSOA
                    .Where(p => p.SITUACAO == "A")
                    .Where(p => p.FORNECEDOR == 1), "ID", "RAZAO", compra.FORNECEDOR);

            ViewBag.UNIDADES = _db.UNIDADE.ToDictionary(u => u.ID.ToString(), u => u.SIGLA);

            return View(compra);

        }

        public async Task<ActionResult> Edit(int id = 0)
        {

                var conta = _db.COMPRA
                    .Include(cp => cp.COMPRA_PARCELA)
                    .FirstOrDefault(cp => cp.ID == id);

                if (conta != null)
                {
                    conta.DATA_VENCIMENTO = _db.COMPRA_PARCELA
                        .Where(cpp => cpp.COMPRA == conta.ID)
                        .OrderBy(cpp => cpp.DATA_VENCIMENTO)
                        .Select(cpp => cpp.DATA_VENCIMENTO)
                        .First();

                    ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", conta.PROJETO);

                    ViewBag.FORNECEDOR = new SelectList(_db.PESSOA
                        .Where(p => p.SITUACAO == "A")
                        .Where(p => p.FORNECEDOR == 1), "ID", "RAZAO", conta.FORNECEDOR);

                    ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", conta.DESPESA);

                    ViewBag.UNIDADES = _db.UNIDADE.ToList();

                    return View(conta);
                }
                else
                {
                    return HttpNotFound();
                }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(COMPRA conta)
        {

                if (conta.OSSB != null && !_db.OSSB.Any(ossb => ossb.ID == conta.OSSB))
                    ModelState.AddModelError("", "Informe uma ossb válida");

                if (conta.FORNECEDOR == 0)
                    ModelState.AddModelError("", "Informe um fornecedor!");

                if (conta.DESPESA == 0)
                    ModelState.AddModelError("", "Informe uma despesa!");

                if (conta.PROJETO == 0)
                    ModelState.AddModelError("", "Informe um projeto!");

                if (ModelState.IsValid)
                {
                    _db.Entry(conta).State = EntityState.Modified;

                    _db.SaveChanges();

                    return RedirectToAction("Index", "Compra");
                }


                ViewBag.PROJETO = new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", conta.PROJETO);
                ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", conta.DESPESA);

                ViewBag.FORNECEDOR = new SelectList(_db.PESSOA
                    .Where(p => p.SITUACAO == "A")
                        .Where(p => p.FORNECEDOR == 1), "ID", "RAZAO", conta.FORNECEDOR);

                ViewBag.UNIDADES = _db.UNIDADE.ToList();

                return View(conta);

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}