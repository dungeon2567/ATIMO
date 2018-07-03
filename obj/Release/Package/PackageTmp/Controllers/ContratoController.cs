using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using WebGrease.Css.Extensions;
using ATIMO.Models;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using ATIMO;

namespace Atimo.Controllers
{
    public class ContratoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        //
        // GET: /Contrato/

        public async Task<ActionResult> Index(int id = 0)
        {
           
                var cvs = await _db
                .CONTRATO
                .Where(c => c.CLIENTE == id)
                .Select(c => new
                {
                    Contrato = c,
                    Valor =
                    _db.CONTRATO_LOJA
                        .Where(cl => cl.CONTRATO == c.ID)
                        .Select(cl => cl.VALOR)
                        .DefaultIfEmpty()
                        .Sum()
                })
                .ToArrayAsync();

                var viewModel = new ContratoIndexViewModel
                {
                    Lista = cvs.Select(cv =>
                    {
                        var contrato = cv.Contrato;

                        var valor = cv.Valor;

                        return new ContratoViewModel()
                        {
                            Id = contrato.ID,
                            Cliente = contrato.CLIENTE,
                            Descricao = contrato.DESCRICAO,
                            Situacao = contrato.SITUACAO,
                            Gerado = contrato.GERADO,
                            Valor = valor.ToString("C"),
                        };
                    }).ToList(),
                    Cliente = id
                };

                return View(viewModel);

        }

        //
        // GET: /Contrato/Create

        public ActionResult Create(int id = 0)
        {
            var viewModel = new ContratoViewModel { Cliente = id };

            return View(viewModel);
        }

        //
        // POST: /Contrato/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContratoViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (!string.IsNullOrEmpty(viewModel.Descricao))
                viewModel.Descricao = viewModel.Descricao.ToUpper();

            var contrato = new CONTRATO
            {
                CLIENTE = viewModel.Cliente,
                SITUACAO = viewModel.Situacao,
                DESCRICAO = viewModel.Descricao,
                GERADO = "N"
            };

            _db.CONTRATO.Add(contrato);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { id = viewModel.Cliente });
        }

        //
        // GET: /Contrato/Edit/5

        public async Task<ActionResult> Edit(int id = 0)
        {
            var cv = await _db
                .CONTRATO
                .Where(c => c.ID == id)
                .Select(c => new
                {
                    Contrato = c,
                    Valor =
                    _db.CONTRATO_LOJA
                        .Where(cl => cl.CONTRATO == c.ID)
                        .Select(cl => cl.VALOR)
                        .DefaultIfEmpty()
                        .Sum()
                })
                .FirstOrDefaultAsync();

            if (cv == null)
            {
                return HttpNotFound();
            }

            var contrato = cv.Contrato;

            var valor = cv.Valor;

            var viewModel = new ContratoViewModel
            {
                Id = contrato.ID,
                Cliente = contrato.CLIENTE,
                Descricao = contrato.DESCRICAO,
                Situacao = contrato.SITUACAO,
                Gerado = contrato.GERADO,
                Valor = valor.ToString("C"),
            };


            return View(viewModel);
        }

        //
        // POST: /Contrato/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContratoViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var contrato = new CONTRATO
            {
                ID = viewModel.Id,
                CLIENTE = viewModel.Cliente,
                SITUACAO = viewModel.Situacao,
                GERADO = viewModel.Gerado,
            };

            if (!string.IsNullOrEmpty(viewModel.Descricao))
                contrato.DESCRICAO = viewModel.Descricao.ToUpper();

            _db.Entry(contrato).State = EntityState.Modified;
            _db.SaveChanges();

            return RedirectToAction("Index", new { id = viewModel.Cliente });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public DateTime VerificarData(DateTime data)
        {
            while (true)
            {
                var existe = _db
                    .FERIADO
                    .Any(x => (data.Day + "/" + data.Month) == x.DATA);

                if (existe)
                {
                    data = data.AddDays(1);
                    continue;
                }

                break;
            }

            if (data.DayOfWeek == DayOfWeek.Sunday)
            {
                data = data.AddDays(1);
            }

            return data;
        }

        public ActionResult GerarOs(int id)
        {
            /*

            var contrato = _db
                .CONTRATO
                .FirstOrDefault(c => c.ID == id);

            if(contrato.GERADO == "S")
                return Json(new { status = 100 });

            var loja = _db.CLIENTE
                .FirstOrDefault(l => l.ID == contrato.CLIENTE);
               

            var servico = _db.CONTRATO_SERVICO
                .Where(cs => cs.CONTRATO == id)
                .ToList();

            var dataInicio = contrato.DATA_INICIO;

            if (servico.Count == 0)
                return Json(new { status = 100 });

            contrato.GERADO = "S";

            _db.Entry(contrato).State = EntityState.Modified;

            _db.SaveChanges();

            for (var i = 0; i < contrato.DURACAO; i++)
            {
                dataInicio = dataInicio.AddDays(contrato.PERIODICIDADE);

                var ossb = new OSSB
                {
                    CONTRATO = contrato.ID,
                    DATA_CADASTRO = VerificarData(dataInicio),
                    PROJETO = 1,
                    CLIENTE = loja.CLIENTE,
                    OCORRENCIA = "OSSB RELATIVA AO CONTRATO: " + contrato.ID.ToString(""),
                    HISTORICO = "CRIAÇÃO DE OS'S DE CONTRATO, RELATIVAS AO CONTRATO: " + contrato.ID.ToString(""),
                    AGENDADO_DIA = VerificarData(dataInicio),
                    PORTE = "P",
                    SITUACAO = "E",
                    AMBIENTE = "A",
                    TIPO = "P",
                    EXECUCAO_INICIO = VerificarData(dataInicio)
                };

                _db.OSSB.Add(ossb);
                _db.SaveChanges();

                servico.ForEach(y =>
                {
                    var ossbServico = new OSSB_SERVICO
                    {
                        OSSB = ossb.ID,
                        AREA_MANUTENCAO = y.AREA_MANUTENCAO,
                        SERVICO = y.SERVICO,
                        UNIDADE = y.UNIDADE,
                        TERCEIRO = y.TERCEIRO,
                        QUANTIDADE = y.QUANTIDADE,
                        VALOR_MO = y.VALOR_MO,
                        FLG_ABANDONO = "N"
                    };

                    _db.OSSB_SERVICO.Add(ossbServico);
                    _db.SaveChanges();
                });
            }
                

            return Json(new { status = 200 });
            */

            return null;
        }
    }
}