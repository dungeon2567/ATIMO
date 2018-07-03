using ATIMO.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using ATIMO;

namespace Atimo.Controllers
{
    public class AreaGerencialController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Resultados(string de = null, string ate = null)
        {
            if (Session.UsuarioId() != 58)
                return HttpNotFound();

            DateTime deDate = de == null ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : DateTime.Parse(de);
            DateTime ateDate = ate == null ? DateTime.Today : DateTime.Parse(ate);

            if (deDate < new DateTime(2018, 5, 1))
            {
                deDate = new DateTime(2018, 5, 1);
            }

            ViewBag.DE = deDate.ToString("dd/MM/yyyy");
            ViewBag.ATE = ateDate.ToString("dd/MM/yyyy");


            IQueryable<CONTAS_RECEBER> queryReceita = _db.CONTAS_RECEBER
                .Where(cr => cr.DATA_RECEBIMENTO != null);

            IQueryable<PAGAMENTO> queryPagamento = _db.PAGAMENTO.Where(p => p.DATA_PAGAMENTO != null);


            queryReceita = queryReceita.Where(cr => cr.DATA_RECEBIMENTO >= deDate && cr.DATA_RECEBIMENTO <= ateDate);

            queryPagamento = queryPagamento.Where(p => p.DATA_PAGAMENTO >= deDate && p.DATA_PAGAMENTO <= ateDate);


            var query = _db
                .PESSOA
                .Where(p => p.ID == 72)
                .Select(pe => new
                {
                    PESSOA = pe.NOME,
                    VALOR = (queryReceita
                            .Where(r => r.OSSB1.PROJETO == 1)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .Sum() - queryPagamento
                            .Where(p => p.PROJETO == 1)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .Sum() - (queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .Sum() / 2)) * 0.05m
                })
                .Concat(_db.PESSOA.Where(p => p.ID == 83 || p.ID == 312 || p.ID == 74).Select(pe => new
                {
                    PESSOA = pe.NOME,
                    VALOR = (queryReceita
                               .Where(r => r.OSSB1.PROJETO == 1 || r.OSSB1.PROJETO == 2)
                               .Select(r => r.VALOR_LIQUIDO)
                               .DefaultIfEmpty()
                               .Sum() - queryPagamento
                               .Where(p => p.PROJETO == 1 || p.PROJETO == 2)
                               .Select(p => p.VALOR)
                               .DefaultIfEmpty()
                               .Sum() - (queryPagamento
                               .Where(p => p.PROJETO == 4)
                               .Select(p => p.VALOR)
                               .DefaultIfEmpty()
                               .Sum())) * 0.015m
                }))
                           .Concat(_db.PESSOA.Where(p => p.ID == 523).Select(pe => new
                           {
                               PESSOA = pe.NOME + " (MANUTENÇÃO)",
                               VALOR = (queryReceita
                               .Where(r => r.OSSB1.PROJETO == 1)
                               .Select(r => r.VALOR_LIQUIDO)
                               .DefaultIfEmpty()
                               .Sum() - queryPagamento
                               .Where(p => p.PROJETO == 1)
                               .Select(p => p.VALOR)
                               .DefaultIfEmpty()
                               .Sum() - (queryPagamento
                               .Where(p => p.PROJETO == 4)
                               .Select(p => p.VALOR)
                               .DefaultIfEmpty()
                               .Sum() / 2)) * 0.015m
                           }))
                           .Concat(_db.PESSOA.Where(p => p.ID == 523).Select(pe => new
                           {
                               PESSOA = pe.NOME + " (OBRAS)",
                               VALOR = (queryReceita
            .Where(r => r.OSSB1.PROJETO == 2)
            .Select(r => r.VALOR_LIQUIDO)
            .DefaultIfEmpty()
            .Sum() - queryPagamento
            .Where(p => p.PROJETO == 2)
            .Select(p => p.VALOR)
            .DefaultIfEmpty()
            .Sum() - (queryPagamento
            .Where(p => p.PROJETO == 4)
            .Select(p => p.VALOR)
            .DefaultIfEmpty()
            .Sum() / 2)) * 0.015m
                           }))
                .Concat(_db.PESSOA.Where(p => p.ID == 136).Select(pe => new
                {
                    PESSOA = pe.NOME,
                    VALOR = (queryReceita
                            .Where(r => r.OSSB1.PROJETO == 2)
                            .Select(r => r.VALOR_LIQUIDO)
                            .DefaultIfEmpty()
                            .Sum() - queryPagamento
                            .Where(p => p.PROJETO == 2)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .Sum() - (queryPagamento
                            .Where(p => p.PROJETO == 4)
                            .Select(p => p.VALOR)
                            .DefaultIfEmpty()
                            .Sum() / 2)) * 0.015m
                }));


            return View(await query.ToDictionaryAsync(kv => kv.PESSOA, kv => kv.VALOR));

        }
    }
}
