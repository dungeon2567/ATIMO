using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;
using System;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.Generic;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class ExtratoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /AreaManutencao/

        public async Task<ActionResult> Index()
        {

            DateTime firstDay = DateTime.Today.AddDays(-3);

            var k = from cb in _db.CONTA_BANCARIA
                    select new
                    {
                        CONTA = cb,
                        TRANSACOES = cb.PAGAMENTO
                        .Where(p => p.DATA_PAGAMENTO != null)
                        .Select(p => new ExtratoViewModel.TRANSACAO { DATA = (DateTime)p.DATA_PAGAMENTO, VALOR = -p.VALOR, CONTA_BANCARIA = p.CONTA_BANCARIA1, PESSOA = p.PESSOA1.RAZAO, NOTA_FISCAL = (String)null, OSSB = p.OSSB, DESPESA = p.DESPESA1.DESCRICAO })
                        .Concat(cb.CONTAS_RECEBER.Where(cr => cr.CONTA_BANCARIA != null && cr.DATA_RECEBIMENTO != null)
                        .Select(cr => new ExtratoViewModel.TRANSACAO { DATA = (DateTime)cr.DATA_RECEBIMENTO, VALOR = cr.VALOR_LIQUIDO, CONTA_BANCARIA = cr.CONTA_BANCARIA1, PESSOA = cr.OSSB1.PESSOA.RAZAO, NOTA_FISCAL = cr.NOTA_FISCAL, OSSB = (Int32?)cr.OSSB, DESPESA = (String)null }))
                    };


            var saldo = await (from cb in k
                               select new
                               {
                                   CONTA = cb.CONTA.ID,
                                   VALOR = cb.CONTA.SALDO_INICIAL + cb.TRANSACOES.Select(p => p.VALOR).DefaultIfEmpty().Sum()
                               }).ToDictionaryAsync(kv => kv.CONTA, kv => kv.VALOR);

            var transacoes = await (from cb in k
                                    select new
                                    {
                                        CONTA_BANCARIA = cb.CONTA,
                                        TRANSACOES = cb.TRANSACOES.Where(t => t.DATA >= firstDay)
                                    }).ToDictionaryAsync(kv => kv.CONTA_BANCARIA, kv => kv.TRANSACOES);

            return View(new ExtratoViewModel() { SALDO = saldo, TRANSACOES = transacoes });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}