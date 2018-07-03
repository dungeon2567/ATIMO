using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class DespesaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESCRICAO).ToArray());
        }

        public ActionResult Create()
        {

            ViewBag.CLASSE = new SelectList(_db.DESPESA_CLASSE.ToArray(), "ID", "DESCRICAO", (object)null);

            return View(new DESPESA());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DESPESA despesa)
        {

            if (string.IsNullOrEmpty(despesa.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");

            if (!string.IsNullOrEmpty(despesa.DESCRICAO))
                despesa.DESCRICAO = despesa.DESCRICAO.ToUpper();


            if (ModelState.IsValid)
            {
                _db.DESPESA.Add(despesa);

                _db.SaveChanges();

                return RedirectToAction("Index", "Despesa", new { id = despesa.CLASSE });
            }

            ViewBag.CLASSE = new SelectList(_db.DESPESA_CLASSE.ToArray(), "ID", "DESCRICAO", despesa.CLASSE);

            return View(despesa);
        }

        public ActionResult Edit(int id = 0)
        {

            var despesa = _db.DESPESA
            .FirstOrDefault(dsp => dsp.ID == id);

            if (despesa == null)
                return HttpNotFound();

            ViewBag.CLASSE = new SelectList(_db.DESPESA_CLASSE.ToArray(), "ID", "DESCRICAO", despesa.CLASSE);


            return View(despesa);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DESPESA despesa)
        {

            if (string.IsNullOrEmpty(despesa.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");

            if (!string.IsNullOrEmpty(despesa.DESCRICAO))
                despesa.DESCRICAO = despesa.DESCRICAO.ToUpper();


            if (ModelState.IsValid)
            {

                _db.Entry(despesa)
                    .State = EntityState.Modified;

                _db.SaveChanges();

                ViewBag.CLASSE = new SelectList(_db.DESPESA_CLASSE.ToArray(), "ID", "DESCRICAO", despesa.CLASSE);

                return RedirectToAction("Index", "Despesa");
            }

            return View(despesa);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}