using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using ATIMO;

namespace Atimo.Controllers
{
    public class EquipamentoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        //private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
          
                var equipamento = _db.EQUIPAMENTO.Include(e => e.GRUPO1).Include(e => e.MARCA1).Include(e => e.MODELO1).Include(e => e.TIPO1);
                return View(equipamento.ToList());

        }

        public ActionResult Create()
        {
            var viewModel = new EquipamentoViewModel {FlgDeprecia = "N"};

            ViewBag.GRUPO = new SelectList(_db.GRUPO.Where(g => g.SITUACAO == "A").Where(g => g.TIPO == "E"), "ID", "DESCRICAO");
            ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.SITUACAO == "A").Where(m => m.TIPO == "E"), "ID", "DESCRICAO");
            ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO");
            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "E"), "ID", "DESCRICAO");
            return View(viewModel);
        }

        public ActionResult ConfirmarCreate(EquipamentoViewModel viewModel)
        {
            #region Validações

            if (viewModel.Grupo <= 0)
                return Json(new {status = 100, ex = "Selecione um grupo!"});

            if (viewModel.Tipo <= 0)
                return Json(new { status = 100, ex = "Selecione um tipo!" });

            if (viewModel.Marca <= 0)
                return Json(new { status = 100, ex = "Selecione uma marca!" });

            if (viewModel.Modelo <= 0)
                return Json(new { status = 100, ex = "Selecione um modelo!" });

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new { status = 100, ex = "Informe uma descrição" });

            if (string.IsNullOrEmpty(viewModel.NumSerie))
                return Json(new {status = 100, ex = "Informe o número de série"});

            if (string.IsNullOrEmpty(viewModel.Situacao))
                return Json(new {status = 100, ex = "Informe a situação!"});

            if (string.IsNullOrEmpty(viewModel.Numero))
                return Json(new { status = 100, ex = "Informe um número de patrimonio para o veículo!" });

            if (string.IsNullOrEmpty(viewModel.FlgDeprecia))
                return Json(new { status = 100, ex = "Informe se o veículo sofre depreciação ou não!" });

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
                return Json(new { status = 100, ex = "Informe o tipo de depreciação!" });

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new { status = 100, ex = "Informe um percentual de depreciação!" });

            if (Convert.ToDecimal(viewModel.Depreciacao) < 0)
                return Json(new { status = 100, ex = "Informe um percentual de depreciação!" });

            var existe = _db.PATRIMONIO.Any(p => p.NUMERO == viewModel.Numero.ToUpper());

            if (existe)
                return Json(new { status = 100, ex = "Número de patrimonio já informado para outro Equipamento!" });

            #endregion

            #region Incluir Equipamento

            var equipamento = new EQUIPAMENTO
            {
                GRUPO = viewModel.Grupo,
                TIPO = viewModel.Tipo,
                MARCA = viewModel.Marca,
                MODELO = viewModel.Modelo,
                SITUACAO = viewModel.Situacao
            };

            if (!string.IsNullOrEmpty(viewModel.Depreciacao))
                equipamento.DESCRICAO = viewModel.Depreciacao.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Fabricante))
                equipamento.FABRICANTE = viewModel.Fabricante.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.NumSerie))
                equipamento.NUM_SERIE = viewModel.NumSerie.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Observacao))
                equipamento.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.EQUIPAMENTO.Add(equipamento);

            #endregion

            #region Incluir Patrimonio

            var patrimonio = new PATRIMONIO
            {
                FLG_DEPRECIA = viewModel.FlgDeprecia,
                DATA_AQUISICAO = Convert.ToDateTime(viewModel.DataAquisicao),
                VALOR_COMPRA = Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()),
                TIPO_DEPRECIACAO = viewModel.TipoDepreciacao,
                DEPRECIACAO = Convert.ToDecimal(viewModel.Depreciacao),
                EQUIPAMENTO1 = equipamento
            };

            if (!string.IsNullOrEmpty(viewModel.Numero))
                patrimonio.NUMERO = viewModel.Numero.ToUpper();

            _db.PATRIMONIO.Add(patrimonio);

            #endregion

            #region Incluir Rateio

            #endregion

            _db.SaveChanges();

            return Json(new {status = 200, msg = "Incluído com sucesso!"});
        }

        public ActionResult Edit(int id = 0)
        {
            var equipamento = _db.EQUIPAMENTO.Find(id);

            if (equipamento == null)
            {
                return HttpNotFound();
            }

            var patrimonio = _db.PATRIMONIO.FirstOrDefault(p => p.EQUIPAMENTO == id);
            var viewModel = new EquipamentoViewModel();
            viewModel.Id = equipamento.ID;
            viewModel.Grupo = equipamento.GRUPO;
            viewModel.Tipo = equipamento.TIPO;
            viewModel.Descricao = equipamento.DESCRICAO;
            viewModel.Fabricante = equipamento.FABRICANTE;
            viewModel.NumSerie = equipamento.NUM_SERIE;
            viewModel.Observacao = equipamento.OBSERVACAO;
            viewModel.Situacao = equipamento.SITUACAO;

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


            if (equipamento.MARCA != null)
            {
                viewModel.Marca = (int) equipamento.MARCA;
                ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.SITUACAO == "A").Where(m => m.TIPO == "E"), "ID", "DESCRICAO", equipamento.MARCA);
            } else
                ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.SITUACAO == "A").Where(m => m.TIPO == "E"), "ID", "DESCRICAO");

            if (equipamento.MODELO != null)
            {
                viewModel.Modelo = (int)equipamento.MODELO;
                ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO", equipamento.MODELO);
            } else
                ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO");

            ViewBag.GRUPO = new SelectList(_db.GRUPO.Where(g => g.SITUACAO == "A").Where(g => g.TIPO == "E"), "ID", "DESCRICAO", equipamento.GRUPO);
            ViewBag.TIPO = new SelectList(_db.TIPO.Where(t => t.SITUACAO == "A").Where(t => t.TIPO1 == "E"), "ID", "DESCRICAO", equipamento.TIPO);
            
            return View(viewModel);
        }

        public ActionResult ConfirmarEdit(EquipamentoViewModel viewModel)
        {
            #region Validações

            if (viewModel.Grupo <= 0)
                return Json(new { status = 100, ex = "Selecione um grupo!" });

            if (viewModel.Tipo <= 0)
                return Json(new { status = 100, ex = "Selecione um tipo!" });

            if (viewModel.Marca <= 0)
                return Json(new { status = 100, ex = "Selecione uma marca!" });

            if (viewModel.Modelo <= 0)
                return Json(new { status = 100, ex = "Selecione um modelo!" });

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new { status = 100, ex = "Informe uma descrição" });

            if (string.IsNullOrEmpty(viewModel.NumSerie))
                return Json(new { status = 100, ex = "Informe o número de série" });

            if (string.IsNullOrEmpty(viewModel.Situacao))
                return Json(new { status = 100, ex = "Informe a situação!" });

            if (string.IsNullOrEmpty(viewModel.Numero))
                return Json(new { status = 100, ex = "Informe um número de patrimonio para o veículo!" });

            if (string.IsNullOrEmpty(viewModel.FlgDeprecia))
                return Json(new { status = 100, ex = "Informe se o veículo sofre depreciação ou não!" });

            if (string.IsNullOrEmpty(viewModel.DataAquisicao))
                return Json(new { status = 100, ex = "Informe a data de aquisição do veículo!" });

            if (Convert.ToDateTime(viewModel.DataAquisicao) > DateTime.Now)
                return Json(new { status = 100, ex = "Informe uma data de aquisição" });

            if (string.IsNullOrEmpty(viewModel.ValorCompra.Replace("R$", "").Trim()))
                return Json(new { status = 100, ex = "Informe um valor de compra!" });

            if (Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()) < 0)
                return Json(new { status = 100, ex = "Informe um valor de compra!" });

            if (string.IsNullOrEmpty(viewModel.TipoDepreciacao))
                return Json(new { status = 100, ex = "Informe o tipo de depreciação!" });

            if (string.IsNullOrEmpty(viewModel.Depreciacao))
                return Json(new { status = 100, ex = "Informe um percentual de depreciação!" });

            if (Convert.ToDecimal(viewModel.Depreciacao) < 0)
                return Json(new { status = 100, ex = "Informe um percentual de depreciação!" });

            var existe = _db.PATRIMONIO.Any(p => p.NUMERO == viewModel.Numero.ToUpper() && p.ID != viewModel.Id);

            if (existe)
                return Json(new { status = 100, ex = "Número de patrimonio já informado para outro Equipamento!" });

            #endregion

            #region Alterar Equipamento

            var equipamento = new EQUIPAMENTO
            {
                ID = viewModel.Id,
                GRUPO = viewModel.Grupo,
                TIPO = viewModel.Tipo,
                MARCA = viewModel.Marca,
                MODELO = viewModel.Modelo,
                SITUACAO = viewModel.Situacao
            };

            if (!string.IsNullOrEmpty(viewModel.Depreciacao))
                equipamento.DESCRICAO = viewModel.Depreciacao.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Fabricante))
                equipamento.FABRICANTE = viewModel.Fabricante.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.NumSerie))
                equipamento.NUM_SERIE = viewModel.NumSerie.ToUpper();

            if (string.IsNullOrEmpty(viewModel.Observacao))
                equipamento.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.Entry(equipamento).State = EntityState.Modified;

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
                EQUIPAMENTO = viewModel.Id
            };

            if (!string.IsNullOrEmpty(viewModel.Numero))
                patrimonio.NUMERO = viewModel.Numero.ToUpper();

            _db.PATRIMONIO.AddOrUpdate(patrimonio);

            #endregion

            _db.SaveChanges();

            return Json(new {status = 200, msg = "Alterado com sucesso!"});
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}