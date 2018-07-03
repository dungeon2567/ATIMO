using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using ATIMO;

namespace Atimo.Controllers
{
    public class CompraParcelaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index(int id = 0)
        {
            var viewModel = new ATIMO.ViewModel.CompraParcelaViewModel()
            {
                ContaPagar = id,
                Parcelas = _db.COMPRA_PARCELA.Where(p => p.COMPRA == id)
            .ToArray()
            };

            return View(viewModel);
        }


        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var parcela = _db.COMPRA_PARCELA
                .FirstOrDefault(p => p.ID == id);

                if (parcela == null)
                    return HttpNotFound();

                ViewBag.FORMA_PAGAMENTO = new SelectList(_db.FORMA_PAGAMENTO, "ID", "DESCRICAO", parcela.FORMA_PAGAMENTO);

                return View(parcela);
            }
            else

                return RedirectToAction("", "");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(COMPRA_PARCELA parcela)
        {
            if (Session.IsFuncionario())
            {
                if (ModelState.IsValid)
                {

                    _db.Entry(parcela)
                        .State = EntityState.Modified;

                    _db.SaveChanges();

                    return RedirectToAction("Index", "CompraParcela", new { id = parcela.COMPRA });
                }
                else
                {
                    ViewBag.FORMA_PAGAMENTO = new SelectList(_db.FORMA_PAGAMENTO, "ID", "DESCRICAO", parcela.FORMA_PAGAMENTO);

                    return View(parcela);
                }
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