using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    public class OperacionalController : FuncionarioController
    {
        readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            return View();
        }
    }
}
