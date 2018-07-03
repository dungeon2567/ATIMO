using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class DepartamentoRateioController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index(int id = 0)
        {
            var rateio = _db
                .RATEIO
                .Include(r => r.DEPARTAMENTO1)
                .Where(r => r.DEPARTAMENTO == id);
            return View(rateio.ToList()); ;
        }

        public ActionResult Create(int id = 0)
        {
            var rateio = new RATEIO {DEPARTAMENTO = id, SITUACAO = "A"};
            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO");
            return View(rateio);
        }

        public ActionResult ConfirmarCreate(RATEIO rateio)
        {
            #region Validações

            var existe = _db.RATEIO.Any(r => r.DEPARTAMENTO == rateio.DEPARTAMENTO && r.PROJETO == rateio.PROJETO);

            if (existe)
                return Json(new {status = 100, ex = "Projeto já informado para este departamento!"});

            #endregion

            #region Incluir Rateio

            _db.RATEIO.Add(rateio);

            #endregion

            _db.SaveChanges();

            return Json(new { status = 200, msg = "Incluído com sucesso!" });
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

        public ActionResult ConfirmarEdit(RATEIO rateio)
        {
            #region Validações

            var existe = _db.RATEIO.Any(r => r.DEPARTAMENTO == rateio.DEPARTAMENTO && r.PROJETO == rateio.PROJETO && r.ID != rateio.ID);

            if (existe)
                return Json(new {status = 100, ex = "Projeto já informado para este departamento!"});

            #endregion

            #region Alterar Rateio

            _db.Entry(rateio).State = EntityState.Modified;

            #endregion

            _db.SaveChanges();

            return Json(new { status = 200, msg = "Alterado com sucesso!" });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
