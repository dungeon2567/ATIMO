using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ATIMO.ViewModel;
using ATIMO.Models;
using ATIMO;

namespace Atimo.Controllers
{
    
    public class ContratoServicoController : FuncionarioController
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();
        
        //
        // GET: /OssbServico/
        public ActionResult Index(int id = 0)
        {

                var contrato = _db.CONTRATO.FirstOrDefault(co => co.ID == id);

                if (contrato == null)
                    return HttpNotFound();

                var contratoServico = _db.CONTRATO_SERVICO
                    .Include(o => o.PESSOA)
                    .Where(o => o.CONTRATO == id);

                var viewModel = new ContratoServicoViewModel { Lista = contratoServico.ToList(), Contrato = id };

                return View(viewModel);

        }

        //
        // GET: /OssbServico/Create

        public ActionResult Create(int id = 0, int area = 0)
        {
     
                var contrato = _db
                .CONTRATO
                .FirstOrDefault(c => c.ID == id);

                if (contrato == null)
                    return HttpNotFound();

                var viewModel = new ContratoServicoViewModel { Contrato = id, Erro = "N" };

                var area_manutencao = _db
                    .AREA_MANUTENCAOSet
                    .FirstOrDefault(a => a.ID == area);

                if (area_manutencao != null)
                {
                    viewModel.AreaManutencaoDescricao = area_manutencao.DESCRICAO;
                }

                ViewBag.TERCEIRO = new SelectList(_db
                                   .PESSOA
                                   .Where(p => p.SITUACAO == "A")
                                   .Where(p => p.TERCEIRO == 1), "ID", "RAZAO");

                ViewBag.UNIDADE = new SelectList(_db
                    .UNIDADE, "ID", "SIGLA");

                return View(viewModel);

        }

        //
        // POST: /OssbServico/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContratoServicoViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.ServicoDescricao))
                ModelState.AddModelError(string.Empty, "Informe um serviço!");

            if (String.IsNullOrEmpty(viewModel.AreaManutencaoDescricao))
                ModelState.AddModelError(string.Empty, "Informe uma area de manutenção!");


            if (ModelState.IsValid)
            {
                SERVICO servico = _db.SERVICO.FirstOrDefault(serv => serv.DESCRICAO == viewModel.ServicoDescricao);

                if (servico == null)
                {
                    servico = new SERVICO()
                    {
                        DESCRICAO = viewModel.ServicoDescricao,
                        ESPECIALIDADE = 1,
                        SITUACAO = "A"
                    };

                    _db.SERVICO.Add(servico);

                    _db.SaveChanges();
                }

                AREA_MANUTENCAO area_manutencao = _db.AREA_MANUTENCAOSet.FirstOrDefault(area => area.DESCRICAO == viewModel.AreaManutencaoDescricao);

                if (area_manutencao == null)
                {
                    area_manutencao = new AREA_MANUTENCAO()
                    {
                        DESCRICAO = viewModel.AreaManutencaoDescricao,
                        SITUACAO = "A"
                    };

                    _db.AREA_MANUTENCAOSet.Add(area_manutencao);

                    _db.SaveChanges();
                }


                var contratoServico = new CONTRATO_SERVICO
                {
                    CONTRATO = viewModel.Contrato,
                    AREA_MANUTENCAO = area_manutencao.ID,
                    SERVICO = servico.ID,
                    TERCEIRO = viewModel.Terceiro,
                    QUANTIDADE = Convert.ToDecimal(viewModel.Quantidade),
                    VALOR_MO = Convert.ToDecimal(viewModel.ValorMo),
                    VALOR_TERCEIRO = Convert.ToDecimal(viewModel.ValorTerceiro),
                    UNIDADE = viewModel.Unidade,
                };

                _db.CONTRATO_SERVICO.Add(contratoServico);
                _db.SaveChanges();

                return RedirectToAction("Create", new { id = viewModel.Contrato, area = contratoServico.AREA_MANUTENCAO });
            }

            viewModel.Erro = "S";

            ViewBag.UNIDADE = new SelectList(_db.UNIDADE, "ID", "SIGLA", viewModel.Unidade);
            ViewBag.TERCEIRO = new SelectList(_db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.TERCEIRO == 1), "ID", "RAZAO", viewModel.Terceiro);

            return View(viewModel);
        }

        //
        // GET: /OssbServico/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var contratoServico = _db.CONTRATO_SERVICO
                .FirstOrDefault(os => os.ID == id);

            if (contratoServico == null)
            {
                return HttpNotFound();
            }



            var viewModel = new ContratoServicoViewModel
            {
                Id = contratoServico.ID,
                Contrato = contratoServico.CONTRATO,
                AreaManutencaoDescricao = contratoServico.AREA_MANUTENCAO1.DESCRICAO,
                ServicoDescricao = contratoServico.SERVICO1.DESCRICAO,
                Terceiro = contratoServico.TERCEIRO,
                Quantidade = contratoServico.QUANTIDADE.ToString("F"),
                ValorMo = contratoServico.VALOR_MO.ToString("C"),
                ValorMat = contratoServico.VALOR_MAT.ToString("C"),
                ValorTerceiro = contratoServico.VALOR_TERCEIRO.ToString("C"),
                Erro = "N"
            };

            ViewBag.UNIDADE = new SelectList(_db.UNIDADE, "ID", "SIGLA", contratoServico.UNIDADE);
            ViewBag.TERCEIRO = new SelectList(_db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.TERCEIRO == 1), "ID", "RAZAO", contratoServico.TERCEIRO);


            return View(viewModel);
        }

        //
        // POST: /OssbServico/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContratoServicoViewModel viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.AreaManutencaoDescricao))
                ModelState.AddModelError(string.Empty, "Informe uma area de manutenção!");

            if (String.IsNullOrEmpty(viewModel.ServicoDescricao))
                ModelState.AddModelError(string.Empty, "Informe um serviço!");

            if (ModelState.IsValid)
            {
                SERVICO servico = _db
                    .SERVICO
                    .FirstOrDefault(serv => serv.DESCRICAO == viewModel.ServicoDescricao);

                if (servico == null)
                {
                    servico = new SERVICO()
                    {
                        DESCRICAO = viewModel.ServicoDescricao,
                        SITUACAO = "A",
                        ESPECIALIDADE = 1
                    };

                    _db.SERVICO.Add(servico);

                    _db.SaveChanges();

                }

                AREA_MANUTENCAO area_manutencao = _db.AREA_MANUTENCAOSet.FirstOrDefault(area => area.DESCRICAO == viewModel.AreaManutencaoDescricao);

                if (area_manutencao == null)
                {
                    area_manutencao = new AREA_MANUTENCAO()
                    {
                        DESCRICAO = viewModel.AreaManutencaoDescricao,
                        SITUACAO = "A"
                    };

                    _db.AREA_MANUTENCAOSet.Add(area_manutencao);

                    _db.SaveChanges();
                }

                var contratoServico = new CONTRATO_SERVICO
                {
                    ID = viewModel.Id,
                    CONTRATO = viewModel.Contrato,
                    AREA_MANUTENCAO = area_manutencao.ID,
                    SERVICO = servico.ID,
                    TERCEIRO = viewModel.Terceiro,
                    UNIDADE = viewModel.Unidade,
                    QUANTIDADE = Decimal.Parse(viewModel.Quantidade),
                    VALOR_MO = Convert.ToDecimal(viewModel.ValorMo.Replace("R$", "").Trim()),
                    VALOR_TERCEIRO = Convert.ToDecimal(viewModel.ValorTerceiro.Replace("R$", "").Trim()),
                };

                _db.Entry(contratoServico)
                    .State = EntityState.Modified;

                _db.SaveChanges();

                return RedirectToAction("Index", new { id = viewModel.Contrato });
            }

            viewModel.Erro = "S";

            ViewBag.TERCEIRO = new SelectList(_db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.TERCEIRO == 1), "ID", "RAZAO", viewModel.Terceiro);
            ViewBag.UNIDADE = new SelectList(_db.UNIDADE, "ID", "SIGLA", viewModel.Unidade);

            return View(viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private bool VerificarServico(string servico)
        {
            var existe = _db.SERVICO.Any(s => s.DESCRICAO == servico);

            return existe;
        }

        public ActionResult BuscarServicos(string term)
        {
            var items = (from s in _db.SERVICO
                         where s.DESCRICAO.Contains(term)
                         where s.SITUACAO == "A"
                         select s.DESCRICAO)
                         .Distinct()
                         .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BuscarAreaManutencao(string term)
        {
            var items = (from a in _db.AREA_MANUTENCAOSet
                         where a.DESCRICAO.Contains(term)
                         where a.SITUACAO == "A"
                         select a.DESCRICAO)
                         .Distinct()
                         .ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListaServicosArea(int contrato, string area)
        {
            return Json(_db.CONTRATO_SERVICO
                .Include(osserv => osserv.SERVICO1)
                .Include(osserv => osserv.AREA_MANUTENCAO1)
                .Where(osserv => osserv.AREA_MANUTENCAO1.DESCRICAO == area && osserv.CONTRATO == contrato)
                .Select(osserv=> osserv.SERVICO1.DESCRICAO).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaServicos(int id, int idc)
        {
            /*
            if (idc != 0)
            {
                var contratoServico = _db.CONTRATO_SERVICO
                    .Where(cs => cs.AREA_MANUTENCAO == id)
                    .Where(cs => cs.CONTRATO == idc)
                    .Where(cs => cs.SITUACAO == "A")
                    .Select(cs => cs.SERVICO);

                var servicos = _db.SERVICO.Where(s => contratoServico.Contains(s.ID));

                return Json(new SelectList(servicos.ToArray(), "ID", "DESCRICAO"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var servicos = _db
                    .SERVICO
                    .Where(s => s.SITUACAO == "A");

                return Json(new SelectList(servicos.ToArray(), "ID", "DESCRICAO"), JsonRequestBehavior.AllowGet);
            }
            */
            return null;
        }

        public JsonResult ListaTerceiros(int id, int idc)
        {
            if (idc != 0)
            {
                var terceiroServico = _db.TERCEIRO_SERVICO.Where(ts => ts.SITUACAO == "A").Where(ts => ts.SERVICO == id).Where(ts => ts.AREA_MANUTENCAO == idc).Select(ts => ts.TERCEIRO).ToList();
                var terceiros = _db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.TERCEIRO == 1).Where(p => terceiroServico.Contains(p.ID));

                return Json(new SelectList(terceiros.ToArray(), "ID", "RAZAO"), JsonRequestBehavior.AllowGet);
            }
            else {
                var terceiroServico = _db.TERCEIRO_SERVICO.Where(ts => ts.AREA_MANUTENCAO == id).Where(ts => ts.SITUACAO == "A").Select(ts => ts.TERCEIRO).ToList();
                var terceiros = _db.PESSOA.Where(p => p.SITUACAO == "A").Where(p => p.TERCEIRO == 1).Where(p => terceiroServico.Contains(p.ID));
                return Json(new SelectList(terceiros.ToArray(), "ID", "RAZAO"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObterQuantidade(int id, int idc)
        {
            /*
            if (idc == 0)
                return Json(new decimal(0), JsonRequestBehavior.AllowGet);

            var servico = _db.SERVICO.FirstOrDefault(s => s.ID == idc);
            var contratoServico = _db.CONTRATO_SERVICO.Where(cs => cs.TERCEIRO == id)
                .FirstOrDefault(cs => cs.SERVICO_DESCRICAO == servico.DESCRICAO);

            return Json(contratoServico != null ? new decimal((float)contratoServico.QUANTIDADE) : new decimal(0), JsonRequestBehavior.AllowGet);
            */

            return null;
        }

        public JsonResult ObterValorMo(int id, int idc)
        {
         /*   
            if (idc == 0)
                return Json(new decimal(0), JsonRequestBehavior.AllowGet);

            var servico = _db.SERVICO.FirstOrDefault(s => s.ID == idc);
            var contratoServico = _db.CONTRATO_SERVICO.FirstOrDefault(cs => cs.SERVICO_DESCRICAO == servico.DESCRICAO);

            return Json(contratoServico != null ? new decimal((float)contratoServico.VALOR_MO) : new decimal(0), JsonRequestBehavior.AllowGet);
           */ 

            return null;
        }
    } 
}
