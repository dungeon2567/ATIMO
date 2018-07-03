using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;

namespace Atimo.Controllers
{
    public class TerceiroServicoController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /TerceiroServico/

        public ActionResult Index(int id = 0)
        {
            var terceiroServico = _db.TERCEIRO_SERVICO
                .Include(t => t.AREA_MANUTENCAO1)
                .Include(t => t.PESSOA)
                .Include(t => t.SERVICO1)
                .Where(t => t.TERCEIRO == id);

            var viewModel = new TerceiroServicoViewModel {Lista = terceiroServico.ToList(), Terceiro = id};
            return View(viewModel);
        }

        //
        // GET: /TerceiroServico/Create

        public ActionResult Create(int id = 0)
        {
            ViewBag.AREA_MANUTENCAO = new SelectList(_db
                .AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO");
            var viewModel = new TerceiroServicoViewModel {Terceiro = id, Erro = "N"};

            return View(viewModel);
        }

        //
        // POST: /TerceiroServico/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TerceiroServicoViewModel viewModel)
        {
            if (VerificarTerceiroServico(viewModel, "N"))
            {
                viewModel.Erro = "S";
                ModelState.AddModelError(string.Empty, "Serviço já informado para o terceiro!");
            }

            if (ModelState.IsValid)
            {
                var terceiroServico = new TERCEIRO_SERVICO
                {
                    TERCEIRO = viewModel.Terceiro,
                    AREA_MANUTENCAO = viewModel.Area_Manutencao,
                    SERVICO = viewModel.Servico,
                    SITUACAO = viewModel.Situacao
                };

                _db.TERCEIRO_SERVICO.Add(terceiroServico);
                _db.SaveChanges();
                return RedirectToAction("Index", new { id = viewModel.Terceiro });
            }

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", viewModel.Area_Manutencao);
            ViewBag.SERVICO = new SelectList(_db.SERVICO, "ID", "DESCRICAO", viewModel.Servico);
            return View(viewModel);
        }

        //
        // GET: /TerceiroServico/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var terceiroServico = _db.TERCEIRO_SERVICO.Find(id);

            if (terceiroServico == null)
            {
                return HttpNotFound();
            }

            var viewModel = new TerceiroServicoViewModel
            {
                Id = terceiroServico.ID,
                Terceiro = terceiroServico.TERCEIRO,
                Area_Manutencao = terceiroServico.AREA_MANUTENCAO,
                Servico = terceiroServico.SERVICO,
                Situacao = terceiroServico.SITUACAO,
                Erro = "N"
            };

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", terceiroServico.AREA_MANUTENCAO);
            ViewBag.SERVICO = new SelectList(_db.SERVICO, "ID", "DESCRICAO", terceiroServico.SERVICO);
            return View(viewModel);
        }

        //
        // POST: /TerceiroServico/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TerceiroServicoViewModel viewModel)
        {
            if (VerificarTerceiroServico(viewModel, "E"))
            {
                viewModel.Erro = "S";
                ModelState.AddModelError(string.Empty, "Serviço já informado para o terceiro!");
            }

            if (ModelState.IsValid)
            {
                var terceiroServico = new TERCEIRO_SERVICO
                {
                    ID = viewModel.Id,
                    TERCEIRO = viewModel.Terceiro,
                    AREA_MANUTENCAO = viewModel.Area_Manutencao,
                    SERVICO = viewModel.Servico,
                    SITUACAO = viewModel.Situacao
                };

                _db.Entry(terceiroServico).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index", new { id = viewModel.Terceiro });
            }

            ViewBag.AREA_MANUTENCAO = new SelectList(_db.AREA_MANUTENCAOSet.Where(am => am.SITUACAO == "A"), "ID", "DESCRICAO", viewModel.Area_Manutencao);
            ViewBag.SERVICO = new SelectList(_db.SERVICO, "ID", "DESCRICAO", viewModel.Servico);
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult ListaServico(int id)
        {
            var servicos = _db.SERVICO.Where(s => s.SITUACAO == "A");
            return Json(new SelectList(servicos.ToArray(), "ID", "DESCRICAO"), JsonRequestBehavior.AllowGet);
        }

        public bool VerificarTerceiroServico(TerceiroServicoViewModel viewModel, string situacao)
        {
            var existe = situacao == "N" ? _db.TERCEIRO_SERVICO.Any(x => x.AREA_MANUTENCAO == viewModel.Area_Manutencao && x.SERVICO == viewModel.Servico && x.TERCEIRO == viewModel.Terceiro) : _db.TERCEIRO_SERVICO.Any(x => x.AREA_MANUTENCAO == viewModel.Area_Manutencao && x.SERVICO == viewModel.Servico && x.TERCEIRO == viewModel.Terceiro && x.ID != viewModel.Id);

            return existe;
        }
    }
}