using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;

namespace Atimo.Controllers
{
    public class ProjetoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        public ActionResult Index()
        {
            return View(_db
                .PROJETO.ToList());
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.EMPRESA = new SelectList(await _db.EMPRESA.ToArrayAsync(), "ID", "NOME");

            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(PROJETO projeto)
        {

            if (string.IsNullOrEmpty(projeto.DESCRICAO))
                ModelState.AddModelError("", "Informe uma descrição");



            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(projeto.DESCRICAO))
                    projeto.DESCRICAO = projeto.DESCRICAO.ToUpper();

                _db.PROJETO.Add(projeto);

                _db.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(projeto);
        }

        public async Task<ActionResult> Edit(int id = 0)
        {
            var projeto = await _db.PROJETO.FindAsync(id);

            if (projeto == null)
            {
                return HttpNotFound();
            }

            return View(projeto);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Edit(PROJETO projeto)
        {
            if (string.IsNullOrEmpty(projeto.DESCRICAO))
                ModelState.AddModelError("", "Informe uma descrição");


            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(projeto.DESCRICAO))
                    projeto.DESCRICAO = projeto.DESCRICAO.ToUpper();

                _db.Entry(projeto)
                    .State = EntityState.Modified;

               await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            return View(projeto);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}