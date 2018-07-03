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
    public class AreaDoClienteController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> OrdensDeServico()
        {
            var user = Session.Usuario();

            if (user != null && user.CLIENTE == 1)
            {

                var queryObject = _db.OSSB
                    .Include(os => os.PESSOA)
                    .Where(o => o.CLIENTE == user.ID);


                return View(await queryObject.ToArrayAsync());
            }

            return RedirectToAction("", "");
        }

        private static string TextoSituacao(string t)
        {
            switch (t)
            {
                case "I":
                    return "VISITA INICIAL";
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
                case "K":
                    return "COBRANÇA";
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

        public async Task<ActionResult> Dashboard()
        {
            var user = Session.Usuario();

            if (user != null && user.CLIENTE == 1)
            {
                var statusOs = await _db
                      .OSSB
                      .Where(o => o.CLIENTE == user.ID)
                      .GroupBy(o => o.SITUACAO, o => o, (key, o) => new { SITUACAO = key, COUNT = o.Count() })
                      .Where(g => g.COUNT > 0)
                      .ToDictionaryAsync(o => TextoSituacao(o.SITUACAO), o => o.COUNT);

                var tipoOs = await _db
                    .OSSB
                    .Where(o => o.CLIENTE == user.ID)
                    .GroupBy(o => o.TIPO, o => o, (key, o) => new { TIPO = key, COUNT = o.Count() })
                    .Where(g => g.COUNT > 0)
                    .ToDictionaryAsync(o => TextoTipo(o.TIPO), o => o.COUNT);

                return View(new DashboardViewModel() { StatusOs = statusOs, TipoOs = tipoOs });
            }

            return RedirectToAction("", "");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
