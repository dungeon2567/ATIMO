using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class ContratoAreaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        /*
        //
        // GET: /ContratoArea/

        public ActionResult Index(int id = 0)
        {
            var contratoAreaManutencao = _db
                .CONTRATO_AREA_MANUTENCAO
                .Include(c => c.AREA_MANUTENCAO1)
                .Include(c => c.CONTRATO1)
                .Where(c => c.CONTRATO == id);

            var cliente = _db.CONTRATO.FirstOrDefault(c => c.ID == id);

            var viewModel = new ContratoAreaManutencaoViewModel {Lista = contratoAreaManutencao.ToList(), Contrato = id};

            if (cliente != null)
                viewModel.Cliente = cliente.LOJA;

            return View(viewModel);
        }

        //
        // GET: /ContratoArea/Create

        public ActionResult Create(int id = 0)
        {
            var cliente = _db.CONTRATO.FirstOrDefault(c => c.ID == id);

            ViewBag.AREA_MANUTENCAO = new SelectList(_db
                .AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO");

            var viewModel = new ContratoAreaManutencaoViewModel {Contrato = id};

            if (cliente != null)
                viewModel.Cliente = cliente.CLIENTE;

            return View(viewModel);
        }

        //
        // POST: /ContratoArea/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContratoAreaManutencaoViewModel viewModel)
        {
            if (VerificarContratoArea(viewModel, "N"))
                ModelState.AddModelError(string.Empty, "Area de manutenção já informada!");

            if (ModelState.IsValid)
            {
                var contratoArea = new CONTRATO_AREA_MANUTENCAO
                {
                    CONTRATO = viewModel.Contrato,
                    AREA_MANUTENCAO = viewModel.Area_Manutencao,
                    SITUACAO = viewModel.Situacao
                };

                _db.CONTRATO_AREA_MANUTENCAO.Add(contratoArea);
                _db.SaveChanges();
                return RedirectToAction("Index", new {id = viewModel.Contrato});
            }

            var cliente = _db.CONTRATO.FirstOrDefault(c => c.ID == viewModel.Contrato);

            if (cliente != null)
                viewModel.Cliente = cliente.CLIENTE;

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", viewModel.Area_Manutencao);
            return View(viewModel);
        }

        //
        // GET: /ContratoArea/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var contratoAreaManutencao = _db.CONTRATO_AREA_MANUTENCAO.Find(id);

            if (contratoAreaManutencao == null)
            {
                return HttpNotFound();
            }

            var cliente = _db.CONTRATO.FirstOrDefault(c => c.ID == contratoAreaManutencao.CONTRATO);
            var viewModel = new ContratoAreaManutencaoViewModel
            {
                Id = contratoAreaManutencao.ID,
                Contrato = contratoAreaManutencao.CONTRATO,
                Area_Manutencao = contratoAreaManutencao.AREA_MANUTENCAO,
                Situacao = contratoAreaManutencao.SITUACAO
            };

            if (cliente != null)
                viewModel.Cliente = cliente.CLIENTE;

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", contratoAreaManutencao.AREA_MANUTENCAO);
            return View(viewModel);
        }

        //
        // POST: /ContratoArea/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContratoAreaManutencaoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var contratoArea = new CONTRATO_AREA_MANUTENCAO
                {
                    ID = viewModel.Id,
                    CONTRATO = viewModel.Contrato,
                    AREA_MANUTENCAO = viewModel.Area_Manutencao,
                    SITUACAO = viewModel.Situacao
                };

                _db.Entry(contratoArea).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index", new { id = viewModel.Contrato });
            }

            var cliente = _db.CONTRATO.FirstOrDefault(c => c.ID == viewModel.Contrato);

            if (cliente != null)
                viewModel.Cliente = cliente.CLIENTE;

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", viewModel.Area_Manutencao);
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public bool VerificarContratoArea(ContratoAreaManutencaoViewModel viewModel, string situacao)
        {
            var existe = situacao == "N" ? _db.CONTRATO_AREA_MANUTENCAO.Any(x => x.CONTRATO == viewModel.Contrato && x.AREA_MANUTENCAO == viewModel.Area_Manutencao) : _db.CONTRATO_AREA_MANUTENCAO.Any(x => x.CONTRATO == viewModel.Contrato && x.AREA_MANUTENCAO == viewModel.Area_Manutencao && x.ID == viewModel.Id);

            return existe;
        }
        */
    }
    
}