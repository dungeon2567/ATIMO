using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
/*
namespace Atimo.Controllers
{
    public class ClienteProjetoController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /ClienteProjeto/

        public ActionResult Index(int id = 0)
        {
            var clienteProjeto = _db
                .CLIENTE_PROJETO.Include(c => c.PESSOA).Include(c => c.PROJETO1).Where(c => c.CLIENTE == id);
            var viewModel = new ClienteProjetoViewModel {Lista = clienteProjeto.ToList(), Cliente = id};

            return View(viewModel);
        }

        //
        // GET: /ClienteProjeto/Create

        public ActionResult Create(int id = 0)
        {
            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO");

            var viewModel = new ClienteProjetoViewModel {Cliente = id};

            return View(viewModel);
        }

        //
        // POST: /ClienteProjeto/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteProjetoViewModel viewModel)
        {
            if (VerificarClienteProjeto(viewModel, "N"))
                ModelState.AddModelError(string.Empty, "Projeto já informado!");

            if (ModelState.IsValid)
            {
                var clienteProjeto = new CLIENTE_PROJETO
                {
                    CLIENTE = viewModel.Cliente,
                    PROJETO = viewModel.Projeto,
                    SITUACAO = viewModel.Situacao
                };

                _db.CLIENTE_PROJETO.Add(clienteProjeto);
                _db.SaveChanges();
                return RedirectToAction("Index", new {id = viewModel.Cliente});
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", viewModel.Projeto);
            return View(viewModel);
        }

        //
        // GET: /ClienteProjeto/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var clienteProjeto = _db.CLIENTE_PROJETO.Find(id);

            if (clienteProjeto == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ClienteProjetoViewModel
            {
                Id = clienteProjeto.ID,
                Cliente = clienteProjeto.CLIENTE,
                Projeto = clienteProjeto.PROJETO,
                Situacao = clienteProjeto.SITUACAO
            };

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", clienteProjeto.PROJETO);
            return View(viewModel);
        }

        //
        // POST: /ClienteProjeto/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClienteProjetoViewModel viewModel)
        {
            if (VerificarClienteProjeto(viewModel, "E"))
                ModelState.AddModelError(string.Empty, "Projeto já informado!");

            if (ModelState.IsValid)
            {
                var clienteProjeto = new CLIENTE_PROJETO
                {
                    ID = viewModel.Id,
                    CLIENTE = viewModel.Cliente,
                    PROJETO = viewModel.Projeto,
                    SITUACAO = viewModel.Situacao
                };

                _db.Entry(clienteProjeto).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index", new {id = viewModel.Cliente});
            }

            ViewBag.PROJETO = new SelectList(_db.PROJETO, "ID", "DESCRICAO", viewModel.Projeto);
            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public bool VerificarClienteProjeto(ClienteProjetoViewModel viewModel, string situacao)
        {
            var existe = situacao == "N" ? _db.CLIENTE_PROJETO.Any(x => x.CLIENTE == viewModel.Cliente && x.PROJETO == viewModel.Projeto) : _db.CLIENTE_PROJETO.Any(x => x.CLIENTE == viewModel.Cliente && x.PROJETO == viewModel.Projeto && x.ID != viewModel.Id);

            return existe;
        }
    }
}
*/