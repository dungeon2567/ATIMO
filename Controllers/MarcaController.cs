using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class MarcaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.MARCA.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ConfirmarCreate(MARCA marca)
        {
            #region Validações 

            if (string.IsNullOrEmpty(marca.TIPO))
                return Json(new {status = 100, ex = "Informe um tipo!"});

            if (string.IsNullOrEmpty(marca.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(marca.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Marca

            if (!string.IsNullOrEmpty(marca.DESCRICAO))
                marca.DESCRICAO = marca.DESCRICAO.ToUpper();

            _db.MARCA.Add(marca);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var marca = _db.MARCA.Find(id);

            if (marca == null)
            {
                return HttpNotFound();
            }

            return View(marca);
        }

        public ActionResult ConfirmarEdit(MARCA marca)
        {
            #region Validações

            if (string.IsNullOrEmpty(marca.TIPO))
                return Json(new { status = 100, ex = "Informe um tipo!" });

            if (string.IsNullOrEmpty(marca.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(marca.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Marca

            if (!string.IsNullOrEmpty(marca.DESCRICAO))
                marca.DESCRICAO = marca.DESCRICAO.ToUpper();

            _db.Entry(marca).State = EntityState.Modified;
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