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

namespace Atimo.Controllers
{

    public class OssbTerceiroController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public async Task<ActionResult> Index(int id = 0)
        {
            var ossbTerceiro = await _db.PAGAMENTO
                .Include(p => p.OSSB_SERVICO)
                .Include(p => p.PESSOA1)
                .Where(p => p.OSSB == id)
                .Where(p => p.DESPESA == 16 || p.DESPESA == 56)
                .ToArrayAsync();

            ViewBag.OSSB = id;

            return View(ossbTerceiro);

        }

        public async Task<ActionResult> Create(int id = 0)
        {

            ViewBag.PESSOA = new SelectList(await _db
           .PESSOA
           .Where(p => p.SITUACAO == "A" && p.TERCEIRO == 1)
           .OrderBy(p => p.RAZAO)
           .ThenBy(p => p.NOME)
           .ToArrayAsync(), "ID", "NOME_COMPLETO");

            var situacao = await _db.OSSB.Where(os => os.ID == id).Select(os => os.SITUACAO).FirstOrDefaultAsync();

            ViewBag.SERVICO = new SelectList(await _db.OSSB_SERVICO.Where(s => s.OSSB == id).ToArrayAsync(), "ID", "DESCRICAO", null);


            return View(new PAGAMENTO() { OSSB = id,  DESPESA = situacao == "P" ? 56 : 16 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PAGAMENTO pagamento)
        {
            if (pagamento.PESSOA == 0)
            {
                ModelState.AddModelError("", "Terceiro não informado!");
            }

            if (ModelState.IsValid)
            {

                var proj = await _db.OSSB.Where(os => os.ID == pagamento.OSSB).Select(os => os.PROJETO).FirstOrDefaultAsync();

                pagamento.PROJETO = proj;
                pagamento.CRITICIDADE = 1;

                _db.PAGAMENTO.Add(pagamento);

                _db.SaveChanges();

                return RedirectToAction("Index", new { id = pagamento.OSSB });
            }

            ViewBag.PESSOA = new SelectList(await _db
           .PESSOA
           .Where(p => p.SITUACAO == "A" && p.TERCEIRO == 1)
           .OrderBy(p => p.RAZAO)
           .ThenBy(p => p.NOME)
           .ToArrayAsync(), "ID", "NOME_COMPLETO", pagamento.PESSOA);

            ViewBag.SERVICO = new SelectList(await _db.OSSB_SERVICO.Where(s => s.OSSB == pagamento.OSSB).ToArrayAsync(), "ID", "DESCRICAO", pagamento.PESSOA);



            return View(pagamento);
        }

        public async Task<ActionResult> Edit(int id = 0)
        {
            var pagamento = await _db.PAGAMENTO.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (pagamento == null)
            {
                return HttpNotFound();
            }

            ViewBag.PESSOA = new SelectList(await _db
           .PESSOA
           .Where(p => p.SITUACAO == "A" && p.TERCEIRO == 1)
           .OrderBy(p => p.RAZAO)
           .ThenBy(p => p.NOME)
           .ToArrayAsync(), "ID", "NOME_COMPLETO", pagamento.PESSOA);

            ViewBag.SERVICO = new SelectList(await _db.OSSB_SERVICO.Where(s => s.OSSB == pagamento.OSSB).ToArrayAsync(), "ID", "DESCRICAO", pagamento.SERVICO);



            return View(pagamento);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PAGAMENTO pagamento)
        {
            if (pagamento.PESSOA == 0)
            {
                ModelState.AddModelError("", "Terceiro não informado!");
            }

            if(await _db.PAGAMENTO.Where(p => p.ID == pagamento.ID).Select(p => p.DATA_PAGAMENTO).FirstOrDefaultAsync() != null)
            {
                ModelState.AddModelError("", "Terceiro já pago!");
            }
                 
            if (ModelState.IsValid)
            {
                var entry = _db.Entry(pagamento);

                _db.PAGAMENTO.Attach(pagamento);

                entry.Property(pp => pp.PESSOA).IsModified = true;
                entry.Property(pp => pp.SERVICO).IsModified = true;
                entry.Property(pp => pp.DATA_VENCIMENTO).IsModified = true;
                entry.Property(pp => pp.VALOR).IsModified = true;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { id = pagamento.OSSB});
            }

            ViewBag.PESSOA = new SelectList(await _db
           .PESSOA
           .Where(p => p.SITUACAO == "A" && p.TERCEIRO == 1)
           .OrderBy(p => p.RAZAO)
           .ThenBy(p => p.NOME)
           .ToArrayAsync(), "ID", "NOME_COMPLETO", pagamento.PESSOA);

            ViewBag.SERVICO = new SelectList(await _db.OSSB_SERVICO.Where(s => s.OSSB == pagamento.OSSB).ToArrayAsync(), "ID", "DESCRICAO", pagamento.SERVICO);

            return View(pagamento);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


    }

}