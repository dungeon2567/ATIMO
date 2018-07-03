using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ATIMO;

namespace Atimo.Controllers
{
    public class OrcarController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                var ossb = await _db
                .OSSB
                .Include(os => os.CONTRATO1)
                .Include(os => os.LOJA1)
                .Include(os => os.PESSOA)
                .Include(os => os.PROJETO1)
                .FirstOrDefaultAsync(os => os.ID == id);

                if (ossb == null)
                {
                    return HttpNotFound();
                }

                ViewBag.ORCAMENTISTA = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1 || p.TERCEIRO == 1)
                  .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "NOME", ossb.ORCAMENTISTA);

                return View(ossb);
            }
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /Orcar/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OSSB ossb)
        {
            if (Session.IsFuncionario())
            {

                if (ModelState.IsValid)
                {
                    _db.Entry(ossb).State = EntityState.Modified;

                    try
                    {

                        await _db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException e)
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (var eve in e.EntityValidationErrors)
                        {
                            sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            sb.AppendLine();

                            foreach (var ve in eve.ValidationErrors)
                            {
                                sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);

                                sb.AppendLine();
                            }
                        }

                        throw new Exception(sb.ToString());
                    }
                    return RedirectToAction("Index", "VisitaInicial");
                }


                ViewBag.ORCAMENTISTA = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1 || p.TERCEIRO == 1)
                    .Where(p => p.SITUACAO == "A").ToArrayAsync(), "ID", "RAZAO", ossb.ORCAMENTISTA);

                return View(ossb);
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