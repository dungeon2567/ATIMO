using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class TarefaController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index()
        {
            if (Session.IsFuncionario())
            {
                var usuarioId = Session.UsuarioId();

                ViewBag.MEMBROS = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.FUNCIONARIO == 1)
                    .Select(p => new { ID = p.ID, NOME = p.NOME }).ToArrayAsync(), "ID", "NOME");

                return View(await _db.TAREFA_MEMBRO
                    .Where(tm => tm.MEMBRO == usuarioId)
                    .Join(_db.TAREFAs, tm => tm.TAREFA, t => t.ID, (tm, t) => t)
                    .Where(t => t.SITUACAO == "P")
                    .ToArrayAsync());
            }
            else
                return RedirectToAction("", "");
        }

        public ActionResult Create(int id = 0)
        {
            if (Session.IsFuncionario())
                return View(new TAREFA());
            else
                return RedirectToAction("", "");
        }

        //
        // POST: /OssbMaterial/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TAREFA tarefa)
        {
            if (Session.IsFuncionario())
            {

                if (string.IsNullOrEmpty(tarefa.DESCRICAO))
                    ModelState.AddModelError(string.Empty, "informe uma descrição!");


                if (ModelState.IsValid)
                {
                    tarefa.SITUACAO = "P";

                    _db.TAREFAs.Add(tarefa);

                    await _db.SaveChangesAsync();

                    var tarefaMembro = new TAREFA_MEMBRO()
                    {
                        MEMBRO = Session.UsuarioId(),
                        TAREFA = tarefa.ID,
                        TIPO = "D"
                    };

                    _db.TAREFA_MEMBRO.Add(tarefaMembro);

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return View(tarefa);
            }
            else
                return RedirectToAction("", "");
        }

        public async Task<JsonResult> MarcarComoConcluido(int id)
        {
            if (Session.IsFuncionario())
            {

                var t = await _db.TAREFAs.FirstOrDefaultAsync(v => v.ID == id);

                t.SITUACAO = "C";

                _db.Entry(t).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public async Task<JsonResult> MarcarComoCancelado(int id)
        {
            if (Session.IsFuncionario())
            {

                var t = await _db.TAREFAs.FirstOrDefaultAsync(v => v.ID == id);

                t.SITUACAO = "I";

                _db.Entry(t).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }


        public async Task<JsonResult> RemoverMembro(int membro, int tarefa)
        {
            if (Session.IsFuncionario())
            {
                var tarefaMembro = await _db.TAREFA_MEMBRO
                     .FirstOrDefaultAsync(tm => tm.MEMBRO == membro && tm.TAREFA == tarefa);

                _db.Entry(tarefaMembro)
                    .State = EntityState.Deleted;

                await _db.SaveChangesAsync();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public async Task<JsonResult> AdicionarMembro(int membro, int tarefa)
        {
            if (Session.IsFuncionario())
            {

                var tarefaMembro = new TAREFA_MEMBRO()
                {
                    TAREFA = tarefa,
                    MEMBRO = membro,
                    TIPO = "C"
                };

                _db.TAREFA_MEMBRO.Add(tarefaMembro);

                await _db.SaveChangesAsync();

                return Json("", JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

        public async Task<JsonResult> GetMembros(int id)
        {
            if (Session.IsFuncionario())
            {
                var membros = await _db.TAREFA_MEMBRO
                .Where(tm => tm.TAREFA == id)
                .Join(_db.PESSOA, tm => tm.MEMBRO, p => p.ID, (tm, p) => new { NOME = p.NOME, ID = p.ID, TIPO = tm.TIPO })

                .ToArrayAsync();

                return Json(membros, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }


        public async Task<JsonResult> GetSubtarefas(int id)
        {
            if (Session.IsFuncionario())
            {
                var subtarefas = await _db.SUBTAREFA
                .Where(st => st.TAREFA == id && st.SITUACAO == "P")
                .Select(st => new { ID = st.ID, DESCRICAO = st.DESCRICAO })
                .ToArrayAsync();

                return Json(subtarefas, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();

            base.Dispose(disposing);
        }
    }
}