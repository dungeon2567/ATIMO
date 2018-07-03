using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class UnidadeController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            if (Session.IsFuncionario())
            {
                return View(_db.UNIDADE.ToList());
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult Create()
        {
            if (Session.IsFuncionario())
                return View();
            else
                return RedirectToAction("", "");
        }

        public ActionResult ConfirmarCreate(UNIDADE unidade)
        {
            if (Session.IsFuncionario())
            {
                #region Validações

                if (string.IsNullOrEmpty(unidade.DESCRICAO))
                    return Json(new { status = 100, ex = "Informe uma descrição!" });

                if (string.IsNullOrEmpty(unidade.SIGLA))
                    return Json(new { status = 100, ex = "Informe uma sigla!" });

                #endregion

                #region Incluir Unidade

                if (!string.IsNullOrEmpty(unidade.DESCRICAO))
                    unidade.DESCRICAO = unidade.DESCRICAO.ToUpper();

                if (!string.IsNullOrEmpty(unidade.SIGLA))
                    unidade.SIGLA = unidade.SIGLA.ToUpper();

                _db.UNIDADE.Add(unidade);
                _db.SaveChanges();

                #endregion

                return Json(new { status = 200, msg = "Incluído com sucesso!" });
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var unidade = _db.UNIDADE.Find(id);

                if (unidade == null)
                {
                    return HttpNotFound();
                }

                return View(unidade);
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult ConfirmarEdit(UNIDADE unidade)
        {
            if (Session.IsFuncionario())
            {
                #region Validações

                if (string.IsNullOrEmpty(unidade.DESCRICAO))
                    return Json(new { status = 100, ex = "Informe uma descrição!" });

                if (string.IsNullOrEmpty(unidade.SIGLA))
                    return Json(new { status = 100, ex = "Informe uma sigla!" });

                #endregion

                #region Alterar Unidade

                if (!string.IsNullOrEmpty(unidade.DESCRICAO))
                    unidade.DESCRICAO = unidade.DESCRICAO.ToUpper();

                if (!string.IsNullOrEmpty(unidade.SIGLA))
                    unidade.SIGLA = unidade.SIGLA.ToUpper();

                _db.Entry(unidade).State = EntityState.Modified;
                _db.SaveChanges();

                #endregion

                return Json(new { status = 200, msg = "Alterado com sucesso!" });
            }
            else
                return RedirectToAction("", "");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}