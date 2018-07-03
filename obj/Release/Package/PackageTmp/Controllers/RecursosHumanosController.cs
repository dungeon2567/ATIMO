using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using Atimo.Controllers;
using System;
using System.Web;
using System.Web.Helpers;

namespace ATIMO.Controllers
{
    public class RecursosHumanosController : FuncionarioController
    {

        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            if (Session.IsFuncionario())
            {
                return View(_db.PESSOA
                    .Include(p => p.ESPECIALIDADE_1)
                    .Include(p => p.ESPECIALIDADE_2)
                    .Include(p => p.ESPECIALIDADE_3)
                    .Where(pp => pp.SITUACAO == "A" && pp.FUNCIONARIO == 1));
            }
            else
                return RedirectToAction("", "");
        }

        public async Task<ActionResult> Edit(int id = 0)
        {
            if (Session.IsFuncionario())
            {
                PESSOA p = _db.PESSOA.FirstOrDefault(pe => pe.ID == id);

                if (p == null)
                    return HttpNotFound();


                ViewBag.PROJETO =
                    new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", p.PROJETO);

                ViewBag.TIPO_ACESSO =
                        new SelectList(await _db.FUNCIONARIO_TIPO_ACESSO.ToArrayAsync(), "ID", "DESCRICAO", p.TIPO_ACESSO);


                var especialidades = await _db.ESPECIALIDADE
                    .Where(ep => ep.SITUACAO == "A")
                    .OrderBy(ep => ep.DESCRICAO)
                    .ToArrayAsync();

                ViewBag.ESPECIALIDADE1 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE1);

                ViewBag.ESPECIALIDADE2 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE2);

                ViewBag.ESPECIALIDADE3 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE3);

                return View(p);
            }
            else
                return RedirectToAction("", "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PESSOA p)
        {

            if (Session.IsFuncionario())
            {
                if (ModelState.IsValid)
                {
                    _db.Configuration.ValidateOnSaveEnabled = false;

                    var entry = _db.Entry(p);

                    _db.PESSOA.Attach(p);

                    entry.Property(pp => pp.PROJETO).IsModified = true;
                    entry.Property(pp => pp.SALARIO).IsModified = true;

                    entry.Property(pp => pp.VALE_TRANSPORTE).IsModified = true;
                    entry.Property(pp => pp.VALE_ALIMENTACAO).IsModified = true;

                    entry.Property(pp => pp.ESPECIALIDADE1).IsModified = true;
                    entry.Property(pp => pp.ESPECIALIDADE2).IsModified = true;
                    entry.Property(pp => pp.ESPECIALIDADE3).IsModified = true;

                    entry.Property(pp => pp.TIPO_ACESSO).IsModified = true;

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", "RecursosHumanos");
                }

                ViewBag.PROJETO =
                    new SelectList(await _db.PROJETO.ToArrayAsync(), "ID", "DESCRICAO", p.PROJETO);


                var especialidades = await _db.ESPECIALIDADE.Where(ep => ep.SITUACAO == "A").ToArrayAsync();

                ViewBag.ESPECIALIDADE1 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE1);

                ViewBag.ESPECIALIDADE2 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE2);

                ViewBag.ESPECIALIDADE3 = new SelectList(especialidades, "ID", "DESCRICAO", p.ESPECIALIDADE3);

                ViewBag.TIPO_ACESSO =
        new SelectList(await _db.FUNCIONARIO_TIPO_ACESSO.ToArrayAsync(), "ID", "DESCRICAO", p.TIPO_ACESSO);

                return View(p);
            }
            else
                return RedirectToAction("", "");
        }


        [HttpGet]
        public async Task<FileContentResult> VisualizarFoto(Int32 id, Int32? width = null, Int32? height = null)
        {

            var item = await _db
                .PESSOA
                .Include(p => p.IMAGEM1)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (item != null && item.IMAGEM1 != null)
            {

                Response.AddHeader("Content-Disposition", "inline; filename=" + item.IMAGEM1.NOME);

                byte[] buffer = item.IMAGEM1.BUFFER;

                if (width != null && height != null && width < 1000 && height < 1000)
                {
                    WebImage img = new WebImage(item.IMAGEM1.BUFFER);

                    img.Resize(width.Value, height.Value, false, false);

                    buffer = img.GetBytes(System.IO.Path.GetExtension(item.IMAGEM1.NOME).Replace(".", ""));

                }

                return File(buffer, MimeMapping.GetMimeMapping(item.IMAGEM1.NOME));
            }

            return null;

        }

        [HttpPost]
        public async Task<ActionResult> AnexarFoto(Int32 id, HttpPostedFileBase file)
        {

            var item = await _db
                .PESSOA
                .Include(p => p.IMAGEM1)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (file != null && item != null)
            {
                byte[] buffer = new byte[file.ContentLength];

                file.InputStream.Read(buffer, 0, buffer.Length);

                if (item.IMAGEM1 == null)
                {
                    item.IMAGEM1 = new ANEXO() { NOME = file.FileName, BUFFER = buffer };
                }
                else
                {
                    item.IMAGEM1.NOME = file.FileName;
                    item.IMAGEM1.BUFFER = buffer;
                }

                await _db.SaveChangesAsync();

            }

            return Redirect(Request.UrlReferrer.ToString());
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
