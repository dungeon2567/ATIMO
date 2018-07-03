using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using ATIMO;
using System;
using System.Web;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.IO;

namespace Atimo.Controllers
{
    public class FuncionarioDocumentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public async Task<ActionResult> Index(int id = 0)
        {
            var query = from d in _db.FUNCIONARIO_DOCUMENTO
                        where d.FUNCIONARIO == id
                        orderby d.DESCRICAO
                        group d by d.CATEGORIA into g
                        select g;

            return View(new FuncionarioDocumentoViewModel() { FUNCIONARIO = id, DOCUMENTOS = await query.ToDictionaryAsync(fd => fd.Key, fd => fd as IEnumerable<FUNCIONARIO_DOCUMENTO>) });

        }


        public async Task<ActionResult> Delete(Int32 id)
        {
            var doc = await _db.FUNCIONARIO_DOCUMENTO
                .FirstOrDefaultAsync(dc => dc.ID == id);

            if (doc == null)
                return HttpNotFound();


            _db.Entry(doc)
                .State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }

        public async Task<ActionResult> Download(Int32 id)
        {

            var item = await _db
                .FUNCIONARIO_DOCUMENTO
                .Include(dc => dc.ANEXO1)
                .FirstOrDefaultAsync(dc => dc.ID == id);



            if (item != null && item.ANEXO1 != null)
            {
                return File(item.ANEXO1.BUFFER, MimeMapping.GetMimeMapping(item.ANEXO1.NOME), item.ANEXO1.NOME);
            }
            else
                return HttpNotFound();
        }

        [HttpPost]
        public async Task<ActionResult> FileUpload(Int32 funcionario, String categoria, IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                if (file != null)
                {
                    String filename = Path.GetFileName(file.FileName);

                    byte[] data = new byte[file.InputStream.Length];

                    await file.InputStream.ReadAsync(data, 0, (int)file.InputStream.Length);

                    _db.FUNCIONARIO_DOCUMENTO.Add(new FUNCIONARIO_DOCUMENTO() { DATA_CRIACAO = DateTime.Today, CATEGORIA = categoria, FUNCIONARIO = funcionario, DESCRICAO = file.FileName, ANEXO1 = new ANEXO() { BUFFER = data, NOME = file.FileName } });
                }
            }

            await _db.SaveChangesAsync();

            return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}