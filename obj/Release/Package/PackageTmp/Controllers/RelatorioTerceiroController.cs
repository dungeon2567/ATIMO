using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;

namespace Atimo.Controllers
{
    public class RelatorioTerceiroController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(int? terceiro = null, String de = null, String ate = null)
        {

            IQueryable<PAGAMENTO_TERCEIRO> query = _db.PAGAMENTO_TERCEIRO
                .Include(pt => pt.OSSB_SERVICO1)
                .Include(pt => pt.TERCEIRO1);

            if (terceiro != null)
            {
                query = query.Where(pt => pt.TERCEIRO == terceiro);
            }

            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                query = query.Where(pt => pt.DATA_PAGAMENTO >= deDate);
            }


            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);

                query = query.Where(pt => pt.DATA_PAGAMENTO <= ateDate);
            }


            query = query
                .OrderBy(q => q.DATA_PAGAMENTO)
                .ThenBy(q => q.ID);

            return View(await query.ToArrayAsync());

        }

        public ActionResult Search()
        {

            return View();
        }

        public async Task<ActionResult> Deletar(Int32 id)
        {
            var cr = await _db.PAGAMENTO_TERCEIRO
                .FirstOrDefaultAsync(ptt => ptt.ID == id);

            if (cr == null)
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

            _db.Entry(cr)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }


        public async Task<FileResult> VisualizarArquivo(Int32 id)
        {
            var item = await _db
                .PAGAMENTO_TERCEIRO
                .Include(cl => cl.ANEXO1)
                .FirstOrDefaultAsync(pt => pt.ID == id);

            if (item != null && item.ANEXO1 != null)
            {
                Response.AddHeader("Content-Disposition", "inline; filename=" + item.ANEXO1.NOME);

                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME), item.ANEXO1.NOME);
            }

            return null;
        }

        public async Task<JsonResult> GetTerceiros(String query)
        {
            if (Session.IsFuncionario())
            {
                var terceiros = await ((from p in _db.PESSOA
                                        where p.SITUACAO == "A" && p.TERCEIRO == 1 && (p.RAZAO.Contains(query) || p.NOME.Contains(query))
                                        select p).ToArrayAsync());

                return Json(new { status = 0, terceiros = terceiros.Select(p => new { id = p.ID, text = p.NOME }) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}