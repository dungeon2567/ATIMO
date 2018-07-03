using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text;
using ATIMO;

namespace Atimo.Controllers
{

    public class OssbMaterialController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();


        public async Task<ActionResult> Index(int id = 0)
        {
            var ossbTerceiro = await _db.OSSB_MATERIAL
                .Include(m => m.PESSOA)
                .Include(m => m.UNIDADE1)
                .Where(m => m.OSSB == id)
                .ToArrayAsync();

            ViewBag.OSSB = id;

            return View(ossbTerceiro);

        }


        [HttpGet]
        public async Task<ActionResult> Comprar(Int32 id, string valor, string data){
            OSSB_MATERIAL material = new OSSB_MATERIAL();

            material.ID = id;

            _db.OSSB_MATERIAL.Attach(material);

            material.DATA_COMPRADO = DateTime.Parse(data);
            material.VALOR_COMPRADO = Decimal.Parse(valor);

            _db.Entry(material).Property(m => m.DATA_COMPRADO).IsModified = true;
            _db.Entry(material).Property(m => m.VALOR_COMPRADO).IsModified = true;

            _db.Configuration.ValidateOnSaveEnabled = false;

            await _db.SaveChangesAsync();

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Compras(int status = 1, String de = null, String ate = null, String descricao = null, String local_entrega = null)
        {
            var ossbMaterial =  _db.OSSB_MATERIAL
                .Include(m => m.PESSOA)
                .Include(m => m.OSSB1)
                .Include(m => m.UNIDADE1);

            ViewBag.STATUS = status;
            ViewBag.DE = de;
            ViewBag.ATE = ate;
            ViewBag.DESCRICAO = descricao;
            ViewBag.LOCAL_ENTREGA = local_entrega;


            switch (status)
            {
                case 1:
                    ossbMaterial = ossbMaterial.Where(m => m.DATA != null && m.DATA_COMPRADO == null && m.OSSB1.SITUACAO == "E");
                    break;

                case 2:
                    ossbMaterial = ossbMaterial.Where(m => m.DATA_COMPRADO != null);
                    break;
            }


            if (de != null)
            {
                var deDate = DateTime.Parse(de);

                ossbMaterial = ossbMaterial.Where(m => m.DATA >= deDate);
            }



            if (ate != null)
            {
                var ateDate = DateTime.Parse(ate);

                ossbMaterial = ossbMaterial.Where(m => m.DATA <= ateDate);
            }

            if (!String.IsNullOrEmpty(descricao))
            {

                ossbMaterial = ossbMaterial.Where(m => m.DESCRICAO.StartsWith(descricao));
            }

            if (!String.IsNullOrEmpty(local_entrega))
            {

                ossbMaterial = ossbMaterial.Where(m => m.LOCAL_ENTREGA.StartsWith(local_entrega));
            }

            return View(await ossbMaterial.OrderBy(m => m.DESCRICAO)
                .ThenBy(m => m.DATA)
                .ToArrayAsync());
        }

        [HttpGet]
        public async Task<JsonResult> Autocomplete(String query)
        {
            return Json(await _db.OSSB_MATERIAL.Where(m => m.DESCRICAO.StartsWith(query))
                .Select(m => m.DESCRICAO)
                .Distinct()
                .ToArrayAsync(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> Deletar(Int32 id)
        {
            var mat = await _db.OSSB_MATERIAL
                .FirstOrDefaultAsync(m => m.ID == id);

            if(mat.DATA_COMPRADO != null)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _db.Entry(mat).State = EntityState.Deleted;

                await _db.SaveChangesAsync();

                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        [HttpGet]
        public async Task<JsonResult> AutocompleteLocalEntrega(String query)
        {
            return Json(await _db.OSSB_MATERIAL.Where(m => m.LOCAL_ENTREGA.StartsWith(query))
                .Select(m => m.LOCAL_ENTREGA)
                .Distinct()
                .ToArrayAsync(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Create(int id = 0)
        {
            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA",null);

            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1).OrderBy(p => p.RAZAO).ToArrayAsync(), "ID", "RAZAO");

            return View(new OSSB_MATERIAL() { OSSB = id, QUANTIDADE = 1 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OSSB_MATERIAL material)
        {
            material.DESCRICAO = material.DESCRICAO?.
                Trim()
                .ToUpper();

            material.LOCAL_ENTREGA = material.LOCAL_ENTREGA?
                .Trim()
                .ToUpper();

            material.OBSERVACAO = material.OBSERVACAO?
                .Trim()
                .ToUpper();

            if (material.VALOR == 0)
                material.VALOR = null;

            if (ModelState.IsValid)
            {
                _db.OSSB_MATERIAL.Add(material);

                _db.SaveChanges();

                return RedirectToAction("Index", new { id = material.OSSB });
            }

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", material.UNIDADE);


            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1).OrderBy(p => p.RAZAO).ToArrayAsync(), "ID", "RAZAO", material.FORNECEDOR);

            return View(material);
        }

        public async Task<ActionResult> Edit(int id = 0)
        {
            var material = await _db.OSSB_MATERIAL.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (material == null)
            {
                return HttpNotFound();
            }

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", material.UNIDADE);


            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1).OrderBy(p => p.RAZAO).ToArrayAsync(), "ID", "RAZAO", material.FORNECEDOR);

            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OSSB_MATERIAL material)
        {
            material.DESCRICAO = material.DESCRICAO?.
                Trim()
                .ToUpper();

            material.LOCAL_ENTREGA = material.LOCAL_ENTREGA?
                .Trim()
                .ToUpper();

            material.OBSERVACAO = material.OBSERVACAO?
                .Trim()
                .ToUpper();

            if (material.VALOR == 0)
                material.VALOR = null;

            if (ModelState.IsValid)
            {
                _db.OSSB_MATERIAL.Attach(material);

                _db.Entry(material).Property(m => m.DATA).IsModified = true;
                _db.Entry(material).Property(m => m.VALOR).IsModified = true;
                _db.Entry(material).Property(m => m.DESCRICAO).IsModified = true;
                _db.Entry(material).Property(m => m.FORNECEDOR).IsModified = true;
                _db.Entry(material).Property(m => m.QUANTIDADE).IsModified = true;
                _db.Entry(material).Property(m => m.UNIDADE).IsModified = true;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { id = material.OSSB});
            }

            ViewBag.UNIDADE = new SelectList(await _db.UNIDADE.ToArrayAsync(), "ID", "SIGLA", material.UNIDADE);

            ViewBag.FORNECEDOR = new SelectList(await _db.PESSOA.Where(p => p.FORNECEDOR == 1).OrderBy(p => p.RAZAO).ToArrayAsync(), "ID", "RAZAO", material.FORNECEDOR);

            return View(material);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


    }

}