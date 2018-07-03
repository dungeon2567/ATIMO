using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class AreaManutencaoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /AreaManutencao/

        public ActionResult Index()
        {

            return View(_db.AREA_MANUTENCAOSet.ToList());

        }

        //
        // GET: /AreaManutencao/Create

        public ActionResult Create()
        {

            return View();

        }

        //
        // POST: /AreaManutencao/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AREA_MANUTENCAO areaManutencao)
        {
            if (!ModelState.IsValid)
                return View(areaManutencao);

            if (!string.IsNullOrEmpty(areaManutencao.DESCRICAO))
                areaManutencao.DESCRICAO = areaManutencao.DESCRICAO.ToUpper();

            _db.AREA_MANUTENCAOSet.Add(areaManutencao);
            _db.SaveChanges();

            return RedirectToAction("Index");

        }

        //
        // GET: /AreaManutencao/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var areaManutencao = _db.AREA_MANUTENCAOSet.Find(id);

                if (areaManutencao == null)
                {
                    return HttpNotFound();
                }

                return View(areaManutencao);
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /AreaManutencao/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AREA_MANUTENCAO areaManutencao)
        {

            if (!ModelState.IsValid)
                return View(areaManutencao);

            if (!string.IsNullOrEmpty(areaManutencao.DESCRICAO))
                areaManutencao.DESCRICAO = areaManutencao.DESCRICAO.ToUpper();

            _db.Entry(areaManutencao).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}