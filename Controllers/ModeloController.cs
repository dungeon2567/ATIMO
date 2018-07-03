using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class ModeloController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.MODELO.Include(md => md.TIPO1).ToArray());
        }

        public ActionResult Create()
        {
            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "E"), "ID", "DESCRICAO");

            return View(new MODELO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MODELO modelo)
        {
            if (modelo.TIPO == 0)
                ModelState.AddModelError(string.Empty, "Informe um tipo!");

            if (string.IsNullOrEmpty(modelo.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");


            if (string.IsNullOrEmpty(modelo.SITUACAO))
                ModelState.AddModelError(string.Empty, "Informe uma situação!");


            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(modelo.DESCRICAO))
                    modelo.DESCRICAO = modelo.DESCRICAO.ToUpper();


                _db.MODELO.Add(modelo);
                _db.SaveChanges();

                return RedirectToAction("Index", "Modelo");
            }

            return View(modelo);
        }

        public ActionResult Edit(int id = 0)
        {
            var modelo = _db.MODELO.Find(id);

            if (modelo == null)
            {
                return HttpNotFound();
            }

            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "E"), "ID", "DESCRICAO", modelo.TIPO);

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MODELO modelo)
        {

            if (modelo.TIPO == 0)
                ModelState.AddModelError(string.Empty, "Informe um tipo!");



            if (string.IsNullOrEmpty(modelo.DESCRICAO))
                ModelState.AddModelError(string.Empty, "Informe uma descrição!");


            if (string.IsNullOrEmpty(modelo.SITUACAO))
                ModelState.AddModelError(string.Empty, "Informe uma situação!");


            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(modelo.DESCRICAO))
                    modelo.DESCRICAO = modelo.DESCRICAO.ToUpper();


                _db.Entry(modelo).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index", "Modelo");
            }

            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "E"), "ID", "DESCRICAO", modelo.TIPO);

            return View(modelo);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}