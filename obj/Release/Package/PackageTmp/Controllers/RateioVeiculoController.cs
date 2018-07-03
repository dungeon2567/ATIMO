using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class RateioVeiculoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /RateioVeiculo/

        public ActionResult Index(int id = 0)
        {
            var rateio = _db.RATEIO
                .Include(r => r.DEPARTAMENTO1)
                .Include(r => r.EQUIPAMENTO1)
                .Include(r => r.FERRAMENTA1)
                .Include(r => r.PESSOA1)
                .Include(r => r.PROJETO1)
                .Include(r => r.VEICULO1)
                .Where(r => r.VEICULO == id);
            return View(new RateioIndexViewModel() { Id = id, Lista = rateio.ToList() });
        }

        //
        // GET: /RateioVeiculo/Create

        public ActionResult Create(int id = 0)
        {
            var rateio = new RATEIO {VEICULO = id};

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO");

            return View(rateio);
        }

        //
        // POST: /RateioVeiculo/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RATEIO rateio)
        {
            if (!VerificarProjeto(rateio, "N"))
                ModelState.AddModelError(string.Empty, "Unidade de negócio já vinculada ao veículo!"+rateio.VEICULO);

            if (ModelState.IsValid)
            {
                _db.RATEIO.Add(rateio);
                _db.SaveChanges();
                return RedirectToAction("Index/"+rateio.VEICULO, new {id = rateio.PESSOA});
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", rateio.PROJETO);
            return View(rateio);
        }

        //
        // GET: /RateioVeiculo/Edit/5

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
        // POST: /RateioVeiculo/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RATEIO rateio)
        {
            if (!VerificarProjeto(rateio, "E"))
                ModelState.AddModelError(string.Empty, "Unidade de negócio já vinculada ao veículo!");

            if (ModelState.IsValid)
            {
                _db.Entry(rateio).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index/"+rateio.VEICULO, new {id =  rateio.PESSOA});
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
            var existe = situacao == "N" ? _db.RATEIO.Any(x => x.VEICULO == rateio.VEICULO && x.PROJETO == rateio.PROJETO) : _db.RATEIO.Any(x => x.VEICULO == rateio.VEICULO && x.PROJETO == rateio.PROJETO && x.ID != rateio.ID);

            return existe;
        }
    }
}