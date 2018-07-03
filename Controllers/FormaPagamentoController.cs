using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class FormaPagamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            if (Session.IsFuncionario())
            {

                return View(_db.FORMA_PAGAMENTO.ToList());
            }
            else
                return RedirectToAction("", "");
        }

        public async Task<ActionResult> Create()
        {
            if (Session.IsFuncionario())
            {
                ViewBag.BANCO = new SelectList(await _db.BANCO.Where(b => b.SITUACAO == "A").ToArrayAsync(), "ID", "DESCRICAO");

                return View();
            }
            else
                return RedirectToAction("", "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FORMA_PAGAMENTO formaPagamento)
        {
            if (Session.IsFuncionario())
            {

                if (string.IsNullOrEmpty(formaPagamento.DESCRICAO))
                    ModelState.AddModelError("", "Informe uma descrição!");

                if (string.IsNullOrEmpty(formaPagamento.SITUACAO))
                    ModelState.AddModelError("", "Informe uma situação!");


                if (!string.IsNullOrEmpty(formaPagamento.DESCRICAO))
                    formaPagamento.DESCRICAO = formaPagamento.DESCRICAO.ToUpper();

                if (ModelState.IsValid)
                {
                    _db.FORMA_PAGAMENTO.Add(formaPagamento);
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                ViewBag.BANCO = new SelectList(await _db.BANCO.Where(b => b.SITUACAO == "A").ToArrayAsync(), "ID", "DESCRICAO");

                return View(formaPagamento);
            }
            else
                return RedirectToAction("", "");
        }

        public async Task<ActionResult> Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var formaPagamento = _db.FORMA_PAGAMENTO.Find(id);

                if (formaPagamento == null)
                {
                    return HttpNotFound();
                }

                ViewBag.BANCO = new SelectList(await _db.BANCO.Where(b => b.SITUACAO == "A").ToArrayAsync(), "ID", "DESCRICAO");


                return View(formaPagamento);
            }
            else
                return RedirectToAction("", "");
        }

        [HttpPost]
        public ActionResult Edit(FORMA_PAGAMENTO formaPagamento)
        {
            if (Session.IsFuncionario())
            {
                #region Validações 

                if (string.IsNullOrEmpty(formaPagamento.DESCRICAO))
                    return Json(new { status = 100, ex = "Informe uma descrição!" });

                if (string.IsNullOrEmpty(formaPagamento.SITUACAO))
                    return Json(new { status = 100, ex = "Informe uma situação!" });

                #endregion

                #region Alterar Forma de Pagamento

                if (!string.IsNullOrEmpty(formaPagamento.DESCRICAO))
                    formaPagamento.DESCRICAO = formaPagamento.DESCRICAO.ToUpper();

                _db.Entry(formaPagamento).State = EntityState.Modified;
                _db.SaveChanges();

                #endregion

                return RedirectToAction("Index");
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