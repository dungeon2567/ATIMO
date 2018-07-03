using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class GrupoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.GRUPO.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ConfirmarCreate(GRUPO grupo)
        {
            #region Validações

            if (string.IsNullOrEmpty(grupo.TIPO))
                return Json(new {status = 100, ex = "Informe um tipo!"});

            if (string.IsNullOrEmpty(grupo.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(grupo.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Grupo

            if (!string.IsNullOrEmpty(grupo.DESCRICAO))
                grupo.DESCRICAO = grupo.DESCRICAO.ToUpper();

            _db.GRUPO.Add(grupo);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var grupo = _db.GRUPO.Find(id);

            if (grupo == null)
            {
                return HttpNotFound();
            }

            return View(grupo);
        }

        public ActionResult ConfirmarEdit(GRUPO grupo)
        {
            #region Validações

            if (string.IsNullOrEmpty(grupo.TIPO))
                return Json(new { status = 100, ex = "Informe um tipo!" });

            if (string.IsNullOrEmpty(grupo.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(grupo.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Grupo

            if (!string.IsNullOrEmpty(grupo.DESCRICAO))
                grupo.DESCRICAO = grupo.DESCRICAO.ToUpper();

            _db.Entry(grupo).State = EntityState.Modified;
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