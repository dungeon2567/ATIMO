using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Atimo.Controllers
{
    public class RelatorioMensalController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Search()
        {
            ViewBag.CLIENTES = await _db.PESSOA.Where(p => p.CLIENTE == 1)
                .OrderBy(p => p.RAZAO)
                .ThenBy(p => p.NOME)
                .Select(p => new { id = p.ID.ToString(), nome = p.RAZAO + " (" + p.NOME + ")" })
              .ToArrayAsync();

            return View();
        }



        public async Task<ActionResult> Index(Int32[] clientes = null)
        {

            var query = _db.OSSB
                .Include(o => o.PESSOA)
                .Include(o => o.LOJA1)
                .Include(o => o.OSSB_TECNICO)
                .Include(o => o.OSSB_TECNICO.Select(ot => ot.TECNICO1))
                .Include(o => o.OSSB_SERVICO)
                .Include(o => o.PAGAMENTO)
                .Include(o => o.OSSB_CHECK_LIST)
                .Where(o => o.SITUACAO == "E");

            if (clientes != null)
            {
                query = query.Where(o => clientes.Contains((Int32)o.CLIENTE));
            }

            query = query.Where(o => o.OSSB_CHECK_LIST.Any());

            var ossbPreventiva = await query.OrderBy(o => o.OSSB_CHECK_LIST.FirstOrDefault().AGENDADO.Month)
                   .ToArrayAsync();

            var fs = new MemoryStream();

            var tw = new StreamWriter(fs, Encoding.UTF8);

            tw.WriteLine("Tipo;Cliente;Local;Prestador;Custo;Venda;Resultado;Mês;O.S;Status");

            int index = 1;

            foreach (var os in ossbPreventiva)
            {



                index += 1;

                String tecnico = "";



                if (os.OSSB_TECNICO.Any())
                {
                    tecnico = os.OSSB_TECNICO
                        .First()
                        .TECNICO1
                        .NOME_COMPLETO;
                }


                decimal custo = os.PAGAMENTO
                    .Select(p => p.VALOR)
                    .DefaultIfEmpty()
                    .Sum();


                decimal venda = os.OSSB_SERVICO
                    .Select(oss => oss.VALOR_MO * oss.QUANTIDADE)
                    .DefaultIfEmpty()
                    .Sum();


                /* this is slow */




                tw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                    os.TEXTO_TIPO,
                    os.PESSOA.NOME_COMPLETO,
                    os.LOJA != null ? os.LOJA1.APELIDO : "",
                    tecnico,
                    custo.ToString("C"),
                    venda.ToString("C"),
                    (venda - custo).ToString("C"),
               CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(os.OSSB_CHECK_LIST.First().AGENDADO.Month)),
               os.ID,
               os.TEXTO_SITUACAO
                    );


            }

            tw.Close();


            Response.AppendHeader("Content-Disposition", "filename=preventiva.csv;");

            return new FileContentResult(fs.ToArray(), MimeMapping.GetMimeMapping("preventiva.csv"));

        }

        public async Task<JsonResult> GetClientes(String query)
        {

            var pessoas = await ((from p in _db.PESSOA
                                  where p.SITUACAO == "A" && p.CLIENTE == 1 && (p.RAZAO.StartsWith(query) || p.NOME.StartsWith(query))
                                  select p).ToArrayAsync());

            return Json(new { status = 0, pessoas = pessoas.Select(p => new { id = p.ID, text = p.NOME_COMPLETO }) }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}