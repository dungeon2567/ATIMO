using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class BeneficioController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View(_db.BENEFICIO.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.FORNECEDOR = new SelectList(_db
                .PESSOA
                .Where(p => p.FORNECEDOR == 1)
                .Where(p => p.SITUACAO == "A"), "ID", "NOME");

            return View();
        }

        public ActionResult ConfirmarCreate(BENEFICIO beneficio)
        {
            #region Validações

            if (beneficio.FORNECEDOR <= 0)
                return Json(new { status = 100, ex = "Informe um fornecedor!" });

            if (string.IsNullOrEmpty(beneficio.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(beneficio.CUMULATIVO))
                return Json(new { status = 100, ex = "Informe se é cumulativo ou não!" });

            if (string.IsNullOrEmpty(beneficio.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Beneficio

            if (!string.IsNullOrEmpty(beneficio.DESCRICAO))
                beneficio.DESCRICAO = beneficio.DESCRICAO.ToUpper();

            _db.BENEFICIO.Add(beneficio);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var beneficio = _db.BENEFICIO.Find(id);

            if (beneficio == null)
            {
                return HttpNotFound();
            }

            ViewBag.FORNECEDOR = new SelectList(_db
                .PESSOA
                .Where(p => p.FORNECEDOR == 1)
                .Where(p => p.SITUACAO == "A"), "ID", "NOME", beneficio.FORNECEDOR);
            return View(beneficio);
        }

        public ActionResult ConfirmarEdit(BENEFICIO beneficio)
        {
            #region Validações

            if (beneficio.FORNECEDOR <= 0)
                return Json(new { status = 100, ex = "Informe um fornecedor!" });

            if (string.IsNullOrEmpty(beneficio.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(beneficio.CUMULATIVO))
                return Json(new { status = 100, ex = "Informe se é cumulativo ou não!" });

            if (string.IsNullOrEmpty(beneficio.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Beneficio

            if (!string.IsNullOrEmpty(beneficio.DESCRICAO))
                beneficio.DESCRICAO = beneficio.DESCRICAO.ToUpper();

            _db.Entry(beneficio).State = EntityState.Modified;
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
