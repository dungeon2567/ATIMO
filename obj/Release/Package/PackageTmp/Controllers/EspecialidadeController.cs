using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class EspecialidadeController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.ESPECIALIDADE.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ConfirmarCreate(ESPECIALIDADE especialidade)
        {
            #region Validações

            if (string.IsNullOrEmpty(especialidade.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            if (string.IsNullOrEmpty(especialidade.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Especialidade

            if (!string.IsNullOrEmpty(especialidade.DESCRICAO))
                especialidade.DESCRICAO = especialidade.DESCRICAO.ToUpper();

            _db.ESPECIALIDADE.Add(especialidade);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var especialidade = _db.ESPECIALIDADE.Find(id);

            if (especialidade == null)
            {
                return HttpNotFound();
            }

            return View(especialidade);
        }

        public ActionResult ConfirmarEdit(ESPECIALIDADE especialidade)
        {
            #region Validações

            if (string.IsNullOrEmpty(especialidade.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            if (string.IsNullOrEmpty(especialidade.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Especialidade

            if (!string.IsNullOrEmpty(especialidade.DESCRICAO))
                especialidade.DESCRICAO = especialidade.DESCRICAO.ToUpper();

            _db.Entry(especialidade).State = EntityState.Modified;
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