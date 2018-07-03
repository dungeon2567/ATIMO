using ATIMO.Models;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using ATIMO;

namespace Atimo.Controllers
{
    public class DashboardController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index()
        {

                DateTime startingDate = DateTime.Now.Date.AddMonths(-12);

                startingDate = new DateTime(startingDate.Year, startingDate.Month, 1);


                var statusOs = await _db
                    .OSSB
                    .Where(os => os.TIPO != "P")
                    .GroupBy(o => o.SITUACAO, o => o, (key, o) => new { SITUACAO = key, COUNT = o.Count() })
                    .Where(g => g.COUNT > 0)
                    .ToDictionaryAsync(o => TextoSituacao(o.SITUACAO), o => o.COUNT);

                var statusOsPreventiva = await _db
                    .OSSB
                    .Where(os => os.TIPO == "P")
                    .GroupBy(o => o.SITUACAO, o => o, (key, o) => new { SITUACAO = key, COUNT = o.Count() })
                    .Where(g => g.COUNT > 0)
                    .ToDictionaryAsync(o => TextoSituacao(o.SITUACAO), o => o.COUNT);

              

                return View(new DashboardAdminViewModel() { StatusOs = statusOs, StatusOsPreventiva = statusOsPreventiva });
    
        }

        private static string TextoSituacao(string t)
        {
            switch (t)
            {
                case "I":
                    return "VISITA INICIAL";
                case "K":
                    return "COBRANÇA";
                case "O":
                    return "ORÇAR";
                case "R":
                    return "ORÇAMENTO";
                case "E":
                    return "EXECUTANDO";
                case "C":
                    return "CANCELADA";
                case "F":
                    return "FINALIZADA";
                case "P":
                    return "PARCELAMENTO";
                default:
                    throw new NotSupportedException();
            }
        }

        private static string TextoTipo(string t)
        {
            switch (t)
            {
                case "P":
                    return "PREVENTIVA";
                case "C":
                    return "CORRETIVA";
                case "O":
                    return "ACOMPANHAMENTO";
                case "E":
                    return "EMERGENCIAL";
                case "X":
                    return "EXTRA CONTRATUAL";
                case "G":
                    return "GARANTIA";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
