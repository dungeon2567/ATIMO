using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;

namespace Atimo.Controllers
{
    public class VeiculoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            var veiculo = _db.VEICULO
                .Include(v => v.MARCA1)
                .Include(v => v.MODELO1);

            return View(veiculo.ToList());
        }

        public ActionResult Create()
        {
            var viewModel = new VeiculoViewModel { FlgDeprecia = "N", Situacao = "A" };

            ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.TIPO == "V").Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO");
            ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO");
            return View(viewModel);
        }

        public ActionResult ConfirmarCreate(VeiculoViewModel viewModel)
        {
            #region Validações

            if (viewModel.Marca <= 0)
                return Json(new { status = 100, ex = "Informe uma marca!" });

            if (viewModel.Modelo <= 0)
                return Json(new { status = 100, ex = "Informe um modelo!" });

            if (viewModel.Ano <= 0)
                return Json(new { status = 100, ex = "Informe um ano!" });

            if (viewModel.Ano > DateTime.Now.Year)
                return Json(new { status = 100, ex = "Informe um ano!" });

            if (string.IsNullOrEmpty(viewModel.Placa))
                return Json(new { status = 100, ex = "Informe uma placa!" });

            if (viewModel.Placa.Length < 8)
                return Json(new { status = 100, ex = "Informe uma placa" });

            if (string.IsNullOrEmpty(viewModel.Combustivel))
                return Json(new { status = 100, ex = "Selecione um tipo de combustível!" });

            if (string.IsNullOrEmpty(viewModel.Seguro.Replace("R$", "").Trim()))
                return Json(new { status = 100, ex = "Informe o valor de seguro!" });

            if (Convert.ToDecimal(viewModel.Seguro.Replace("R$", "").Trim()) <= 0)
                return Json(new { status = 100, ex = "Informe um valor de seguro!" });

            if (string.IsNullOrEmpty(viewModel.Ipva.Replace("R$", "").Trim()))
                return Json(new { stats = 100, ex = "Informe um valor de IPVA!" });

            if (Convert.ToDecimal(viewModel.Ipva.Replace("R$", "").Trim()) < 0)
                return Json(new { stats = 100, ex = "Informe um valor de IPVA!" });

            if (string.IsNullOrEmpty(viewModel.Situacao))
                return Json(new { status = 100, ex = "Informe uma situação!" });

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

            var existe = _db.PATRIMONIO.Any(p => p.NUMERO == viewModel.Numero.ToUpper());

            if (existe)
                return Json(new { status = 100, ex = "Número de patrimonio já informado para outro veículo!" });

            #endregion

            #region Incluir Veículo

            var veiculo = new VEICULO
            {
                MARCA = viewModel.Marca,
                MODELO = viewModel.Modelo,
                ANO = viewModel.Ano,
                COMBUSTIVEL = viewModel.Combustivel,
                SEGURO = Convert.ToDecimal(viewModel.Seguro.Replace("R$", "").Trim()),
                IPVA = Convert.ToDecimal(viewModel.Ipva.Replace("R$", "").Trim()),
                SITUACAO = viewModel.Situacao
            };

            if (!string.IsNullOrEmpty(viewModel.Chassi))
                veiculo.CHASSI = viewModel.Chassi.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Placa))
                veiculo.PLACA = viewModel.Placa.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Observacao))
                veiculo.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.VEICULO.Add(veiculo);

            #endregion

            #region Incluir Patrimonio

            var patrimonio = new PATRIMONIO
            {
                FLG_DEPRECIA = viewModel.FlgDeprecia,
                DATA_AQUISICAO = Convert.ToDateTime(viewModel.DataAquisicao),
                VALOR_COMPRA = Convert.ToDecimal(viewModel.ValorCompra.Replace("R$", "").Trim()),
                TIPO_DEPRECIACAO = viewModel.TipoDepreciacao,
                DEPRECIACAO = Convert.ToDecimal(viewModel.Depreciacao),
                VEICULO1 = veiculo
            };

            if (!string.IsNullOrEmpty(viewModel.Numero))
                patrimonio.NUMERO = viewModel.Numero.ToUpper();

            _db.PATRIMONIO.Add(patrimonio);

            #endregion

            #region Incluir Rateio
            #endregion

            _db.SaveChanges();

            return Json(new { status = 200, msg = "Incluído com sucesso!" });
        }

        public ActionResult Edit(int id = 0)
        {
            var veiculo = _db.VEICULO.Find(id);

            if (veiculo == null)
            {
                return HttpNotFound();
            }

            var patrimonio = _db.PATRIMONIO.FirstOrDefault(p => p.VEICULO == id);
            var viewModel = new VeiculoViewModel
            {
                Id = veiculo.ID,
                Ano = veiculo.ANO,
                Chassi = veiculo.CHASSI,
                Placa = veiculo.PLACA,
                Combustivel = veiculo.COMBUSTIVEL,
                Seguro = veiculo.SEGURO.ToString("C"),
                Ipva = veiculo.IPVA.ToString("C"),
                Observacao = veiculo.OBSERVACAO,
                Situacao = veiculo.SITUACAO
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

            if (veiculo.MARCA != null)
            {
                viewModel.Marca = (int)veiculo.MARCA;

                ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.SITUACAO == "A").Where(m => m.TIPO == "V"), "ID", "DESCRICAO", veiculo.MARCA);
            }
            else
                ViewBag.MARCA = new SelectList(_db.MARCA.Where(m => m.SITUACAO == "A").Where(m => m.TIPO == "V"), "ID", "DESCRICAO");

            if (veiculo.MODELO != null)
            {
                viewModel.Modelo = (int)veiculo.MODELO;

                ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO", veiculo.MODELO);
            }
            else
                ViewBag.MODELO = new SelectList(_db.MODELO.Where(m => m.SITUACAO == "A"), "ID", "DESCRICAO");

            return View(viewModel);
        }

        public ActionResult ConfirmarEdit(VeiculoViewModel viewModel)
        {
            #region Validações

            if (viewModel.Marca <= 0)
                return Json(new { status = 100, ex = "Informe uma marca!" });

            if (viewModel.Modelo <= 0)
                return Json(new { status = 100, ex = "Informe um modelo!" });

            if (viewModel.Ano <= 0)
                return Json(new { status = 100, ex = "Informe um ano!" });

            if (viewModel.Ano > DateTime.Now.Year)
                return Json(new { status = 100, ex = "Informe um ano!" });

            if (string.IsNullOrEmpty(viewModel.Placa))
                return Json(new { status = 100, ex = "Informe uma placa!" });

            if (viewModel.Placa.Length < 8)
                return Json(new { status = 100, ex = "Informe uma placa" });

            if (string.IsNullOrEmpty(viewModel.Combustivel))
                return Json(new { status = 100, ex = "Selecione um tipo de combustível!" });

            if (string.IsNullOrEmpty(viewModel.Seguro.Replace("R$", "").Trim()))
                return Json(new { status = 100, ex = "Informe o valor de seguro!" });

            if (Convert.ToDecimal(viewModel.Seguro.Replace("R$", "").Trim()) <= 0)
                return Json(new { status = 100, ex = "Informe um valor de seguro!" });

            if (string.IsNullOrEmpty(viewModel.Ipva.Replace("R$", "").Trim()))
                return Json(new { stats = 100, ex = "Informe um valor de IPVA!" });

            if (Convert.ToDecimal(viewModel.Ipva.Replace("R$", "").Trim()) < 0)
                return Json(new { stats = 100, ex = "Informe um valor de IPVA!" });

            if (string.IsNullOrEmpty(viewModel.Situacao))
                return Json(new { status = 100, ex = "Informe uma situação!" });

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
                return Json(new { status = 100, ex = "Número de patrimonio já informado para outro veículo!" });

            #endregion

            #region Alterar Veículo

            var veiculo = new VEICULO
            {
                ID = viewModel.Id,
                MARCA = viewModel.Marca,
                MODELO = viewModel.Modelo,
                ANO = viewModel.Ano,
                COMBUSTIVEL = viewModel.Combustivel,
                SEGURO = Convert.ToDecimal(viewModel.Seguro.Replace("R$", "").Trim()),
                IPVA = Convert.ToDecimal(viewModel.Ipva.Replace("R$", "").Trim()),
                SITUACAO = viewModel.Situacao
            };

            if (!string.IsNullOrEmpty(viewModel.Chassi))
                veiculo.CHASSI = viewModel.Chassi.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Placa))
                veiculo.PLACA = viewModel.Placa.ToUpper();

            if (!string.IsNullOrEmpty(viewModel.Observacao))
                veiculo.OBSERVACAO = viewModel.Observacao.ToUpper();

            _db.Entry(veiculo).State = EntityState.Modified;

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
                VEICULO = viewModel.Id
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