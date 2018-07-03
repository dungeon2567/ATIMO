using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class DespesaClasseController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.DESPESA_CLASSE.ToArray());
        }

        public ActionResult Create()
        {
            return View(new DESPESA_CLASSE() { DESCRICAO = "" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DESPESA_CLASSE classe)
        {

            if (string.IsNullOrEmpty(classe.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");

            if (!string.IsNullOrEmpty(classe.DESCRICAO))
                classe.DESCRICAO = classe.DESCRICAO.ToUpper();


            if (ModelState.IsValid)
            {
                _db.DESPESA_CLASSE.Add(classe);

                _db.SaveChanges();

                return RedirectToAction("Index", "DespesaClasse");
            }

            return View(classe);
        }

        public ActionResult Edit(int id = 0)
        {
            var classe = _db.DESPESA_CLASSE
            .FirstOrDefault(dsp => dsp.ID == id);

            if (classe == null)
                return HttpNotFound();

            return View(classe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DESPESA_CLASSE classe)
        {

            if (string.IsNullOrEmpty(classe.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");


            if (ModelState.IsValid)
            {

                classe.DESCRICAO = classe.DESCRICAO.ToUpper();

                _db.Entry(classe)
                    .State = EntityState.Modified;


                _db.SaveChanges();

                return RedirectToAction("Index", "DespesaClasse");
            }

            return View(classe);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}