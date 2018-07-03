using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class ServicoController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            var servico = _db.SERVICO.Include(s => s.ESPECIALIDADE1);
            return View(servico.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.ESPECIALIDADE = new SelectList(_db.ESPECIALIDADE.Where(e => e.SITUACAO == "A"), "ID", "DESCRICAO");
            return View();
        }

        public ActionResult ConfirmarCreate(SERVICO servico)
        {
            #region Validações

            if (servico.ESPECIALIDADE <= 0)
                return Json(new {status = 0, ex = "Informe uma especialidade!"});

            if (string.IsNullOrEmpty(servico.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            if (string.IsNullOrEmpty(servico.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Incluir Serviço

            if (!string.IsNullOrEmpty(servico.DESCRICAO))
                servico.DESCRICAO = servico.DESCRICAO.ToUpper();

            _db.SERVICO.Add(servico);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var servico = _db.SERVICO.Find(id);

            if (servico == null)
            {
                return HttpNotFound();
            }

            ViewBag.ESPECIALIDADE = new SelectList(_db.ESPECIALIDADE.Where(e => e.SITUACAO == "A"), "ID", "DESCRICAO", servico.ESPECIALIDADE);
            return View(servico);
        }

        public ActionResult ConfirmarEdit(SERVICO servico)
        {
            #region Validações

            if (servico.ESPECIALIDADE <= 0)
                return Json(new { status = 0, ex = "Informe uma especialidade!" });

            if (string.IsNullOrEmpty(servico.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(servico.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Serviço

            if (!string.IsNullOrEmpty(servico.DESCRICAO))
                servico.DESCRICAO = servico.DESCRICAO.ToUpper();

            _db.Entry(servico).State = EntityState.Modified;
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