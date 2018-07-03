using System;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class RateioFerramentaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public ActionResult Index(int id = 0)
        {
            var rateio = _db
                .RATEIO
                .Include(r => r.DEPARTAMENTO1)
                .Include(r => r.EQUIPAMENTO1)
                .Include(r => r.FERRAMENTA1)
                .Include(r => r.PESSOA1)
                .Include(r => r.PROJETO1)
                .Include(r => r.VEICULO1)
                .Where(r => r.FERRAMENTA == id);

            return View(new RateioIndexViewModel() { Id = id, Lista = rateio.ToList() });
        }

        public ActionResult Create(int id = 0)
        {
            var rateio = new RATEIO {FERRAMENTA = id};

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO");
            return View(rateio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RATEIO rateio)
        {
            if (VerificarProjeto(rateio, "N"))
                ModelState.AddModelError(string.Empty, "Projeto já informado para esta ferramenta!");

            if (ModelState.IsValid)
            {
                _db.RATEIO.Add(rateio);
                _db.SaveChanges();
                return RedirectToAction("Index/"+ rateio.FERRAMENTA);
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            return View(rateio);
        }

        public ActionResult Edit(int id = 0)
        {
            var rateio = _db.RATEIO.Find(id);
            if (rateio == null)
            {
                return HttpNotFound();
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            return View(rateio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RATEIO rateio)
        {
            if (VerificarProjeto(rateio, "E"))
                ModelState.AddModelError(string.Empty, "Projeto já informado para esta ferramenta!");

            if (ModelState.IsValid)
            {
                _db.Entry(rateio).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index/"+rateio.FERRAMENTA);
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            
            return View(rateio);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private bool VerificarProjeto(RATEIO rateio, string situacao)
        {
            var existe = situacao == "N" ? _db.RATEIO.Any(x => x.FERRAMENTA == rateio.FERRAMENTA && x.PROJETO == rateio.PROJETO) : _db.RATEIO.Any(x => x.FERRAMENTA == rateio.FERRAMENTA && x.PROJETO == rateio.PROJETO && x.ID != rateio.ID);

            return existe;
        }
    }
}