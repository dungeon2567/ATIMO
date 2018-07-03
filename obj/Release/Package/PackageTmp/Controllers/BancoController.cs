using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO;
using System.Threading.Tasks;

namespace Atimo.Controllers
{
    public class BancoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
                return View(_db.BANCO.ToArray());

        }

        public ActionResult Create()
        {
            return View();
        }


        public async Task<JsonResult> GetBancos(string query)
        {

            var bancos = from b in _db.BANCO
                        where b.DESCRICAO.StartsWith(query) || b.CODIGO.StartsWith(query)
                        select b;

            return Json(await bancos.Select(b => new { id = b.ID.ToString(), text = b.CODIGO + " " + b.DESCRICAO }).ToArrayAsync(), JsonRequestBehavior.AllowGet);

        }
        public ActionResult ConfirmarCreate(BANCO banco)
        {
            #region Validações

            if (string.IsNullOrEmpty(banco.CODIGO))
                return Json(new {status = 100, ex = "Informe um código!"});

            if (string.IsNullOrEmpty(banco.DESCRICAO))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            if (string.IsNullOrEmpty(banco.SITUACAO))
                return Json(new {status = 100, ex = "Informe uma situação!"});

            #endregion

            #region Incluir Banco

            if (!string.IsNullOrEmpty(banco.CODIGO))
                banco.CODIGO = banco.CODIGO.ToUpper();

            if (!string.IsNullOrEmpty(banco.DESCRICAO))
                banco.DESCRICAO = banco.DESCRICAO.ToUpper();

            _db.BANCO.Add(banco);
            _db.SaveChanges();

            #endregion

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var banco = _db.BANCO.Find(id);

            if (banco == null)
            {
                return HttpNotFound();
            }

            return View(banco);
        }

        public ActionResult ConfirmarEdit(BANCO banco)
        {
            #region Validações

            if (string.IsNullOrEmpty(banco.CODIGO))
                return Json(new { status = 100, ex = "Informe um código!" });

            if (string.IsNullOrEmpty(banco.DESCRICAO))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (string.IsNullOrEmpty(banco.SITUACAO))
                return Json(new { status = 100, ex = "Informe uma situação!" });

            #endregion

            #region Alterar Banco

            if (!string.IsNullOrEmpty(banco.CODIGO))
                banco.CODIGO = banco.CODIGO.ToUpper();

            if (!string.IsNullOrEmpty(banco.DESCRICAO))
                banco.DESCRICAO = banco.DESCRICAO.ToUpper();

            _db.Entry(banco).State = EntityState.Modified;
            _db.SaveChanges();

            #endregion

            return Json(new { status = 200, msg = "Alterado com sucesso!" });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}