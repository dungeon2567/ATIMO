using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using System.Collections.Generic;

namespace Atimo.Controllers
{
    public class CaixinhaDevolvidaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Devolvidas(Int32 pessoa, String de = null, String ate = null)
        {

            IQueryable<CAIXINHA_ITEM> query = _db.CAIXINHA_ITEM
                .Include(cx => cx.PESSOA1)
                .Where(cx => cx.PESSOA == pessoa && cx.VALIDACAO_FINANCEIRO && cx.VALIDACAO_OPERACIONAL);


            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                query = query.Where(cx => cx.DATA_QUITADO >= deDate);
            }


            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);

                query = query.Where(cx => cx.DATA_QUITADO <= ateDate);
            }

            ViewBag.DE = de;
            ViewBag.ATE = ate;

            query = query
                .OrderBy(q => q.DATA_QUITADO)
                .ThenBy(q => q.ID);

            return View(new ATIMO.ViewModel.CaixinhaDevolvidaViewModel() { PESSOA = pessoa, ITEMS = await query.ToArrayAsync() });

        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<FileResult> BaixarArquivoPagamento(Int32 id)
        {
            var item = await _db
                .PAGAMENTO_TERCEIRO
                .Include(cl => cl.ANEXO1)
                .FirstOrDefaultAsync(pt => pt.ID == id);

            if (item != null && item.ANEXO1 != null)
            {
                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME), item.ANEXO1.NOME);
            }

            return null;

            
        }

        public async Task<JsonResult> GetPessoas(String query)
        {

            var pessoas = await ((from p in _db.PESSOA
                                  where p.SITUACAO == "A" && p.TERCEIRO == 1 || p.FUNCIONARIO == 1 && p.RAZAO.StartsWith(query)
                                  select p).ToArrayAsync());

            return Json(new { status = 0, pessoas = pessoas.Select(p => new { id = p.ID, text = p.RAZAO }) }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}