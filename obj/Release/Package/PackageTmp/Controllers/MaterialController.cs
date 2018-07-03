using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class MaterialController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            var material = _db.MATERIAL
                .Include(m => m.FORNECEDOR1)
                .Include(m => m.TIPO1)
                .Include(m => m.UNIDADE1);

            return View(material.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "M"), "ID", "DESCRICAO");

            return View();
        }


        public ActionResult Edit(int id = 0)
        {
            var material = _db.MATERIAL.Find(id);

            if (material == null)
            {
                return HttpNotFound();
            }

            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "M"), "ID", "DESCRICAO", material.TIPO);

            return View(material);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}