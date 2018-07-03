using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class TipoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.TIPO.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ConfirmarCreate(TIPO tipo)
        {
            #region Validações

            if (string.IsNullOrEmpty(tipo.TIPO1))
                return Json(new {status = 100, ex = "Informe um tipo!"});

            if (string.IsNullOrEmpty(tipo.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(tipo.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Tipo

            if (!string.IsNullOrEmpty(tipo.DESCRICAO))
                tipo.DESCRICAO = tipo.DESCRICAO.ToUpper();

            _db.TIPO.Add(tipo);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var tipo = _db.TIPO.Find(id);

            if (tipo == null)
            {
                return HttpNotFound();
            }

            return View(tipo);
        }

        public ActionResult ConfirmarEdit(TIPO tipo)
        {
            #region Validações

            if (string.IsNullOrEmpty(tipo.TIPO1))
                return Json(new { status = 100, ex = "Informe um tipo!" });

            if (string.IsNullOrEmpty(tipo.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(tipo.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Tipo

            if (!string.IsNullOrEmpty(tipo.DESCRICAO))
                tipo.DESCRICAO = tipo.DESCRICAO.ToUpper();

            _db.Entry(tipo).State = EntityState.Modified;
            _db.SaveChanges();

            #endregion

            return Json(new { status = 200, msg = "Alterado com sucesso!" });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}