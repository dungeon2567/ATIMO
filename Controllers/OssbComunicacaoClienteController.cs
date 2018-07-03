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
    public class OssbComunicacaoClienteController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /OssbComunicacao/

        public async Task<ActionResult> Index(int id = 0)
        {
            var ossbComunicacao = await _db
                     .OSSB_COMUNICACAO
                     .Include(o => o.OSSB1)
                     .Include(o => o.PESSOA1)
                     .Where(o => o.OSSB == id)
                     .Where(o => o.TIPO == "E" || o.TIPO == "S")
                     .ToArrayAsync();

            var viewModel = new OssbComunicacaoIndexViewModel
            {
                Ossb = id,
                Items = ossbComunicacao,
            };

            return View(viewModel);
        }

        //
        // GET: /OssbComunicacao/Create

        public ActionResult Create(int id = 0)
        {

            var viewModel = new OSSB_COMUNICACAO { OSSB = id,
                TIPO = "E",
                PESSOA = Session.UsuarioId()
            };

            return View(viewModel);
        }

        //
        // POST: /OssbComunicacao/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OSSB_COMUNICACAO ossbComunicacao)
        {
            if (!ModelState.IsValid)
                return View(ossbComunicacao);

            ossbComunicacao.TEXTO = ossbComunicacao.TEXTO.ToUpper();

            ossbComunicacao.DATA = DateTime.Today;

            _db.OSSB_COMUNICACAO.Add(ossbComunicacao);

            _db.SaveChanges();

            return RedirectToAction("Index", new { id = ossbComunicacao.OSSB });
        }

        //
        // GET: /OssbComunicacao/Edit/5

        public async Task<ActionResult> Edit(int id = 0)
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

        //
        // POST: /OssbComunicacao/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OSSB_COMUNICACAO ossbComunicacao)
        {
            if (!ModelState.IsValid)
                return View(ossbComunicacao);

            ossbComunicacao.TEXTO = ossbComunicacao.TEXTO.ToUpper();

            _db.Entry(ossbComunicacao)
                .State = EntityState.Modified;

            _db.SaveChanges();

            return RedirectToAction("Index", new { id = ossbComunicacao.OSSB });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}