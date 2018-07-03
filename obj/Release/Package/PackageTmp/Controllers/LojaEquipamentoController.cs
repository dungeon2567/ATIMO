using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using ATIMO;
using System.Data.Entity.Infrastructure;
using System.Web;

namespace Atimo.Controllers
{
    public class LojaEquipamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //
        // GET: /Loja/

        public async Task<ActionResult> Expired()
        {
            var umMes = DateTime.Today.AddMonths(1);


            var equipamentos = await _db.LOJA_EQUIPAMENTO
                .Where(le => le.DATA_VENCIMENTO != null && le.DATA_VENCIMENTO <= umMes)
                .Include(le => le.MODELO1)
                .Include(le => le.TIPO1)
                .Include(le => le.LOJA1)
                .Include(le => le.LOJA1)
                .OrderBy(le => le.DATA_VENCIMENTO)
                .ToArrayAsync();

            return View(equipamentos);

        }

        public async Task<ActionResult> Index(int id = 0)
        {
        
                var equipamentos = await _db.LOJA_EQUIPAMENTO.Where(le => le.LOJA == id)
                    .Include(le => le.MODELO1)
                    .Include(le => le.TIPO1)
                    .ToArrayAsync();

                return View(new LojaEquipamentoViewModel() { EQUIPAMENTOS = equipamentos, LOJA = id });

        }


        public async Task<ActionResult> Search(int? cliente = null, int page = 1)
        {

            var equipamentos = _db.LOJA_EQUIPAMENTO
                .Include(le => le.LOJA1)
                .Include(le => le.LOJA1.CLIENTE1)
                    .Include(le => le.MODELO1)
                    .Include(le => le.TIPO1);

            if (cliente != null)
            {
                equipamentos = from le in equipamentos
                               where le.LOJA1.CLIENTE == cliente
                               select le;

                equipamentos = from le in equipamentos
                               orderby le.LOJA1.APELIDO
                               select le;
            }
            else
            {
                equipamentos = from le in equipamentos
                               orderby le.LOJA1.CLIENTE1.NOME, le.LOJA1.APELIDO
                               select le;
            }

            ViewBag.PAGE = page;

            ViewBag.PAGE_COUNT = ((await equipamentos.CountAsync() - 1) / 10) + 1;

            ViewBag.CLIENTE = new SelectList(await _db.PESSOA.Where(p => p.SITUACAO == "A" && p.CLIENTE == 1)
                .Where(p => p.SITUACAO == "A").OrderBy(p => p.NOME).ToArrayAsync(), "ID", "NOME ", cliente);


            return View(equipamentos
                .Skip((page - 1) * 10)
                .Take(10));
        }



        //
        // GET: /Loja/Create

        public async Task<ActionResult> Create(int id = 0)
        {
            ViewBag.TIPO = new SelectList(await _db.TIPO
                .Where(t => t.TIPO1 == "E" && t.SITUACAO == "A")
                .OrderBy(te => te.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO");

            ViewBag.MODELO = new SelectList(await _db.MODELO
                .Where(m => m.SITUACAO == "A")
                .OrderBy(m => m.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO");



            return View(new LOJA_EQUIPAMENTO() { LOJA = id });
        }

        //

        // POST: /Loja/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LOJA_EQUIPAMENTO le)
        {
      
                if (ModelState.IsValid)
                {
                    _db.LOJA_EQUIPAMENTO.Add(le);

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index", new { id = le.LOJA });
                }

                ViewBag.TIPO = new SelectList(await _db.TIPO
                    .Where(t => t.TIPO1 == "E" && t.SITUACAO == "A")
                    .OrderBy(te => te.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.TIPO);

                ViewBag.MODELO = new SelectList(await _db.MODELO
                    .Where(m => m.SITUACAO == "A")
                    .OrderBy(m => m.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.MODELO);

                return View(le);

        }

        public async Task<ActionResult> Edit(int id = 0)
        {
 
                LOJA_EQUIPAMENTO le = await _db.LOJA_EQUIPAMENTO.Where(lee => lee.ID == id)
                    .FirstOrDefaultAsync();

                if (le == null)
                    return HttpNotFound();

                ViewBag.TIPO = new SelectList(await _db.TIPO
                    .Where(t => t.TIPO1 == "E" && t.SITUACAO == "A")
                    .OrderBy(te => te.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.TIPO);

                ViewBag.MODELO = new SelectList(await _db.MODELO
                    .Where(m => m.SITUACAO == "A")
                    .OrderBy(m => m.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.MODELO);


                return View(le);

        }

        public async Task<ActionResult> Display(int id = 0)
        {
            LOJA_EQUIPAMENTO le = await _db.LOJA_EQUIPAMENTO
                .Include(lee => lee.LOJA1)
                .Include(lee => lee.LOJA1.CLIENTE1)
                .Where(lee => lee.ID == id)
                .FirstOrDefaultAsync();

            if (le == null)
                return HttpNotFound();

            ViewBag.TIPO = new SelectList(await _db.TIPO
                .Where(t => t.ID == le.TIPO)
                .OrderBy(te => te.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.TIPO);

            ViewBag.MODELO = new SelectList(await _db.MODELO
                .Where(m => m.ID == le.MODELO)
                .OrderBy(m => m.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.MODELO);

            return View(le);
        }


        public async Task<ActionResult> GetModelos(Int32 tipo = 0)
        {
            var modelos = await _db.MODELO.Where(m => m.TIPO == tipo)
                .Select(m => new { id = m.ID, desc = m.DESCRICAO })
                .ToArrayAsync();

            return Json(modelos, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> EnviarImagem(Int32 id, HttpPostedFileBase file)
        {
            var item = await _db
                .LOJA_EQUIPAMENTO
                .Include(cl => cl.IMAGEM1)
                .FirstOrDefaultAsync(checkitem => checkitem.ID == id);

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



        public async Task<ActionResult> BaixarImagem(Int32 id)
        {
            var item = await _db
                .LOJA_EQUIPAMENTO
                .Include(cl => cl.IMAGEM1)
                .FirstOrDefaultAsync(le => le.ID == id);

            if (item != null && item.IMAGEM1 != null)
            {
                return File(item.IMAGEM1.BUFFER, MimeMapping.GetMimeMapping(item.IMAGEM1.NOME), item.IMAGEM1.NOME);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LOJA_EQUIPAMENTO le)
        {

            if (ModelState.IsValid)
            {
                _db.Configuration.ValidateOnSaveEnabled = false;

                _db.LOJA_EQUIPAMENTO.Attach(le);

                var entry = _db.Entry(le);

                entry.Property(p => p.SETOR).IsModified = true;
                entry.Property(p => p.SUBSETOR).IsModified = true;
                entry.Property(p => p.TIPO).IsModified = true;
                entry.Property(p => p.MODELO).IsModified = true;
                entry.Property(p => p.SERIE).IsModified = true;
                entry.Property(p => p.NUM_MODELO).IsModified = true;
                entry.Property(p => p.TENSAO_NOMINAL).IsModified = true;
                entry.Property(p => p.CORRENTE_NOMINAL).IsModified = true;
                entry.Property(p => p.FABRICANTE).IsModified = true;
                entry.Property(p => p.POTENCIA).IsModified = true;
                entry.Property(p => p.CAPACIDADE).IsModified = true;

                entry.Property(p => p.TAG).IsModified = true;
                entry.Property(p => p.DATA_FABRICACAO).IsModified = true;
                entry.Property(p => p.DATA_VENCIMENTO).IsModified = true;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { id = le.LOJA });
            }

            ViewBag.TIPO = new SelectList(await _db.TIPO
                .Where(t => t.TIPO1 == "E" && t.SITUACAO == "A")
                .OrderBy(te => te.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.TIPO);

            ViewBag.MODELO = new SelectList(await _db.MODELO
                .Where(m => m.SITUACAO == "A")
                .OrderBy(m => m.DESCRICAO).ToArrayAsync(), "ID", "DESCRICAO", le.MODELO);

            return View(le);

        }

        public async Task<FileResult> GetImagem(Int32 anexo)
        {
            var imagem = await _db.ANEXO
                 .FirstOrDefaultAsync(ax => ax.ID == anexo);

            if (imagem != null)
            {
                return File(imagem.BUFFER, MimeMapping.GetMimeMapping(imagem.NOME), imagem.NOME);
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