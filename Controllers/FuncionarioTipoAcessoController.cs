using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class FuncionarioTipoAcessoController : FuncionarioController
    {

        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {

            return View(_db.FUNCIONARIO_TIPO_ACESSO.ToArray());

        }

        public ActionResult Create()
        {

            return View(new FUNCIONARIO_TIPO_ACESSO());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FUNCIONARIO_TIPO_ACESSO fta)
        {
            if (Session.IsFuncionario())
            {
                if (ModelState.IsValid)
                {

                    _db.FUNCIONARIO_TIPO_ACESSO.Add(fta);

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "FuncionarioTipoAcesso");
                }

                return View(fta);
            }
            else
                return RedirectToAction("", "");
        }



        public ActionResult Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                FUNCIONARIO_TIPO_ACESSO fta = _db.FUNCIONARIO_TIPO_ACESSO.Where(ffttaa => ffttaa.ID == id)
                    .FirstOrDefault();

                if (fta == null)
                    return HttpNotFound();

                return View(fta);
            }
            else
                return RedirectToAction("", "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FUNCIONARIO_TIPO_ACESSO fta)
        {
            if (Session.IsFuncionario())
            {
                if (ModelState.IsValid)
                {

                    _db.Entry(fta).State = EntityState.Modified;

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "FuncionarioTipoAcesso");
                }

                return View(fta);
            }
            else
                return RedirectToAction("", "");
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
