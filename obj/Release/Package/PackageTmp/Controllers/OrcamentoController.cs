using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using ATIMO;

namespace Atimo.Controllers
{
    public class OrcamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /Orcamento/
        //
        // GET: /Orcamento/Edit/5

        public async Task< ActionResult> Edit(int id = 0)
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

            return View(ossb);

        }

        //
        // POST: /Orcamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OSSB ossb)
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

                return View(ossb);
     
        }

        public JsonResult AprovarOs(int id, String historico)
        {
            var ossb = _db.OSSB.FirstOrDefault(os => os.ID == id);

            ossb.SITUACAO = "E";

                ossb.EXECUCAO_INICIO = DateTime.Now.Date;

            _db.Entry(ossb).State = EntityState.Modified;
            _db.SaveChanges();

            return Json(new object(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult ListaClientes(int id)
        {
            //    var projetoClientes = _db.CLIENTE_PROJETO.Where(cp => cp.SITUACAO == "A").Where(cp => cp.PROJETO == id).Select(cp => cp.CLIENTE).ToList();
            //     var clientes = _db.PESSOA.Where(p => p.CLIENTE == 1).Where(p => p.SITUACAO == "A").Where(p => projetoClientes.Contains(p.ID));

            var clientes = new PESSOA[0];


            return Json(new SelectList(clientes.ToArray(), "ID", "RAZAO"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaContratos(int id)
        {
            var contrato = _db.CONTRATO.Where(c => c.SITUACAO == "A");
            return Json(new SelectList(contrato.ToArray(), "ID", "DESCRICAO"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaLojas(int id)
        {
            var contratoLoja = _db
                .CONTRATO_LOJA
                .Where(cl => cl.CONTRATO == id)
                .Select(cl => cl.LOJA)
                .ToList();

            var lojas = _db.LOJA.Where(l => l.SITUACAO == "A").Where(l => contratoLoja.Contains(l.ID));

            return Json(new SelectList(lojas.ToArray(), "ID", "RAZAO"), JsonRequestBehavior.AllowGet);
        }
    }
}