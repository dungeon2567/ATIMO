using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class DepartamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
 
                return View(_db.DEPARTAMENTO.ToList());
     
        }

        public ActionResult Create()
        {

                return View();

        }

        public ActionResult ConfirmarCreate(DEPARTAMENTO departamento)
        {
            if (Session.IsFuncionario())
            {
                #region Validações

                if (string.IsNullOrEmpty(departamento.DESCRICAO))

                    return Json(new { status = 100, ex = "Informe uma descrição!" });

                if (string.IsNullOrEmpty(departamento.SITUACAO))
                    return Json(new { status = 100, ex = "Informe um situação!" });

                #endregion

                #region Incluir Departamento 

                if (!string.IsNullOrEmpty(departamento.DESCRICAO))
                    departamento.DESCRICAO = departamento.DESCRICAO.ToUpper();

                _db.DEPARTAMENTO.Add(departamento);

                #endregion

                #region Incluir Rateio

                #endregion

                _db.SaveChanges();

                return Json(new { status = 200, msg = "Incluído com sucesso!" });
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var departamento = _db.DEPARTAMENTO.Find(id);

                if (departamento == null)
                {
                    return HttpNotFound();
                }

                return View(departamento);
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult ConfirmarEdit(DEPARTAMENTO departamento)
        {
            if (Session.IsFuncionario())
            {
                #region Validações

                if (string.IsNullOrEmpty(departamento.DESCRICAO))
                    return Json(new { status = 100, ex = "Informe uma descrição!" });

                if (string.IsNullOrEmpty(departamento.SITUACAO))
                    return Json(new { status = 100, ex = "Informe um situação!" });

                #endregion

                #region Alterar Departamento

                if (!string.IsNullOrEmpty(departamento.DESCRICAO))
                    departamento.DESCRICAO = departamento.DESCRICAO.ToUpper();

                _db.Entry(departamento).State = EntityState.Modified;

                #endregion

                _db.SaveChanges();

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