using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class OssbComunicacaoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /OssbComunicacao/

        public async Task<ActionResult> Index(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var ossbComunicacao = await _db
                         .OSSB_COMUNICACAO
                         .Include(o => o.OSSB1)
                         .Include(o => o.PESSOA1)
                         .Where(o => o.OSSB == id)
                         .OrderByDescending(o => o.ID)
                         .ToArrayAsync();

                var viewModel = new OssbComunicacaoIndexViewModel
                {
                    Ossb = id,
                    Items = ossbComunicacao,
                    Ocorrencia = await _db.OSSB.Where(os => os.ID == id).Select(os => os.OCORRENCIA).FirstOrDefaultAsync()
                };

                return View(viewModel);
            }
            else
                return RedirectToAction("", "");
        }

        //
        // GET: /OssbComunicacao/Create

        public ActionResult Create(int id = 0)
        {

            var viewModel = new OSSB_COMUNICACAO { OSSB = id, TIPO = "I", PESSOA = Session.UsuarioId() };

            return View(viewModel);
        }

        //
        // POST: /OssbComunicacao/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OSSB_COMUNICACAO ossbComunicacao)
        {
            if (Session.IsFuncionario())
            {
                ossbComunicacao.TEXTO = ossbComunicacao.TEXTO.ToUpper();

                ossbComunicacao.DATA = DateTime.Today;


                if (!ModelState.IsValid)
                    return View(ossbComunicacao);

                _db.OSSB_COMUNICACAO.Add(ossbComunicacao);

                _db.SaveChanges();

                return RedirectToAction("Index", new { id = ossbComunicacao.OSSB });
            }
            else
                return RedirectToAction("", "");
        }

        //
        // GET: /OssbComunicacao/Edit/5

        public async Task<ActionResult> Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var ossbComunicacao = await _db.OSSB_COMUNICACAO
                .Where(o => o.ID == id)
                .FirstOrDefaultAsync();

                if (ossbComunicacao == null)
                {
                    return HttpNotFound();
                }

                return View(ossbComunicacao);
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /OssbComunicacao/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OSSB_COMUNICACAO ossbComunicacao)
        {
            if (Session.IsFuncionario())
            {

                ossbComunicacao.TEXTO = ossbComunicacao.TEXTO.ToUpper();

                if (!ModelState.IsValid)
                    return View(ossbComunicacao);

                _db.Entry(ossbComunicacao).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index", new { id = ossbComunicacao.OSSB });
            }
            else
                return RedirectToAction("", "");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}