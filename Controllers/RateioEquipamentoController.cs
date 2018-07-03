using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class RateioEquipamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /RateioEquipamento/

        public ActionResult Index(int id = 0)
        {
            var rateio = _db.RATEIO.Include(r => r.DEPARTAMENTO1).Include(r => r.EQUIPAMENTO1).Include(r => r.FERRAMENTA1).Include(r => r.PESSOA1).Include(r => r.PROJETO1).Include(r => r.VEICULO1).Where(r => r.EQUIPAMENTO == id);

            return View(new RateioIndexViewModel() { Id = id, Lista = rateio.ToList() });
        }

        //
        // GET: /RateioEquipamento/Create

        public ActionResult Create(int id = 0)
        {
            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO");

            var rateio = new RATEIO {EQUIPAMENTO = id};

            return View(rateio);
        }

        //
        // POST: /RateioEquipamento/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RATEIO rateio)
        {
            if (!VerificarProjeto(rateio, "N"))
                ModelState.AddModelError(string.Empty, "Unidade de negócio já informada para este equipamento!"+ rateio.EQUIPAMENTO);

            if (ModelState.IsValid)
            {
                _db.RATEIO.Add(rateio);
                _db.SaveChanges();
                return RedirectToAction("Index/"+ rateio.EQUIPAMENTO, new {id = rateio.PESSOA});
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            
            return View(rateio);
        }

        //
        // GET: /RateioEquipamento/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var rateio = _db.RATEIO.Find(id);

            if (rateio == null)
            {
                return HttpNotFound();
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);

            return View(rateio);
        }

        //
        // POST: /RateioEquipamento/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RATEIO rateio)
        {
            if (!VerificarProjeto(rateio, "E"))
                ModelState.AddModelError(string.Empty, "Unidade de negócio já informada para este equipamento!");

            if (ModelState.IsValid)
            {
                _db.Entry(rateio).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index/"+rateio.EQUIPAMENTO, new {id = rateio.PESSOA});
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            
            return View(rateio);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();

            base.Dispose(disposing);
        }

        private bool VerificarProjeto(RATEIO rateio, string situacao)
        {
            var existe = situacao == "N" ?
                _db
                .RATEIO
                .Any(x => x.EQUIPAMENTO == rateio.EQUIPAMENTO && x.PROJETO == rateio.PROJETO) :
                _db
                .RATEIO
                .Any(x => x.EQUIPAMENTO == rateio.EQUIPAMENTO && x.PROJETO == rateio.PROJETO && x.ID != rateio.ID);

            return existe;
        }
    }
}