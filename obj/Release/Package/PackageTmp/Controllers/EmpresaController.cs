using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using System.Threading.Tasks;
using Nfse.PMSP.RN;
using ATIMO.Helpers;

using ATIMO;
namespace Atimo.Controllers
{
    public class EmpresaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        public async Task<ActionResult> Index()
        {
            return View(await _db
                .EMPRESA
                .ToArrayAsync());
        }

        public async Task<ActionResult> CreateOrEdit(int? id)
        {
            if (Session.IsFuncionario())
            {
                ViewBag.SERVICOS = await _db
                    .FAT_SERVICOS_PMSP
                    .ToArrayAsync();

                if (id == null)
                {
                    return View(new EMPRESA() { FLG_EMITE_NF = "S", FLG_OPTANTE_SIMPLES_NACIONAL = "S" });
                }
                else
                {
                    var empresa = await _db
                        .EMPRESA
                        .FindAsync(id);

                    if (empresa == null)
                        return HttpNotFound();

                    else
                        return View(empresa);
                }
            }
            else
                return RedirectToAction("", "");
        }

        [HttpGet]
        public JsonResult getSerialNumberCertificado(string astrCnpjEmpresa)
        {
            int lintStatus;
            string lstrMensagem = string.Empty;
            Empresa lobjEmpresa = new Empresa();

            try
            {
                string lstrCertificado = Certificado.getCurrentSerial(out lobjEmpresa);

                if (string.IsNullOrEmpty(lobjEmpresa.CNPJ))
                {
                    lintStatus = 0;
                    lstrMensagem = "Nenhum certificado foi encontrado";
                }
                else if (Utils.LimpaCpfCnpj(astrCnpjEmpresa) != Utils.LimpaCpfCnpj(lobjEmpresa.CNPJ))
                {
                    lintStatus = 0;
                    lstrMensagem = "O CNPJ do cadastro (" + astrCnpjEmpresa + ") não confere com o CNPJ do certificado (" + lobjEmpresa.CNPJ + ")";
                }
                else
                {
                    lintStatus = 1;
                    lobjEmpresa.SerialCertificado = lstrCertificado;
                }

            }
            catch (System.Exception Erro)
            {
                lintStatus = 3;
                lstrMensagem = Erro.Message;
            }

            var lobjRet = new { Status = lintStatus, Mensagem = lstrMensagem, SerialCertificado = lobjEmpresa.SerialCertificado };

            return Json(lobjRet, JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(EMPRESA empresa)
        {

            if (string.IsNullOrEmpty(empresa.CNPJ))
                ModelState.AddModelError("", "Informe um cnpj!");

            if (string.IsNullOrEmpty(empresa.NOME))
                ModelState.AddModelError("", "Informe um nome!");

            if (ModelState.IsValid)
            {
                if (empresa.ID != 0)
                {
                    var emp = await _db.EMPRESA.Include(e => e.FAT_SERVICOS_PMSP)
                        .FirstOrDefaultAsync(e => e.ID == empresa.ID);

                    emp.FAT_SERVICOS_PMSP.Clear();

                    foreach (var fsp in empresa.FAT_SERVICOS_PMSP)
                    {
                        emp.FAT_SERVICOS_PMSP.Add(fsp);
                    }

                    await _db.SaveChangesAsync();
                }

                empresa.NOME = empresa
                    .NOME
                    .ToUpper();

                if (empresa.ID == 0)
                {
                    _db.EMPRESA.Add(empresa);
                }
                else
                {
                    _db.Entry(empresa)
                        .State = EntityState.Modified;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.SERVICOS = await _db
                .FAT_SERVICOS_PMSP
                .ToArrayAsync();

            return View(empresa);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}