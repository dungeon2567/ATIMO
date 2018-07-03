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
    public class FuncionarioCaixinhaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index()
        {
            var query = _db.PESSOA.Include(p => p.CAIXINHA)
                .Include(p => p.CAIXINHA_ITEM)
                .Where(p => p.ID == Session.UsuarioId())
                .FirstOrDefaultAsync();

            return View(await query);
        }
        public async Task<ActionResult> Create()
        {
            if (Session.IsFuncionario())
            {
                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO");

                return View(new CAIXINHA() { DATA_ENTREGA = DateTime.Today });

            }
            else
            {
                return RedirectToAction("", "");
            }
        }

        [HttpPost]
        public async Task<FileResult> BaixarArquivo(Int32 id)
        {
            if (Session.IsFuncionario())
            {
                var item = await _db
                    .CAIXINHA_ITEM
                    .Include(ci => ci.ANEXO1)
                    .FirstOrDefaultAsync(ci => ci.ID == id);

                if (item != null && item.ANEXO1 != null)
                {
                    return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME), item.ANEXO1.NOME);
                }

                return null;
            }
            else
                return null;
        }

        public async Task<JsonResult> Deletar(Int32 id)
        {
            if (Session.IsFuncionario())
            {
                var cr = await _db.CAIXINHA_ITEM
                    .Include(pt => pt.ANEXO1)
                    .FirstOrDefaultAsync(ci => ci.ID == id);

                if (cr == null)
                    return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);

                if (cr.ANEXO1 != null)
                {
                    _db.Entry(cr.ANEXO1)
                        .State = EntityState.Deleted;
                }


                _db.Entry(cr)
                    .State = EntityState.Deleted;

                await _db.SaveChangesAsync();

                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CAIXINHA caixinha)
        {
            if (Session.IsFuncionario())
            {
                if (caixinha.PESSOA == 0)
                    ModelState.AddModelError("", "Informe uma pessoa!");

                if (ModelState.IsValid)
                {


                    _db.CAIXINHA.Add(caixinha);

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "Caixinha");
                }

                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO");

                return View(caixinha);
            }
            else
            {
                return RedirectToAction("", "");
            }
        }

        public async Task<ActionResult> Quitar(int? despesa = null, int? pessoa = null)
        {
            if (Session.IsFuncionario())
            {

                    ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", despesa);




                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO", pessoa);

                return View(new CAIXINHA_ITEM() { DATA_QUITADO = DateTime.Today });

            }
            else
            {
                return RedirectToAction("", "");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Quitar(CAIXINHA_ITEM item)
        {
            if (Session.IsFuncionario())
            {
                if (item.PESSOA == 0)
                    ModelState.AddModelError("", "Informe uma pessoa!");

                if (item.DESPESA == 0)
                    ModelState.AddModelError("", "Informe uma despesa!");

                if (ModelState.IsValid)
                {


                    HttpPostedFileBase file = Request.Files[0];

                    if (file.ContentLength > 0)
                    {

                        byte[] buffer = new byte[file.ContentLength];

                        file.InputStream.Read(buffer, 0, buffer.Length);

                        ANEXO anexo = new ANEXO()
                        {
                            NOME = file.FileName,
                            BUFFER = buffer
                        };

                        item.ANEXO1 = anexo;
                    }


                    if (!String.IsNullOrEmpty(item.DESCRICAO))
                    {
                        item.DESCRICAO = item.DESCRICAO.ToUpper();
                    }

                    _db.CAIXINHA_ITEM.Add(item);

                    await _db.SaveChangesAsync();

                    TempData["MensagemSucesso"] = "Caixinha devolvida com o valor de " + item.VALOR.ToString("C") + ".";

                    return RedirectToAction("Quitar", "Caixinha", new { pessoa = item.PESSOA, despesa = item.DESPESA });
                }

                ViewBag.DESPESA = new SelectList(await _db.DESPESA.Include(dp => dp.DESPESA_CLASSE).OrderBy(dp => dp.DESPESA_CLASSE.DESCRICAO).ThenBy(dp => dp.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", "CLASSE_DESCRICAO", item.DESPESA);


                ViewBag.PESSOA = new SelectList(await _db
                    .PESSOA
                    .Where(p => p.SITUACAO == "A" && (p.TERCEIRO == 1 || p.FUNCIONARIO == 1))
                    .OrderBy(p => p.RAZAO)
                    .ToArrayAsync(), "ID", "RAZAO", item.PESSOA);

                return View(item);
            }
            else
            {
                return RedirectToAction("", "");
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}