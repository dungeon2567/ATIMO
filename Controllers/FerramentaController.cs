using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class FerramentaController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            var ferramenta = _db.FERRAMENTA.Include(f => f.GRUPO1).Include(f => f.TIPO1);
            return View(ferramenta.ToList());
        }

        public ActionResult Create()
        {
            var viewModel = new FerramentaViewModel {FlgConsumivel = "N", FlgDeprecia = "N"};
            
            ViewBag.GRUPO = new SelectList(_db
                .GRUPO.Where(g => g.TIPO == "F").Where(g => g.SITUACAO == "A"), "ID", "DESCRICAO");

            ViewBag.TIPO = new SelectList(_db
                .TIPO.Where(t => t.TIPO1 == "F").Where(t => t.SITUACAO == "A"), "ID", "DESCRICAO");

            return View(viewModel);
        }

        public ActionResult ConfirmarCreate(FerramentaViewModel viewModel)
        {
            #region Validações

            if (viewModel.Grupo <= 0)
                return Json(new {status = 100, ex = "Informe um grupo!"});

            if (viewModel.Tipo <= 0)
                return Json(new { status = 100, ex = "Informe um tipo!" });

            if (string.IsNullOrEmpty(viewModel.Descricao))
                return Json(new {status = 100, ex = "Informe uma descrição!"});

            if (viewModel.Minimo < 0)
                return Json(new {status = 100, ex = "Informe uma quantidade minima!"});

            if (string.IsNullOrEmpty(viewModel.FlgConsumivel))
                return Json(new {status = 100, ex = "Informe se a ferramenta é consumível ou não!"});

            if (string.IsNullOrEmpty(viewModel.Numero))
                return Json(new {status = 100, ex = "Informe um número de patrimonio!"});

            if (string.IsNullOrEmpty(viewModel.FlgDeprecia))
                return Json(new {status = 100, ex = "Informe se a ferramenta deprecia ou não!"});

            if (string.IsNullOrEmpty(viewModel.DataAquisicao))
                return Json(new { status = 100, ex = "Informe a data de aquisição do veículo!" });

            if (Convert.ToDateTime(viewModel.DataAquisicao) > DateTime.Now)
                return Json(new { status = 100, ex = "Informe uma data de aquisição" });

            if (string.IsNullOrEmpty(viewModel.ValorCompra.Replace("R$", "").Trim()))
                return Json(new {status = 100, ex = "Informe um valor de compra!"});

            if (Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()) < 0)
                return Json(new { status = 100, ex = "Informe um valor de compra!" });

            if (string.IsNullOrEmpty(viewModel.TipoDepreciacao))
                return Json(new {status = 100, ex = "Informe um tipo de depreciação!"});

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new {status = 100, ex = "Informe o peercentual de depreciação!"});

            if (Convert.ToDecimal(viewModel.Depreciacao) < 0)
                return Json(new { status = 100, ex = "Informe o peercentual de depreciação!" });

            var existe = _db.PATRIMONIO.Any(p => p.NUMERO == viewModel.Numero.ToUpper());

            if (existe)
                return Json(new {status = 100, ex = "Número de patrimonio já informado para outra ferramenta!"});

            #endregion

            #region Incluir Ferramenta

            var ferramenta = new FERRAMENTA
            {
                GRUPO = viewModel.Grupo,
                TIPO = viewModel.Tipo, 
                MINIMO = viewModel.Minimo,
                FLG_CONSUMIVEL = viewModel.FlgConsumivel
            };

            if (!string.IsNullOrEmpty(viewModel.Descricao))
                ferramenta.DESCRICAO = viewModel.Descricao.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Observacao))
                ferramenta.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.FERRAMENTA.Add(ferramenta);

            #endregion

            #region Incluir Patrimonio

            var patrimonio = new PATRIMONIO
            {
                FLG_DEPRECIA = viewModel.FlgDeprecia,
                DATA_AQUISICAO = Convert.ToDateTime(viewModel.DataAquisicao),
                VALOR_COMPRA = Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()),
                TIPO_DEPRECIACAO = viewModel.TipoDepreciacao,
                DEPRECIACAO = Convert.ToDecimal(viewModel.Depreciacao),
                FERRAMENTA1 = ferramenta
            };

            if (!string.IsNullOrEmpty(viewModel.Numero))
                patrimonio.NUMERO = viewModel.Numero.ToUpper();

            _db.PATRIMONIO.Add(patrimonio);

            #endregion

            #region Incluir Rateio

            var projetos = _db.PROJETO.ToList();

            if (projetos.Count > 0)
            {
                projetos.ForEach(x =>
                {
                    var rateio = new RATEIO {PROJETO = x.ID, FERRAMENTA1 = ferramenta, SITUACAO = "A"};
                    _db.RATEIO.Add(rateio);
                });
            }

            #endregion

            _db.SaveChanges();

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var ferramenta = _db.FERRAMENTA.Find(id);

            if (ferramenta == null)
            {
                return HttpNotFound();
            }

            var patrimonio = _db.PATRIMONIO.FirstOrDefault(p => p.FERRAMENTA == id);

            var viewModel = new FerramentaViewModel
            {
                Id = ferramenta.ID,
                Grupo = ferramenta.GRUPO,
                Tipo = ferramenta.TIPO,
                Descricao = ferramenta.DESCRICAO,
                Minimo = ferramenta.MINIMO,
                Observacao = ferramenta.OBSERVACAO,
                FlgConsumivel = ferramenta.FLG_CONSUMIVEL
            };

            if (patrimonio != null)
            {
                viewModel.IdPatrimonio = patrimonio.ID;
                viewModel.Numero = patrimonio.NUMERO;
                viewModel.FlgDeprecia = patrimonio.FLG_DEPRECIA;
                viewModel.DataAquisicao = patrimonio.DATA_AQUISICAO.ToString("d");
                viewModel.ValorCompra = patrimonio.VALOR_COMPRA.ToString("C");
                viewModel.TipoDepreciacao = patrimonio.TIPO_DEPRECIACAO;
                viewModel.Depreciacao = patrimonio.DEPRECIACAO.ToString("F");
            }

            ViewBag.GRUPO = new SelectList(_db.GRUPO.Where(g => g.TIPO == "F").Where(g => g.SITUACAO == "A"), "ID", "DESCRICAO", ferramenta.GRUPO);
            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.TIPO1 == "F").Where(t => t.SITUACAO == "A"), "ID", "DESCRICAO", ferramenta.TIPO);

            return View(viewModel);
        }

        public ActionResult ConfirmarEdit(FerramentaViewModel viewModel)
        {
            #region Validações

            if (viewModel.Grupo <= 0)
                return Json(new { status = 100, ex = "Informe um grupo!" });

            if (viewModel.Tipo <= 0)
                return Json(new { status = 100, ex = "Informe um tipo!" });

            if (string.IsNullOrEmpty(viewModel.Descricao))
                return Json(new { status = 100, ex = "Informe uma descrição!" });

            if (viewModel.Minimo < 0)
                return Json(new { status = 100, ex = "Informe uma quantidade minima!" });

            if (string.IsNullOrEmpty(viewModel.FlgConsumivel))
                return Json(new { status = 100, ex = "Informe se a ferramenta é consumível ou não!" });

            if (string.IsNullOrEmpty(viewModel.Numero))
                return Json(new { status = 100, ex = "Informe um número de patrimonio!" });

            if (string.IsNullOrEmpty(viewModel.FlgDeprecia))
                return Json(new { status = 100, ex = "Informe se a ferramenta deprecia ou não!" });

            if (string.IsNullOrEmpty(viewModel.DataAquisicao))
                return Json(new { status = 100, ex = "Informe a data de aquisição do veículo!" });
            try
            {
                if (Convert.ToDateTime(viewModel.DataAquisicao) > DateTime.Now)
                    return Json(new { status = 100, ex = "Informe uma data de aquisição" });
            }
            catch (Exception)
            {
                return Json(new { status = 100, ex = "Informe uma data de aquisição" });
            }

            if (string.IsNullOrEmpty(viewModel.ValorCompra.Replace("R$", "").Trim()))
                return Json(new { status = 100, ex = "Informe um valor de compra!" });

            if (Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()) < 0)
                return Json(new { status = 100, ex = "Informe um valor de compra!" });

            if (string.IsNullOrEmpty(viewModel.TipoDepreciacao))
                return Json(new { status = 100, ex = "Informe um tipo de depreciação!" });

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new { status = 100, ex = "Informe o peercentual de depreciação!" });

            if (Convert.ToDecimal(viewModel.Depreciacao) < 0)
                return Json(new { status = 100, ex = "Informe o peercentual de depreciação!" });

            var existe = _db.PATRIMONIO.Any(p => p.NUMERO == viewModel.Numero.ToUpper() && p.ID != viewModel.IdPatrimonio);

            if (existe)
                return Json(new { status = 100, ex = "Número de patrimonio já informado para outra ferramenta!" });

            #endregion

            #region Alterar Ferramenta

            var ferramenta = new FERRAMENTA
            {
                ID = viewModel.Id,
                GRUPO = viewModel.Grupo,
                TIPO = viewModel.Tipo,
                MINIMO = viewModel.Minimo,
                FLG_CONSUMIVEL = viewModel.FlgConsumivel
            };

            if (!string.IsNullOrEmpty(viewModel.Descricao))
                ferramenta.DESCRICAO = viewModel.Descricao.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Observacao))
                ferramenta.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.Entry(ferramenta).State = EntityState.Modified;

            #endregion

            #region Alterar Patrimonio

            var patrimonio = new PATRIMONIO
            {
                ID = viewModel.IdPatrimonio,
                FLG_DEPRECIA = viewModel.FlgDeprecia,
                DATA_AQUISICAO = Convert.ToDateTime(viewModel.DataAquisicao),
                VALOR_COMPRA = Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()),
                TIPO_DEPRECIACAO = viewModel.TipoDepreciacao,
                DEPRECIACAO = Convert.ToDecimal(viewModel.Depreciacao),
                FERRAMENTA = viewModel.Id
            };

            if (!string.IsNullOrEmpty(viewModel.Numero))
                patrimonio.NUMERO = viewModel.Numero.ToUpper();

            _db.PATRIMONIO.AddOrUpdate(patrimonio);

            #endregion

            _db.SaveChanges();

            return Json(new { status = 200, msg = "Alterado com sucesso!" });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}