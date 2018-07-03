using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using ATIMO;
using System.Data.Entity.SqlServer;

namespace Atimo.Controllers
{
    public class RelatorioGeralController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Search()
        {
            ViewBag.PROJETO = new SelectList(await _db.PROJETO
                .ToArrayAsync(), "ID", "DESCRICAO");

            return View();

        }

        public async Task<ActionResult> Index(Int32? entrada_ossb = null, String entrada_tipo = null, Int32 ? entrada_projeto = null, Int32? saida_ossb = null, String saida_tipo = null, Int32? saida_projeto = null, String de = null, String ate = null, String por = "D")
        {
            IQueryable<CONTAS_RECEBER> queryContasReceber = _db.CONTAS_RECEBER.Where(cr => cr.DATA_RECEBIMENTO != null);

            IQueryable<COMPRA_PARCELA> queryCompraParcela = _db.COMPRA_PARCELA.Where(cp => cp.DATA_PAGAMENTO != null);

            IQueryable<PAGAMENTO> queryPagamento = _db.PAGAMENTO.Where(p => p.DATA_PAGAMENTO != null);

            IQueryable<PAGAMENTO_TERCEIRO> queryPagamentoTerceiro = _db.PAGAMENTO_TERCEIRO;

            IQueryable<CAIXINHA_ITEM> queryCaixinhaItem = _db.CAIXINHA_ITEM
                .Where(ci => ci.VALIDACAO_FINANCEIRO && ci.VALIDACAO_OPERACIONAL);

            if (entrada_ossb != null)
            {
                queryContasReceber = queryContasReceber.Where(cr => cr.OSSB == entrada_ossb);
            }

            if(saida_ossb != null)
            {
                queryCompraParcela = queryCompraParcela.Where(cp => cp.COMPRA1.OSSB == saida_ossb);

                queryPagamento = queryPagamento.Where(p => p.OSSB == saida_ossb);

                queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.OSSB_SERVICO1.OSSB == saida_ossb);

                queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.OSSB == saida_ossb);
            }

            if(entrada_projeto != null)
            {
                queryContasReceber = queryContasReceber.Where(cr => cr.OSSB1.PROJETO == entrada_projeto);
            }

            if(saida_projeto != null)
            {
                queryCompraParcela = queryCompraParcela.Where(cp => cp.COMPRA1.PROJETO == saida_projeto);

                queryPagamento = queryPagamento.Where(p => p.PROJETO == saida_projeto);

                queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.OSSB_SERVICO1.OSSB1.PROJETO == saida_projeto);

                queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.OSSB1.PROJETO == saida_projeto);
            }

            if (de != null)
            {
                DateTime deDate = DateTime.Parse(de);

                queryContasReceber = queryContasReceber.Where(cr => cr.DATA_RECEBIMENTO >= deDate);

                queryCompraParcela = queryCompraParcela.Where(cp => cp.DATA_PAGAMENTO >= deDate);

                queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO >= deDate);

                queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.DATA_PAGAMENTO >= deDate);

                queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.DATA_QUITADO >= deDate);
            }

            if (ate != null)
            {
                DateTime ateDate = DateTime.Parse(ate);


                queryContasReceber = queryContasReceber.Where(cr => cr.DATA_RECEBIMENTO <= ateDate);

                queryCompraParcela = queryCompraParcela.Where(cp => cp.DATA_PAGAMENTO <= ateDate);

                queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO <= ateDate);

                queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.DATA_PAGAMENTO <= ateDate);

                queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.DATA_QUITADO <= ateDate);
            }

            switch (entrada_tipo)
            {
                case "V":
                    queryContasReceber = queryContasReceber.Where(cr => cr.OSSB1.TIPO == "P");

                    break;
                case "F":
                    queryContasReceber = queryContasReceber.Where(cr => cr.OSSB1.TIPO != "P");
                    break;
            }

            switch (saida_tipo)
            {
                case "V":

                    queryCompraParcela = queryCompraParcela.Where(cp => cp.COMPRA1.DESPESA1.TIPO == "V");

                    queryPagamento = queryPagamento.Where(p => p.DESPESA1.TIPO == "V");

                    queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.OSSB_SERVICO_TERCEIRO.DESPESA1.TIPO == "V");

                    queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.DESPESA1.TIPO == "V");
                    break;
                case "F":
                    queryCompraParcela = queryCompraParcela.Where(cp => cp.COMPRA1.DESPESA1.TIPO == "F");

                    queryPagamento = queryPagamento.Where(p => p.DESPESA1.TIPO == "F");

                    queryPagamentoTerceiro = queryPagamentoTerceiro.Where(pt => pt.OSSB_SERVICO_TERCEIRO.DESPESA1.TIPO == "F");

                    queryCaixinhaItem = queryCaixinhaItem.Where(ci => ci.DESPESA1.TIPO == "F");
                    break;
            }



            IEnumerable<RelatorioGeralViewModel> valores = null;


            var concat = (from cr in queryContasReceber
                          select new
                          {
                              DATA = (DateTime)cr.DATA_RECEBIMENTO,
                              ENTRADA = cr.VALOR_LIQUIDO,
                              SAIDA = 0m
                          }).Concat(
                from cp in queryCompraParcela
                select new
                {
                    DATA = (DateTime)cp.DATA_PAGAMENTO,
                    ENTRADA = 0m,
                    SAIDA = cp.VALOR
                }).Concat(
                from p in queryPagamento
                select new
                {
                    DATA = (DateTime)p.DATA_PAGAMENTO,
                    ENTRADA = 0m,
                    SAIDA = p.VALOR
                }).Concat(
                from pt in queryPagamentoTerceiro
                select new
                {
                    DATA = pt.DATA_PAGAMENTO,
                    ENTRADA = 0m,
                    SAIDA = pt.VALOR
                }).Concat(from ci in queryCaixinhaItem
                          select new
                          {
                              DATA = ci.DATA_QUITADO,
                              ENTRADA = 0m,
                              SAIDA = ci.VALOR
                          });




            switch (por)
            {
                case "D":
                    valores = await (from item in concat
                                     group item by DbFunctions.TruncateTime(item.DATA) into g
                                     select new RelatorioGeralViewModel()
                                     {
                                         DATA = g.Key.Value.Day + "/" + g.Key.Value.Month + "/" + g.Key.Value.Year,
                                         ENTRADA = g.Select(it => it.ENTRADA).Sum(),
                                         SAIDA = g.Select(it => it.SAIDA).Sum()
                                     }).ToArrayAsync();
                    break;
                case "M":
                    valores = await (from item in concat
                                     group item by new { MES = item.DATA.Month, ANO = item.DATA.Year } into g
                                     select new RelatorioGeralViewModel()
                                     {
                                         DATA = g.Key.MES + "/" + g.Key.ANO,
                                         ENTRADA = g.Select(it => it.ENTRADA).Sum(),
                                         SAIDA = g.Select(it => it.SAIDA).Sum()
                                     }).ToArrayAsync();
                    break;
                case "A":
                    valores = await (from item in concat
                                     group item by item.DATA.Year into g
                                     select new RelatorioGeralViewModel()
                                     {
                                         DATA = g.Key.ToString(),
                                         ENTRADA = g.Select(it => it.ENTRADA).Sum(),
                                         SAIDA = g.Select(it => it.SAIDA).Sum()
                                     }).ToArrayAsync();
                    break;
            }

            return View(valores);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}