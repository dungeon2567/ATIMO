using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class FeriadoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.FERIADO.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ConfirmarCreate(FERIADO feriado)
        {
            #region Validações

            if (string.IsNullOrEmpty(feriado.DATA))
                return Json(new {status = 100, ex = "Informe a data!"});

            if (string.IsNullOrEmpty(feriado.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            var existe = _db.FERIADO.Any(f => f.DATA == feriado.DATA);

            if (existe)
                return Json(new {status = 100, ex = "Data já informada para outro feriado!"});

            #endregion

            #region Incluir Feriado

            if (!string.IsNullOrEmpty(feriado.DESCRICAO))
                feriado.DESCRICAO = feriado.DESCRICAO.ToUpper();

            _db.FERIADO.Add(feriado);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var feriado = _db.FERIADO.Find(id);

            if (feriado == null)
            {
                return HttpNotFound();
            }

            return View(feriado);
        }

        public ActionResult ConfirmarEdit(FERIADO feriado)
        {
            #region Validações

            if (string.IsNullOrEmpty(feriado.DATA))
                return Json(new {status = 100, ex = "Informe uma data!"});

            if (string.IsNullOrEmpty(feriado.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            var existe = _db.FERIADO.Any(f => f.DATA == feriado.DATA && f.ID != feriado.ID);

            if (existe)
                return Json(new {status = 100, ex = "Data já informada para outro feriado!"});

            #endregion

            #region Alterar Feriado

            if (!string.IsNullOrEmpty(feriado.DESCRICAO))
                feriado.DESCRICAO = feriado.DESCRICAO.ToUpper();

            _db.Entry(feriado).State = EntityState.Modified;
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Alterado com sucesso!"});
        }

       protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}