using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System;
using ATIMO;

namespace Atimo.Controllers
{
    public class ComoConheceuController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {

                return View(_db.COMO_CONHECEU.ToList());

        }

        public ActionResult Create()
        {

                return View();

        }

        public ActionResult ConfirmarCreate(COMO_CONHECEU comoConheceu)
        {

                if (ModelState.IsValid)
                {
                    _db.COMO_CONHECEU.Add(comoConheceu);
                    _db.SaveChanges();

                    return Json(new { status = 200, msg = "Incluído com sucesso!" });
                }

                return View(comoConheceu);

        }

        public ActionResult Edit(int id = 0)
        {

                COMO_CONHECEU como_conheceu = _db.COMO_CONHECEU.Find(id);
                if (como_conheceu == null)
                {
                    return HttpNotFound();
                }

                return View(como_conheceu);

        }

       public ActionResult ConfirmarEdit(COMO_CONHECEU comoConheceu)
        {
   
                if (ModelState.IsValid)
                {
                    _db.Entry(comoConheceu).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(comoConheceu);

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}