using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;
using System.Threading.Tasks;
using System;

namespace Atimo.Controllers
{
    public class TipoContaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetTipoConta(string descricao = null, string sigla = null, int page = 1)
        {
            IQueryable<TIPO_CONTA> query = _db.TIPO_CONTA;

            if (descricao != null)
            {
                query = query.Where(tc => tc.DESCRICAO.Contains(descricao));
            }

            if (sigla != null)
            {
                query = query.Where(tc => tc.SIGLA.Contains(sigla));
            }

            int pageCount = ((await query.CountAsync() - 1) / 25) + 1;

            Int32 min = Math.Max(1, Math.Min(page - 6, pageCount - 11));

            Int32 max = Math.Min(pageCount, min + 11);

            return Json(new { paginas = Enumerable.Range(min, max), objetos = await query.Select(tc => new { id = tc.ID, sigla = tc.SIGLA, descricao = tc.DESCRICAO }).ToListAsync() }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Create()
        {
            return View(new TIPO_CONTA());
        }

        [HttpGet]
        public async Task<JsonResult> GetTiposConta(string query)
        {
            IQueryable<TIPO_CONTA> tiposConta = _db.TIPO_CONTA;

            tiposConta = tiposConta.Where(tc => tc.DESCRICAO.Contains(query) || tc.SIGLA.Contains(query));

            return Json( await tiposConta.Select(tc => new { id = tc.ID, text = tc.SIGLA }).ToArrayAsync(), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}