using ATIMO.Models;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using ATIMO;

namespace Atimo.Controllers
{
    public class ContaBancariaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> GetContaBancaria(int? banco = null, int? pessoa = null, int? tipo_conta = null, string conta = null, string agencia = null, int page = 1)
        {
            IQueryable<CONTA_BANCARIA> query = _db.CONTA_BANCARIA
                .Include(cb => cb.BANCO1)
                .Include(cb => cb.PESSOA1);

            if (banco != null)
            {
                query = query.Where(cb => cb.BANCO == banco);
            }

            if (pessoa != null)
            {
                query = query.Where(cb => cb.PESSOA == pessoa);
            }


            if (tipo_conta != null)
            {
                query = query.Where(cb => cb.TIPO_CONTA == tipo_conta);
            }

            if(conta != null)
            {
                query = query.Where(cb => (cb.NUM_CONTA + "-" + cb.DIG_CONTA).StartsWith(conta));
            }

            if (agencia != null)
            {
                query = query.Where(cb => (cb.NUM_AGENCIA + "-" + cb.DIG_AGENCIA).StartsWith(agencia));
            }

            int pageCount = ((await query.CountAsync() - 1) / 25) + 1;

            Int32 min = Math.Max(1, Math.Min(page - 6, pageCount - 11));

            Int32 max = Math.Min(pageCount, min + 11);

            return Json(new
            {
                paginas = Enumerable.Range(min, max),
                objetos = await query.Select(cb => new
                {
                    id = cb.ID,
                    pessoa = cb.PESSOA1.RAZAO,
                    banco = cb.BANCO1.CODIGO + " " + cb.BANCO1.DESCRICAO,
                    tipo_conta = cb.TIPO_CONTA1.SIGLA,
                    num_conta = cb.NUM_CONTA,
                    dig_conta = cb.DIG_CONTA,
                    num_agencia = cb.NUM_AGENCIA,
                    dig_agencia = cb.DIG_AGENCIA
                }).ToArrayAsync()
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.PESSOA = new SelectList(await _db
                     .PESSOA
                     .OrderBy(p => p.RAZAO)
                     .ThenBy(p => p.NOME)
                     .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                     .ToArrayAsync(), "ID", "NOME_COMPLETO");

            ViewBag.BANCO = new SelectList(await _db
                .BANCO
                .OrderBy(tc => tc.DESCRICAO)
                .ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.TIPO_CONTA = new SelectList(await _db
                .TIPO_CONTA
                .OrderBy(tc => tc.SIGLA)
                .ToArrayAsync(), "ID", "SIGLA");

            return View(new CONTA_BANCARIA());
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CONTA_BANCARIA cb)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(cb).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            ViewBag.PESSOA = new SelectList(await _db
                     .PESSOA
                     .OrderBy(p => p.RAZAO)
                     .ThenBy(p => p.NOME)
                     .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                     .ToArrayAsync(), "ID", "NOME_COMPLETO", cb.PESSOA);

            ViewBag.BANCO = new SelectList(await _db
                .BANCO
                .OrderBy(tc => tc.DESCRICAO)
                .ToArrayAsync(), "ID", "DESCRICAO", cb.BANCO);

            ViewBag.TIPO_CONTA = new SelectList(await _db
                .TIPO_CONTA
                .OrderBy(tc => tc.SIGLA)
                .ToArrayAsync(), "ID", "SIGLA", cb.TIPO_CONTA);

            return View(cb);
        }


        public async Task<ActionResult> Edit(Int32 id = 0)
        {

            CONTA_BANCARIA cb = await _db.CONTA_BANCARIA
                .FirstOrDefaultAsync(c => c.ID == id);

            if(cb == null){
                return HttpNotFound();
            }


            ViewBag.PESSOA = new SelectList(await _db
                     .PESSOA
                     .OrderBy(p => p.RAZAO)
                     .ThenBy(p => p.NOME)
                     .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                     .ToArrayAsync(), "ID", "NOME_COMPLETO", cb.PESSOA);

            ViewBag.BANCO = new SelectList(await _db
                .BANCO
                .OrderBy(tc => tc.DESCRICAO)
                .ToArrayAsync(), "ID", "DESCRICAO", cb.BANCO);

            ViewBag.TIPO_CONTA = new SelectList(await _db
                .TIPO_CONTA
                .OrderBy(tc => tc.SIGLA)
                .ToArrayAsync(), "ID", "SIGLA", cb.TIPO_CONTA);

            return View(cb);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CONTA_BANCARIA cb)
        {
            if (ModelState.IsValid)
            {
                _db.CONTA_BANCARIA.Add(cb);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            ViewBag.PESSOA = new SelectList(await _db
                     .PESSOA
                     .OrderBy(p => p.RAZAO)
                     .ThenBy(p => p.NOME)
                     .Select(p => new { ID = p.ID, NOME_COMPLETO = p.RAZAO + " (" + p.NOME + ")" })
                     .ToArrayAsync(), "ID", "NOME_COMPLETO", cb.PESSOA);

            ViewBag.BANCO = new SelectList(await _db
                .BANCO
                .OrderBy(tc => tc.DESCRICAO)
                .ToArrayAsync(), "ID", "DESCRICAO", cb.BANCO);

            ViewBag.TIPO_CONTA = new SelectList(await _db
                .TIPO_CONTA
                .OrderBy(tc => tc.SIGLA)
                .ToArrayAsync(), "ID", "SIGLA", cb.TIPO_CONTA);

            return View(cb);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
