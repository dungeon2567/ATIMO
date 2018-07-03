using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class CopiarServicosController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Copiar(int? de = null, int? para = null)
        {
            if (de == null)
                return HttpNotFound();

            if (para == null)
                return HttpNotFound();

            _db.COPIAR_SERVICOS_OSSB(de, para);


            return RedirectToAction("Index", "OssbServico", new { id = para });

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
