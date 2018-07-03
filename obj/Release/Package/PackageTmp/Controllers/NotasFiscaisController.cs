using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.Models.Faturamento;
using ATIMO.Helpers.ModelsUtils;
using ATIMO.Helpers.PagedData;
using Nfse.PMSP.RN;
using System.Data.Entity;
using ATIMO.Helpers;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using Atimo.Controllers;

namespace ATIMO.Controllers
{
    public class NotasFiscaisController : FuncionarioController
    {
        private readonly ATIMOEntities m_DbContext = new ATIMOEntities();
        
        private readonly IFaturamentoRepository m_faturamentoRepository;
        private readonly IEmpresaRepository m_empresaRepository;
        private readonly IServicoPMSPRepository m_servicoPMSPRepository;
        private readonly IMunicipioRepository m_municipioRepository;
        private readonly IFaturamentoDescricaoRepository m_faturamentoDescricaoRepository;
        private readonly IEmailTemplateRepository m_emailTemplateDescricaoRepository;
        private readonly ISituacaoPagamentoRepository m_situacaoPagamentoRepository;
        private readonly ITipoImpostoRepository m_tipoImpostoRepository;

        private readonly DbSet<PESSOA> m_pessoasRepositiry = new ATIMOEntities().PESSOA;
        


        public NotasFiscaisController() : this(new FaturamentoRepository(), new EmpresaRepository(), new ServicoPMSPRepository(), new MunicipioRepository(), new FaturamentoDescricaoRepository(), new EmailTemplateRepository(), new SituacaoPagamentoRepository(), new TipoImpostoRepository())
        {
        }

        public NotasFiscaisController(IFaturamentoRepository p_faturamentoRepository, IEmpresaRepository p_empresaRepository, IServicoPMSPRepository p_servicoPMSPRepository, IMunicipioRepository p_municipioRepository, IFaturamentoDescricaoRepository p_faturamentoDescricaoRepository, IEmailTemplateRepository p_emailTemplateDescricaoRepository, ISituacaoPagamentoRepository p_situacaoPagamentoRepository, ITipoImpostoRepository p_tipoImpostoRepository)
        {
            m_faturamentoRepository = p_faturamentoRepository;
            m_empresaRepository = p_empresaRepository;
            m_servicoPMSPRepository = p_servicoPMSPRepository;
            m_municipioRepository = p_municipioRepository;
            m_faturamentoDescricaoRepository = p_faturamentoDescricaoRepository;
            m_emailTemplateDescricaoRepository = p_emailTemplateDescricaoRepository;
            m_situacaoPagamentoRepository = p_situacaoPagamentoRepository;
            m_tipoImpostoRepository = p_tipoImpostoRepository;
        }

        private FaturamentoCrudViewModel GetFaturamentoCreateViewModel(int IDOSSB, int IDCliente, int IDEmpresaEmissora, decimal ValorServico, decimal ValorMaoDeObra)
        {
            try
            {
                FaturamentoCrudViewModel lobjNFViewModel = new FaturamentoCrudViewModel();

                List<string> llistOutAlert = new List<string>();

                lobjNFViewModel.Faturamento = new Faturamento();
                lobjNFViewModel.Faturamento.OSSBID = Convert.ToInt32(IDOSSB);
                lobjNFViewModel.Faturamento.ValorMaoDeObra = ValorMaoDeObra;
                lobjNFViewModel.Faturamento.ValorServico = ValorServico;
                lobjNFViewModel.Faturamento.ValorBruto = lobjNFViewModel.Faturamento.GetValorTotal();
                lobjNFViewModel.Faturamento.DataEmissao = DateTime.Now;
                

                lobjNFViewModel.EmpresasEmissorasDisponiveis = m_empresaRepository.All.Where(p => p.EmiteNF == "S").ToList();
                
                if (IDEmpresaEmissora != 0)
                {
                    lobjNFViewModel.Faturamento.EmpresaEmissao = lobjNFViewModel.EmpresasEmissorasDisponiveis.Where(p => p.ID == IDEmpresaEmissora).FirstOrDefault();
                    lobjNFViewModel.Faturamento.EmpresaEmissaoID = IDEmpresaEmissora;
                }

                //Empresa informada não encontrada ou não informada
                if (lobjNFViewModel.Faturamento.EmpresaEmissao == null)
                {
                    lobjNFViewModel.Faturamento.EmpresaEmissao = lobjNFViewModel.EmpresasEmissorasDisponiveis.FirstOrDefault(); //TODO:Modificar para pegar a empresa marcada como padrão e não simplesmente a 1a da lista.
                    lobjNFViewModel.Faturamento.EmpresaEmissaoID = lobjNFViewModel.Faturamento.EmpresaEmissao.ID;
                }

                lobjNFViewModel.ServicosPMSP = lobjNFViewModel.Faturamento.EmpresaEmissao.ServicosPMSP.Select(o => o.ServicoPMSP).ToList();

                if (IDCliente != 0)
                {
                    lobjNFViewModel.Faturamento.Cliente = m_DbContext.PESSOA.Find(IDCliente); //lobjCliente = Utils.getPessoaFromReceita_PMSP(lobjCliente.NUM_DOC, ref llistOutAlert);
                    lobjNFViewModel.Faturamento.ClienteID = IDCliente;
                }

                if (lobjNFViewModel.Faturamento.Cliente != null)
                {
                    if (lobjNFViewModel.Faturamento.ClienteID != 0)
                    {
                        if (PessoaTomador.IsTomadorPreenchidoCorretamente(lobjNFViewModel.Faturamento.Cliente, ref llistOutAlert))
                        {
                            string lstrTempMensRetencoes;
                            PessoaTomador lobjTomador = new PessoaTomador(lobjNFViewModel.Faturamento.Cliente);

                            lobjNFViewModel.Faturamento.Retencoes = FaturamentoRetencoes.getRetencoes(ValorServico + ValorMaoDeObra, ValorMaoDeObra, lobjNFViewModel.Faturamento.EmpresaEmissao, ref lobjTomador, out lstrTempMensRetencoes);

                            lobjNFViewModel.RetencoesViewModel = new FaturamentoRetencoesTableViewModel();
                            lobjNFViewModel.RetencoesViewModel.MensagemRetencoes = lstrTempMensRetencoes;
                            if (lobjNFViewModel.Faturamento.Retencoes != null)
                                lobjNFViewModel.RetencoesViewModel.FaturamentoRetencoes = lobjNFViewModel.Faturamento.Retencoes.ToList();
                            else
                                lobjNFViewModel.RetencoesViewModel.FaturamentoRetencoes = new List<FaturamentoRetencoes>();

                            lobjNFViewModel.RetencoesViewModel.ValorLiquido = lobjNFViewModel.Faturamento.GetValorLiquido();
                            lobjNFViewModel.RetencoesViewModel.TotalRetencoes = lobjNFViewModel.Faturamento.GetTotalRetencoes();
                            lobjNFViewModel.RetencoesViewModel.ValorTotalNota = lobjNFViewModel.Faturamento.GetValorTotal();
                            lobjNFViewModel.CodigoIbgeCidadeTomador = lobjTomador.CodigoMunicipioIBGE.ToString();
                        }
                    }
                }
                else
                {
                    lobjNFViewModel.Faturamento.Cliente = new PESSOA();
                    llistOutAlert.Add("Cliente com código " + IDCliente + " não encontrado");
                }

                lobjNFViewModel.MensagemValidacaoCliente = new List<string>(llistOutAlert);

                return lobjNFViewModel;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ID do daturamento</param>
        /// <returns></returns>
        public ActionResult DownloadPdfNotaFiscal(int id)
        {
            try
            {

                Faturamento lobjNota = m_faturamentoRepository.All.Where(p => p.ID == id).FirstOrDefault<Faturamento>();
                if (lobjNota == null)
                    throw new Exception("Não foi encontrado um faturamento com ID " + id);

                string lstrArquivo = lobjNota.getCaminhoPdfNf(1);

                if (System.IO.File.Exists(lstrArquivo) == false)
                    throw new Exception("Não foi encontrado um arquivo de nota fiscal para este faturamento");

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = lstrArquivo.Substring(lstrArquivo.LastIndexOf(@"\") + 1),
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(System.IO.File.ReadAllBytes(lstrArquivo), MimeMapping.GetMimeMapping(lstrArquivo));

            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return Index();
            }
        }

        private FaturamentoCrudViewModel GetFaturamentoViewOrEditViewModel(int id)
        {
            try
            {
                Faturamento lobjFaturamento = m_faturamentoRepository.Find(id);
                return GetFaturamentoViewOrEditViewModel(lobjFaturamento);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private FaturamentoCrudViewModel GetFaturamentoViewOrEditViewModel(Faturamento aobjFaturamento)
        {
            try
            {
                List<string> llistOutAlert = new List<string>();
                FaturamentoCrudViewModel lobjNFViewModel = new FaturamentoCrudViewModel();
                Faturamento lobjFaturamento = aobjFaturamento;

                if (lobjFaturamento == null)
                    lobjFaturamento = new Faturamento();

                lobjNFViewModel.Faturamento = lobjFaturamento;
                lobjNFViewModel.Faturamento.Cliente = new PESSOA();
                lobjNFViewModel.EmpresasEmissorasDisponiveis = m_empresaRepository.All.Where(p => p.EmiteNF == "S").ToList();
                lobjNFViewModel.ServicosPMSP = m_servicoPMSPRepository.All.ToList();
                

                if (lobjFaturamento != null)
                {

                    if (lobjFaturamento.ClienteID != 0)
                        lobjNFViewModel.Faturamento.Cliente = m_DbContext.PESSOA.Find(lobjFaturamento.ClienteID);

                    PessoaTomador.IsTomadorPreenchidoCorretamente(lobjNFViewModel.Faturamento.Cliente, ref llistOutAlert);

                    if (lobjNFViewModel.Faturamento.EmpresaEmissao == null)
                        lobjNFViewModel.Faturamento.EmpresaEmissao = lobjNFViewModel.EmpresasEmissorasDisponiveis.Where(p => p.ID == lobjNFViewModel.Faturamento.EmpresaEmissaoID).FirstOrDefault();

                    if (lobjNFViewModel.Faturamento.ValorBruto == 0)
                        lobjNFViewModel.Faturamento.ValorBruto = lobjNFViewModel.Faturamento.GetValorTotal();

                        if (lobjFaturamento.Retencoes != null)
                    {
                        string lstrMensagemRetencao = string.Empty;
                        PessoaTomador lobjTomador = new PessoaTomador(lobjNFViewModel.Faturamento.Cliente);

                        FaturamentoRetencoes.getRetencoes(0, 0, lobjNFViewModel.Faturamento.EmpresaEmissao, ref lobjTomador, out lstrMensagemRetencao);
                        List<TipoImposto> l_TiposImpostos = m_tipoImpostoRepository.All.ToList();
                        foreach (var retencao in lobjFaturamento.Retencoes)
                        {
                            TipoImposto l_TipoImpostoTemp = l_TiposImpostos.Where(p => p.ID == retencao.TipoImpostoID).FirstOrDefault();
                            if (l_TipoImpostoTemp != null)
                            {
                                retencao.TipoImposto = l_TipoImpostoTemp;
                            }
                        }

                        lobjNFViewModel.RetencoesViewModel = new FaturamentoRetencoesTableViewModel();
                        lobjNFViewModel.RetencoesViewModel.FaturamentoRetencoes = lobjFaturamento.Retencoes.ToList();
                        lobjNFViewModel.RetencoesViewModel.TotalRetencoes = lobjFaturamento.GetTotalRetencoes();
                        lobjNFViewModel.RetencoesViewModel.ValorLiquido = lobjFaturamento.GetValorLiquido();
                        lobjNFViewModel.RetencoesViewModel.ValorTotalNota = lobjFaturamento.GetValorTotal();
                        lobjNFViewModel.RetencoesViewModel.MensagemRetencoes = lstrMensagemRetencao;
                        lobjNFViewModel.CodigoIbgeCidadeTomador = lobjTomador.CodigoMunicipioIBGE.ToString();

                    }

                }

                return lobjNFViewModel;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult getNotasFromPMSP()
        {
            int lintLoteDefault = 1;
            try
            {

                NFSePmspSender lobjgetNF = new NFSePmspSender();
                EnvioParamPeriodo lobjPeriodo = new EnvioParamPeriodo(enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo, DateTime.Now.AddMonths(-1), DateTime.Now);
                Empresa lobjEmpresa = new Empresa();
                string lstrCertificado = Certificado.getCurrentSerial(out lobjEmpresa);
                IQueryable<Empresa> lobjEmpresas = m_empresaRepository.All.Where(p => p.SerialCertificado == lstrCertificado);

                if (lobjEmpresas.Count<Empresa>() == 0)
                    throw new Exception("Não existe uma empresa cadastrada com o certificado serial number " + lstrCertificado);
                else
                    lobjEmpresa = lobjEmpresas.First<Empresa>();

                lobjgetNF.iniciaEnvio(lobjPeriodo, lobjEmpresa, 1);

                foreach (Faturamento lobjFatRetXML in lobjgetNF.FaturamentosRetorno)
                {
                    IQueryable<Faturamento> lobjRetWhereFat = (m_faturamentoRepository.All.Where(p => p.NumeroNF == lobjFatRetXML.NumeroNF && p.CodigoVerificacao == lobjFatRetXML.CodigoVerificacao && p.NumeroRPS == lobjFatRetXML.NumeroRPS));
                    if (lobjRetWhereFat.Count<Faturamento>() == 0)
                    {
                        if (lobjFatRetXML.ClienteID > 0)
                            lobjFatRetXML.Cliente.ID = lobjFatRetXML.ClienteID;
                        if (lobjFatRetXML.Cliente.ID == 0)
                        {
                            lobjFatRetXML.Cliente.NUM_DOC = Utils.FormataCpfCnpj(lobjFatRetXML.Cliente.NUM_DOC);
                            PESSOA lobjPessoaDB = m_pessoasRepositiry.Where(p => p.NUM_DOC == lobjFatRetXML.Cliente.NUM_DOC).FirstOrDefault<PESSOA>();
                            if (lobjPessoaDB != null)
                            { 
                                lobjFatRetXML.Cliente.ID = lobjPessoaDB.ID;
                                lobjFatRetXML.ClienteID = lobjPessoaDB.ID;
                            }
                        }

                        lobjFatRetXML.DataCadastro = DateTime.Now;
                        lobjFatRetXML.OSSBID = 118;
                        lobjFatRetXML.LoteEmissaoPMSP = lintLoteDefault;
                        
                        m_faturamentoRepository.InsertOrUpdate(lobjFatRetXML);
                    }
                }

                m_faturamentoRepository.Save();

                return Index(1, null, null, null, null, 0, 0, 0, 0, lintLoteDefault);
            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return Index(1, null, null, null, null, 0, 0, 0, 0, 0);
            }
        }

        private PagedData<Faturamento> PagedList(IQueryable<Faturamento> a_FaturamentoList, int? a_page = 1)
        {
            try
            {
                int l_PAGESIZE = 20;
                int l_currentPage = a_page.GetValueOrDefault(1);

                PagedData<Faturamento> l_pagedData = new PagedData<Faturamento>();

                int l_TotalRecords = a_FaturamentoList.Count<Faturamento>();
                if (l_TotalRecords == 0)
                {
                    l_pagedData.Data = new List<Faturamento>();
                }
                else
                {
                    l_pagedData.Data = (from p in
                                            (from p in a_FaturamentoList orderby p.DataCadastro descending select p)
                                             .Skip<Faturamento>(l_PAGESIZE * (l_currentPage - 1))
                                             .Take<Faturamento>(l_PAGESIZE)
                                             .ToList<Faturamento>()
                                        select p);
                    l_pagedData.Data = FaturamentoHelper.BindClienteToFaturamentos(l_pagedData.Data);
                    l_pagedData.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)l_TotalRecords / 10));
                    l_pagedData.TotalNumberOfRecords = l_TotalRecords;
                    l_pagedData.CurrentPage = l_currentPage;
                }

                return l_pagedData;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        private IQueryable<Faturamento> GetEntityQuery(DateTime? DataEmissaoInicio = null, DateTime? DataEmissaoFim = null, DateTime? DataCadastroInicio = null, DateTime? DataCadastroFim = null, int NumeroNF = 0, int NumeroRPS = 0, int StatusNF = 0, int ClienteID = 0, int LoteEmissaoPMSP = 0)
        {
            try
            {
                //int l_PAGESIZE = 20;
                //int l_currentPage = a_page.GetValueOrDefault(1);
                //PagedData<Faturamento> l_pagedData = new PagedData<Faturamento>();
                IQueryable<Faturamento> all = m_faturamentoRepository.All;


                if (ClienteID != 0)
                {
                    all = all.Where(p => p.ClienteID == ClienteID);
                }
                if ((DataEmissaoInicio != null && DataEmissaoFim != null) && (DataEmissaoInicio != DateTime.MinValue && DataEmissaoFim != DateTime.MinValue))
                {
                    all = all.Where(p => p.DataEmissao >= DataEmissaoInicio && p.DataEmissao <= DataEmissaoFim);
                }
                if ((DataCadastroInicio != null && DataCadastroFim != null) && (DataCadastroInicio != DateTime.MinValue && DataCadastroFim != DateTime.MinValue))
                {
                    all = all.Where(p => p.DataCadastro >= DataCadastroInicio && p.DataCadastro <= DataCadastroFim);
                }
                if (NumeroNF != 0)
                {
                    all = all.Where(p => p.NumeroNF == NumeroNF);
                }
                if (NumeroRPS != 0)
                {
                    all = all.Where(p => p.NumeroRPS == NumeroRPS);
                }
                switch (StatusNF)
                {
                    case 1: //NF Emitida
                        all = all.Where(p => p.NumeroNF != null && p.NumeroNF != 0);
                        break;
                    case 2: //NF Aguardando Emissão
                        all = all.Where(p => p.NumeroNF == null || p.NumeroNF == 0);
                        break;
                    case 3: //NF Cancelada
                        all = all.Where(p => p.Cancelada == "S");
                        break;
                    case 4: //NF Cancelada antes da emissão
                        all = all.Where(p => p.Cancelada == "S" && (p.NumeroNF == null || p.NumeroNF == 0));
                        break;
                }

                if (LoteEmissaoPMSP > 0)
                {
                    all = all.Where(p => p.LoteEmissaoPMSP == LoteEmissaoPMSP);
                }

                return all;

                //int l_TotalRecords = all.Count<Faturamento>();
                //if (l_TotalRecords == 0)
                //{
                //    l_pagedData.Data = new List<Faturamento>();
                //}
                //else
                //{
                //    l_pagedData.Data = (from p in
                //                            (from p in all orderby p.DataCadastro descending select p)
                //                             .Skip<Faturamento>(l_PAGESIZE * (l_currentPage - 1))
                //                             .Take<Faturamento>(l_PAGESIZE)
                //                             .ToList<Faturamento>()
                //                        select p);
                //    l_pagedData.Data = FaturamentoHelper.BindClienteToFaturamentos(l_pagedData.Data);
                //    l_pagedData.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)l_TotalRecords / 10));
                //    l_pagedData.TotalNumberOfRecords = l_TotalRecords;
                //    l_pagedData.CurrentPage = l_currentPage;
                //}

                //return l_pagedData;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        /*
        public ActionResult Excluir(int id)
        {
            try
            {
                Faturamento lobjFatExcluir = m_faturamentoRepository.Find(id);
                if (lobjFatExcluir == null)
                    throw new Exception("Não foi encontrado um faturamento com o ID " + id);
                else
                {
                    if (lobjFatExcluir.NumeroNF > 0)
                    {
                        NFSePmspSender lobjCancNF = new NFSePmspSender();
                        EnvioParamListaFaturamentos lobjNfCanc = new EnvioParamListaFaturamentos(enmOperacaoNfsePmsp.eCancelamentoNF);
                        lobjNfCanc.Faturamentos.Add(lobjFatExcluir);
                        lobjCancNF.iniciaEnvio(lobjNfCanc, lobjFatExcluir.EmpresaEmissao, 0);

                        if (lobjCancNF.Sucesso == false)
                        {
                            string lstrErros = string.Empty;

                            for (int i = 0; i < lobjCancNF.Erros.Count; i++)
                            {
                                lstrErros += "[" + (i + 1) + "] - " + lobjCancNF.Erros[i] + "<br/>";
                            }

                            LayoutMessageHelper.SetMessage("Ocorreram os seguintes erros ao tentar cancelar a nota fiscal " + lobjFatExcluir.NumeroNF + " : " + lstrErros, LayoutMessageType.Success); 
                        }
                        else
                        {
                            LayoutMessageHelper.SetMessage("Nota fiscal " + lobjFatExcluir.NumeroNF + " cancelada com sucesso", LayoutMessageType.Success);
                        }

                        return Index();
                    }
                    else
                    {
                        m_faturamentoRepository.Delete(id);
                    }

                    m_faturamentoRepository.Save();

                }

                return Index();

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }
        */

        //Sobrecarga para receber como parâmetro os dados de Contas a Receber
        public ActionResult CreateByContasAPagar(int id)
        {
            try
            {
                //Busca na tabela de Contas a Receber os dados necessários para abrir a tela de emissão de NF
                int lintIDCliente = 0;
                int lintIDOSSB = 0;
                int lintIDEmpresaEmissora = 0;
                decimal ldecValorOutrosNotaFiscal = 0;
                decimal ldecValorMaoDeObraNotaFiscal = 0;

                return Create(lintIDOSSB, lintIDCliente, lintIDEmpresaEmissora, ldecValorOutrosNotaFiscal, ldecValorMaoDeObraNotaFiscal);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        [HttpGet]
        public ActionResult Create(int IDOSSB, int IDCliente, int IDEmpresaEmissora, decimal ValorServico, decimal ValorMaoDeObra)
        {
            try
            {
                FaturamentoCrudViewModel lobjNFViewModel = GetFaturamentoCreateViewModel(IDOSSB, IDCliente, IDEmpresaEmissora, ValorServico, ValorMaoDeObra);

                return View("Create", lobjNFViewModel);
            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage("Erro ao carregar dados para emissão da Nota Fiscal" + "<br>" + "Erro:" + Error.ToString(), LayoutMessageType.Error);
                return View("Index");
            }
        }
        

        [HttpPost]
        public ActionResult Create(Faturamento aobjFaturamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string lstrTipoEmissao = HttpContext.Request.Form["OpcaoEmissao"];

                    if (lstrTipoEmissao != "1" && lstrTipoEmissao != "2")
                        throw new Exception("Não foi informado uma forma de emissão válida (agora ou agendada)");

                    aobjFaturamento.DataCadastro = DateTime.Now;
                    if (aobjFaturamento.Retencoes != null)
                    {
                        foreach (var retencao in aobjFaturamento.Retencoes)
                        {
                            if (retencao.TipoImpostoID == Convert.ToInt32(enmTipoImposto.eISSOutroMunicipio))
                            {
                                string lstrCodIbgeTomador = Request.Form["CodIbgeMunicTomador"];
                                MunicipioRepository lobjMunicipiosDB = new MunicipioRepository();
                                Municipio lobjMunic = lobjMunicipiosDB.FindByCodigoIBGE(lstrCodIbgeTomador);
                                lobjMunic.AliquotaIssForaMunicipio = retencao.PercentUsado;
                                lobjMunicipiosDB.InsertOrUpdate(lobjMunic);
                                lobjMunicipiosDB.Save();
                            }

                            retencao.TipoImposto = null;
                            
                        }
                    }

                    aobjFaturamento.ValorBruto = aobjFaturamento.GetValorTotal();
                    aobjFaturamento.ValorLiquido = aobjFaturamento.GetValorLiquido();

                    aobjFaturamento.OSSB = null;
                    aobjFaturamento.Cliente = null;
                    aobjFaturamento.ServicoPMSP = null;

                    m_faturamentoRepository.InsertOrUpdate(aobjFaturamento);
                    m_faturamentoRepository.Save();

                    if (lstrTipoEmissao == "1")
                    {
                        EmitirNotaFiscal(aobjFaturamento.ID);
                        LayoutMessageHelper.SetMessage("Nota fiscal emitida com sucesso.", LayoutMessageType.Success);
                    }
                    else
                    {
                        LayoutMessageHelper.SetMessage("Emissão de nota fiscal agendada com sucesso.", LayoutMessageType.Success);
                    }
                    return RedirectToAction("Index");

                }
                else
                {
                    LayoutMessageHelper.SetMessage("Existem campos obrigatórios inválidos, por favor verifique.", LayoutMessageType.Alert);

                    FaturamentoCrudViewModel lobjNFViewModel = GetFaturamentoViewOrEditViewModel(aobjFaturamento);

                    return View(lobjNFViewModel);
                }

            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return View(GetFaturamentoViewOrEditViewModel(aobjFaturamento));
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            FaturamentoCrudViewModel lobjNFViewModel = new FaturamentoCrudViewModel();
            try
            {
                lobjNFViewModel = GetFaturamentoViewOrEditViewModel(id);

                if (lobjNFViewModel.Faturamento.ID == 0)
                {
                    LayoutMessageHelper.SetMessage("Nota fiscal com código " + id + " não encontrada.", LayoutMessageType.Alert);
                    return RedirectToAction("Index", "NotasFiscais");
                }

                return View(lobjNFViewModel);
            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return View(lobjNFViewModel);
            }
        }

        /// <summary>
        /// Define se usa ambiente de producao ou de teste da nota fiscal. Atualmente eh teste
        /// </summary>
        /// <returns></returns>
        private enmOperacaoNfsePmsp getTipoEnvioNF()
        {
            return enmOperacaoNfsePmsp.eEnvioLote;
        }

        [HttpGet]
        public void EmitirNotaFiscal(int id)
        {
            try
            {
                List<int> linFats = new List<int>();
                linFats.Add(id);
                EmitirNotaFiscalPorListaFaturamento(linFats);
            }
            catch (Exception Erro)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult EmitirNotaFiscalLote()
        {
            try
            {

                List<int> linFats = new List<int>();

                string lobjSels = Request.Form["checkbox-emitir"];
                string[] lstrCodigosSelecionados = lobjSels.Split(',');
                foreach (string cod in lstrCodigosSelecionados)
                {
                    if (Utils.IsNumeric(cod))
                        linFats.Add(Convert.ToInt32(cod));
                }

                if (linFats.Count == 0)
                    throw new Exception("Não existem faturamentos selecionados para a emissão");

                string lstrLote = EmitirNotaFiscalPorListaFaturamento(linFats);
                LayoutMessageHelper.SetMessage("Notas fiscais emitidas com sucesso.", LayoutMessageType.Success);
                return View("Index", new { Lote = lstrLote });

            }
            catch (Exception Erro)
            {
                LayoutMessageHelper.SetMessage(Erro.Message, LayoutMessageType.Error);
            }
            return View("Index");

        }

        public void EnviarEmailComNF(int idFaturamento, string astrEmailsDestino)
        {
            try
            {
                Faturamento lobjFatNF = m_faturamentoRepository.Find(idFaturamento);
                EmailTemplate lobjTemplate = m_emailTemplateDescricaoRepository.Find(EmailTemplate.enmTemplate.enmEmailNotaFiscalParaCliente);
                List<string> lobjAnexos = new List<string>();

                if (lobjFatNF.CaminhoPdfNf == string.Empty)
                    lobjFatNF.CaminhoPdfNf = lobjFatNF.getCaminhoPdfNf(1);

                lobjAnexos.Add(lobjFatNF.CaminhoPdfNf);

                Utils.EnviarEmail(astrEmailsDestino, lobjTemplate.CorpoTemplate, lobjTemplate.TituloTemplate, lobjTemplate.SenderNameTemplate, lobjAnexos);

                LayoutMessageHelper.SetMessage("E-mail enviado com sucesso", LayoutMessageType.Success);
            }
            catch (Exception Erro)
            {
                LayoutMessageHelper.SetMessage(Erro.ToString(), LayoutMessageType.Error);
            }
        }

        public string EmitirNotaFiscalPorListaFaturamento(List<int> aintFaturamentos)
        {
            
            try
            {
                NFSePmspSender lobjNFE = new NFSePmspSender();
                EnvioParamListaFaturamentos lobjNotasAEmitir = new EnvioParamListaFaturamentos(getTipoEnvioNF());

                lobjNotasAEmitir.Faturamentos = m_faturamentoRepository.AllAsNoTracking.Where(p => aintFaturamentos.Contains(p.ID)).ToList();
                
                FaturamentoHelper.BindClienteToFaturamentos(lobjNotasAEmitir.Faturamentos);

                lobjNFE.iniciaEnvio(lobjNotasAEmitir, lobjNotasAEmitir.Faturamentos[0].EmpresaEmissao, 0);

                if (lobjNFE.Sucesso == false)
                {
                    string lstrErros = string.Empty;

                    for (int i = 0; i < lobjNFE.Erros.Count; i++)
                    {
                        lstrErros += "[" + (i + 1) + "] - " + lobjNFE.Erros[i] + "<br/>";
                    }
                    throw new Exception("Ocorreram o seguintes erros ao tentar emitir a nota: " + lstrErros);
                }

                return lobjNFE.LoteEnvio;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }


        [HttpGet]
        public ActionResult CancelarNota(int idFaturamento)
        {
            Faturamento lobjFaturamento = new Faturamento();
            try
            {

                lobjFaturamento = m_faturamentoRepository.Find(idFaturamento);
                if (lobjFaturamento == null)
                    throw new Exception("Não foi encontrado o faturamento ID " + idFaturamento);

                lobjFaturamento = FaturamentoHelper.BindClienteToFaturamento(lobjFaturamento);

                return View(lobjFaturamento);
            }
            catch (Exception Erro)
            {
                LayoutMessageHelper.SetMessage(Erro.ToString(), LayoutMessageType.Error);
                return View(lobjFaturamento);
            }
        }


        [HttpPost]
        public ActionResult CancelarNota(Faturamento aobjFaturaCanc)
        {
            try
            {
                aobjFaturaCanc = m_faturamentoRepository.AllAsNoTracking.Where(p => p.ID == aobjFaturaCanc.ID).First();
                if (aobjFaturaCanc.NumeroNF > 0)
                {
                    NFSePmspSender lobjNfCanc = new NFSePmspSender();
                    EnvioParamListaFaturamentos lobjNotasCanc = new EnvioParamListaFaturamentos(enmOperacaoNfsePmsp.eCancelamentoNF);
                    lobjNotasCanc.Faturamentos.Add(aobjFaturaCanc);

                    lobjNfCanc.iniciaEnvio(lobjNotasCanc, aobjFaturaCanc.EmpresaEmissao, 1);
                    if (lobjNfCanc.Sucesso)
                        LayoutMessageHelper.SetMessage("Nota fiscal " + aobjFaturaCanc.NumeroNF + " cancelada com sucesso", LayoutMessageType.Success);
                    else
                    {
                        string lstrErros = String.Join("<br>", lobjNfCanc.Erros);
                        LayoutMessageHelper.SetMessage("Ocorreram os seguintes erros ao tentar cancelar a nota " + aobjFaturaCanc.NumeroNF + " : " + lstrErros, LayoutMessageType.Success);
                    }
                }
                else
                {
                    string l_CaminhoPDFContasAReceber = aobjFaturaCanc.CaminhoPDFBoletoContasReceber;
                    m_faturamentoRepository.Delete(aobjFaturaCanc.ID);
                    m_faturamentoRepository.Save();

                    if (!string.IsNullOrEmpty(l_CaminhoPDFContasAReceber))
                    {
                        string l_FullDiskPath = Server.MapPath(l_CaminhoPDFContasAReceber);
                        if (System.IO.File.Exists(l_FullDiskPath))
                        {
                            ATIMO.Helpers.Utils.DeleteFileFromDisk(l_FullDiskPath);
                        }
                    }

                    LayoutMessageHelper.SetMessage("Faturamento excluído com sucesso", LayoutMessageType.Success);
                }
                return View("Index");
            }
            catch (Exception Erro)
            {
                LayoutMessageHelper.SetMessage(Erro.ToString(), LayoutMessageType.Error);
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult Edit(Faturamento aobjFaturamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (aobjFaturamento.Retencoes != null)
                    { 
                        List<FaturamentoRetencoes> l_Retencoes = new List<FaturamentoRetencoes>(aobjFaturamento.Retencoes.Where(p => p.RetemImposto == "S"));
                        foreach (FaturamentoRetencoes retencao in l_Retencoes)
                        {
                            retencao.FaturamentoID = aobjFaturamento.ID;
                        }
                        aobjFaturamento.Retencoes = l_Retencoes;
                    }
                    //foreach (FaturamentoRetencoes r in aobjFaturamento.Retencoes)
                    //{
                    //    if (r.RetemImposto == "N")
                    //    {

                    //    }
                    //}

                    Faturamento lobjFaturamento = m_faturamentoRepository.AllAsNoTracking.Where(p => p.ID == aobjFaturamento.ID).FirstOrDefault();

                    //Campos do Contas a Receber
                    aobjFaturamento.HistoricoContasReceber = lobjFaturamento.HistoricoContasReceber;
                    aobjFaturamento.DataVencimentoContasReceber = lobjFaturamento.DataVencimentoContasReceber;
                    aobjFaturamento.SituacaoPagamentoContasReceberId = lobjFaturamento.SituacaoPagamentoContasReceberId;
                    aobjFaturamento.DataPagamentoContasReceber = lobjFaturamento.DataPagamentoContasReceber;
                    aobjFaturamento.TipoPagamentoContasReceber = lobjFaturamento.TipoPagamentoContasReceber;
                    aobjFaturamento.CaminhoPDFBoletoContasReceber = lobjFaturamento.CaminhoPDFBoletoContasReceber;

                    //Campos do Faturamento
                    aobjFaturamento.DataCadastro = lobjFaturamento.DataCadastro;
                    aobjFaturamento.ValorBruto = aobjFaturamento.GetValorTotal();
                    aobjFaturamento.ValorLiquido = aobjFaturamento.GetValorLiquido();
                    m_faturamentoRepository.InsertOrUpdate(aobjFaturamento);
                    m_faturamentoRepository.Save();

                    string lstrMessage;

                    if (Request.Form["OpcaoEmissao"] == "1")
                    {
                        EmitirNotaFiscal(aobjFaturamento.ID);
                        lstrMessage = "emitida";
                        if (getTipoEnvioNF() == enmOperacaoNfsePmsp.eTesteEnvio)
                            lstrMessage += " (AMBIENTE DE TESTE PMSP. A NF NAO FOI EMITIDA) ";
                    }
                    else
                    {
                        lstrMessage = "salva";
                    }
                    LayoutMessageHelper.SetMessage("Nota fiscal " + lstrMessage + " com sucesso.", LayoutMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    LayoutMessageHelper.SetMessage("Existem campos obrigatórios inválidos, por favor verifique.", LayoutMessageType.Alert);

                    FaturamentoCrudViewModel lobjNFViewModel = GetFaturamentoViewOrEditViewModel(aobjFaturamento);

                    return View(lobjNFViewModel);
                }

            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return View(GetFaturamentoViewOrEditViewModel(aobjFaturamento));
            }
        }

        public ActionResult Index(int? page = 1, string DataEmissaoInicio = null, string DataEmissaoFim = null, string DataCadastroInicio = null, string DataCadastroFim = null, int NumeroNF = 0, int NumeroRPS = 0, int StatusNF = 0, int ClienteID = 0, int Lote = 0, string SubmitType = "")
        {
            try
            {
                DateTime l_DataEmissaoInicio = DateTime.MinValue;
                DateTime l_DataEmissaoFim = DateTime.MinValue;
                DateTime l_DataCadastroInicio = DateTime.MinValue;
                DateTime l_DataCadastroFim = DateTime.MinValue;

                if (!string.IsNullOrEmpty(DataEmissaoInicio) && !string.IsNullOrEmpty(DataEmissaoFim))
                {
                    DateTime.TryParseExact(DataEmissaoInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out l_DataEmissaoInicio);
                    DateTime.TryParseExact(DataEmissaoFim, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out l_DataEmissaoFim);

                    ViewBag.DataEmissaoInicio = DataEmissaoInicio;
                    ViewBag.DataEmissaoFim = DataEmissaoFim;
                }
                else
                {
                    ViewBag.DataEmissaoInicio = "";
                    ViewBag.DataEmissaoFim = "";
                }

                if (!string.IsNullOrEmpty(DataCadastroInicio) && !string.IsNullOrEmpty(DataCadastroFim))
                {
                    DateTime.TryParseExact(DataCadastroInicio, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out l_DataCadastroInicio);
                    DateTime.TryParseExact(DataCadastroFim, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out l_DataCadastroFim);

                    ViewBag.DataCadastroInicio = DataCadastroInicio;
                    ViewBag.DataCadastroFim = DataCadastroFim;
                }
                else
                {
                    ViewBag.DataCadastroInicio = "";
                    ViewBag.DataCadastroFim = "";
                }

                ViewBag.NumeroNF = NumeroNF;
                ViewBag.NumeroRPS = NumeroRPS;
                ViewBag.StatusNF = StatusNF;
                ViewBag.Lote = Lote;

                ViewBag.Cliente = null;

                if (ClienteID != 0)
                {
                    PESSOA l_Cliente = m_DbContext.PESSOA.Where(p => p.ID == ClienteID).FirstOrDefault();
                    if (l_Cliente != null)
                    {
                        ViewBag.Cliente = l_Cliente;
                    }
                }

                IQueryable<Faturamento> l_Query = GetEntityQuery(l_DataEmissaoInicio, l_DataEmissaoFim, l_DataCadastroInicio, l_DataCadastroFim, NumeroNF, NumeroRPS, StatusNF, ClienteID, Lote);

                if (SubmitType == "Export")
                {
                    return ExportToExcel(l_Query);
                }
                else
                {
                    PagedData<Faturamento> l_Data = PagedList(l_Query, page);

                    //PagedData<Faturamento> l_Data = PagedList(page, l_DataEmissaoInicio, l_DataEmissaoFim, l_DataCadastroInicio, l_DataCadastroFim, NumeroNF, NumeroRPS, StatusNF, ClienteID, Lote);

                    return View(l_Data);
                }
            }
            catch (Exception Error)
            {
                LayoutMessageHelper.SetMessage(Error.Message.ToString(), LayoutMessageType.Error);
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetServicosEmpresa(int IDEmpresaEmissora)
        {
            try
            {
                EmpresaServicoPMSPRepository lobjEmpServDB = new EmpresaServicoPMSPRepository();
                var l_List = lobjEmpServDB.All.Where(p => p.EmpresaCodigo == IDEmpresaEmissora).Select(p => new
                {
                    Codigo = p.ServicoPMSP.Codigo,
                    Descricao = p.ServicoPMSP.Descricao
                });

                return Json(l_List, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
        

        [HttpGet]
        public ActionResult GetImpostosRetidos(int IDEmpresaEmissora, int IDCliente, string ValorMaoDeObra, string ValorOutrosServicos, string CodigoServico, int IDFaturamento = 0)
        {
            try
            {
                if (Utils.IsNumeric(ValorMaoDeObra) == false)
                    ValorMaoDeObra = "0";

                if (Utils.IsNumeric(ValorOutrosServicos) == false)
                    ValorOutrosServicos = "0";
                PESSOA lobjCliente = m_DbContext.PESSOA.Where(p => p.ID == IDCliente).FirstOrDefault();
                Empresa lobjEmpresaEmissora = m_empresaRepository.All.Where(p => p.ID == IDEmpresaEmissora).FirstOrDefault();
                lobjEmpresaEmissora.CodigoServicoPMSP = CodigoServico;
                decimal ldecValorMaoDeObra = decimal.Parse(ValorMaoDeObra.Replace(".", ","));
                decimal ldecValorOutrosServicos = decimal.Parse(ValorOutrosServicos.Replace(".", ","));

                FaturamentoRetencoesTableViewModel lobjFaturamentoRetencoesViewModel = new FaturamentoRetencoesTableViewModel();
                string lstrTempMsgRet;
                PessoaTomador lobjTomador = new PessoaTomador(lobjCliente);

                lobjFaturamentoRetencoesViewModel.FaturamentoRetencoes = FaturamentoRetencoes.getRetencoes(ldecValorOutrosServicos + ldecValorMaoDeObra, ldecValorMaoDeObra, lobjEmpresaEmissora, ref lobjTomador, out lstrTempMsgRet);
                foreach (var retencao in lobjFaturamentoRetencoesViewModel.FaturamentoRetencoes)
                {
                    retencao.FaturamentoID = IDFaturamento;
                }
                lobjFaturamentoRetencoesViewModel.TotalRetencoes = lobjFaturamentoRetencoesViewModel.FaturamentoRetencoes.Where(r => r.RetemImposto == "S").Sum(p => p.ValorRetencao);
                lobjFaturamentoRetencoesViewModel.MensagemRetencoes = lstrTempMsgRet;
                lobjFaturamentoRetencoesViewModel.ValorLiquido = (ldecValorMaoDeObra + ldecValorOutrosServicos) - lobjFaturamentoRetencoesViewModel.TotalRetencoes;
                lobjFaturamentoRetencoesViewModel.ValorTotalNota = (ldecValorMaoDeObra + ldecValorOutrosServicos);
                

                return PartialView("_TabelaImpostosRetidos", lobjFaturamentoRetencoesViewModel);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public JsonResult CalculaImposto()
        {
            try
            {
                return Json(null);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public JsonResult GetFaturamentoDescricoes()
        {
            try
            {
                var l_List = m_faturamentoDescricaoRepository.All.Select(p => new
                {
                    Titulo = p.TituloDescricao,
                    Descricao = p.Descricao,
                    DescricaoResumida = ((p.Descricao.Length > 150) ? p.Descricao.Substring(0, 147) + "..." : p.Descricao),
                    QuandoUtilizar = p.QuandoUtilizar,
                    Id = p.ID
                });

                return Json(l_List, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public JsonResult SaveFaturamentoDescricao(FaturamentoDescricao p_Model)
        {
            try
            {
                if (Session.Usuario() != null)
                    p_Model.PessoaCadastroID = Session.Usuario().ID;

                p_Model.DataCadastro = DateTime.Now;
                
                m_faturamentoDescricaoRepository.InsertOrUpdate(p_Model);
                m_faturamentoDescricaoRepository.Save();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public JsonResult DeleteFaturamentoDescricao(int p_Id)
        {
            try
            {
                m_faturamentoDescricaoRepository.Delete(p_Id);
                m_faturamentoDescricaoRepository.Save();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ID do Faturamento</param>
        /// <returns></returns>
        public ActionResult ContasReceber(int id)
        {
            try
            {
                FaturamentoCrudViewModel l_Faturamento = GetFaturamentoViewOrEditViewModel(id);
                if (l_Faturamento == null)
                {
                    LayoutMessageHelper.SetMessage("Nota Fiscal com id " + id + " não encontrada", LayoutMessageType.Alert);
                    return RedirectToAction("Index");
                }
                else {

                    
                    ViewBag.ListaSituacaoPagamento = m_situacaoPagamentoRepository.All.ToList();
                    return View(l_Faturamento);
                }
                
            }
            catch (Exception Error)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ContasReceber(Faturamento aobjFaturamento)
        {
            try
            {
                
                Faturamento l_Faturamento = m_faturamentoRepository.FindAsNoTracking(aobjFaturamento.ID);
                                
                if (l_Faturamento != null)
                {

                    ViewBag.ListaSituacaoPagamento = m_situacaoPagamentoRepository.All.ToList();

                    l_Faturamento.HistoricoContasReceber = aobjFaturamento.HistoricoContasReceber;
                    l_Faturamento.DataVencimentoContasReceber = aobjFaturamento.DataVencimentoContasReceber;
                    l_Faturamento.SituacaoPagamentoContasReceberId = aobjFaturamento.SituacaoPagamentoContasReceberId;
                    l_Faturamento.DataPagamentoContasReceber = aobjFaturamento.DataPagamentoContasReceber;
                    l_Faturamento.TipoPagamentoContasReceber = aobjFaturamento.TipoPagamentoContasReceber;

                    string l_RootApplicatinPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                    string l_FileDiskTempPath = "";

                    if (!string.IsNullOrEmpty(aobjFaturamento.CaminhoPDFBoletoContasReceber))
                    {
                        l_FileDiskTempPath = l_RootApplicatinPath + "\\" + aobjFaturamento.CaminhoPDFBoletoContasReceber.TrimStart('/').Replace('/', '\\');

                        if (aobjFaturamento.CaminhoPDFBoletoContasReceber != l_Faturamento.CaminhoPDFBoletoContasReceber)
                        {
                            if (System.IO.File.Exists(l_FileDiskTempPath))
                            {
                                FileInfo l_FileInfo = new FileInfo(l_FileDiskTempPath);

                                string l_DiskRelativeDefinitivePath = ATIMO.Helpers.Uploader.Uploader.getRelativeFileStoragePath("", "FaturamentoHelper", Convert.ToString(aobjFaturamento.ID));
                                string l_DiskFullDefinitivePath = l_RootApplicatinPath + "\\" + l_DiskRelativeDefinitivePath;

                                string l_FileDiskRelativeDefinitivePath = l_DiskRelativeDefinitivePath.TrimEnd('\\') + "\\" + l_FileInfo.Name;
                                string l_FileDiskFullDefinitivePath = l_DiskFullDefinitivePath.TrimEnd('\\') + "\\" + l_FileInfo.Name;

                                if (!Directory.Exists(l_DiskFullDefinitivePath))
                                    Directory.CreateDirectory(l_DiskFullDefinitivePath);

                                System.IO.File.Copy(l_FileDiskTempPath, l_FileDiskFullDefinitivePath);

                                string l_FileDefinitiveRelativeUrl = "/" + l_FileDiskRelativeDefinitivePath.TrimStart('\\').Replace("\\", "/");

                                if (!string.IsNullOrEmpty(l_Faturamento.CaminhoPDFBoletoContasReceber))
                                {
                                    string l_PreviousFileDiskTempPath = l_RootApplicatinPath + "\\" + l_Faturamento.CaminhoPDFBoletoContasReceber.TrimStart('/').Replace('/', '\\');

                                    if (System.IO.File.Exists(l_PreviousFileDiskTempPath))
                                    {
                                        //Remove do disco
                                        ATIMO.Helpers.Utils.DeleteFileFromDisk(l_PreviousFileDiskTempPath);
                                    }
                                }

                                l_Faturamento.CaminhoPDFBoletoContasReceber = l_FileDefinitiveRelativeUrl;

                                ATIMO.Helpers.Utils.DeleteFileFromDisk(l_FileDiskTempPath);

                            }
                            else
                            {
                                l_Faturamento.CaminhoPDFBoletoContasReceber = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        //Arquivo já existia e foi removido..
                        if (!string.IsNullOrEmpty(l_Faturamento.CaminhoPDFBoletoContasReceber))
                        {
                            l_Faturamento.CaminhoPDFBoletoContasReceber = "";

                            l_FileDiskTempPath = l_RootApplicatinPath + "\\" + l_Faturamento.CaminhoPDFBoletoContasReceber.TrimStart('/').Replace('/', '\\');

                            if (System.IO.File.Exists(l_FileDiskTempPath))
                            {
                                //Remove do disco
                                ATIMO.Helpers.Utils.DeleteFileFromDisk(l_FileDiskTempPath);
                            }
                        }
                    }

                    m_faturamentoRepository.InsertOrUpdate(l_Faturamento);
                    m_faturamentoRepository.Save();

                    LayoutMessageHelper.SetMessage("Dados do Contas a Receber salvos com sucesso!", LayoutMessageType.Success);

                    FaturamentoCrudViewModel lobjNFViewModel = GetFaturamentoViewOrEditViewModel(l_Faturamento);

                    return View(lobjNFViewModel);

                }
                else
                {
                    LayoutMessageHelper.SetMessage("Não foi possível encontrar o faturamento correspondente a esse contas a receber (id:" + aobjFaturamento.ID + "). ", LayoutMessageType.Alert);

                    return RedirectToAction("Index", "NotasFiscais");
                }
            }
            catch (Exception Error)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ID do Faturamento</param>
        /// <returns></returns>
        public FileResult AbrirArquivoContasReceber(int id)
        {
            try
            {
                Faturamento l_Faturamento = m_faturamentoRepository.Find(id);
                if (l_Faturamento != null)
                {
                    string l_FilePath = Server.MapPath(Url.Content(l_Faturamento.CaminhoPDFBoletoContasReceber));
                    return File(l_FilePath, Utils.GetContentType(l_FilePath));
                }
                else
                {
                    throw new HttpException(404, "Arquivo do Contas a Receber do faturamento de código " + id + " não encontrado.");
                }
            }
            catch (Exception Error)
            {
                throw;
            }
        }

        public ActionResult ExportToExcel(IQueryable<Faturamento> p_Query)
        {
            try
            {
                Faturamento l_Fat = new Faturamento();
                
                var l_FaturamentoDataTable = new System.Data.DataTable("Faturamento");
                
                l_FaturamentoDataTable.Columns.Add("ID", typeof(int));
                l_FaturamentoDataTable.Columns.Add("OSSBID", typeof(int));
                l_FaturamentoDataTable.Columns.Add("NumeroNF", typeof(string));
                l_FaturamentoDataTable.Columns.Add("DataCadastro", typeof(string)); 
                l_FaturamentoDataTable.Columns.Add("DataEmissao", typeof(string));
                l_FaturamentoDataTable.Columns.Add("EmpresaEmissao", typeof(string));
                l_FaturamentoDataTable.Columns.Add("Cancelada", typeof(string)); 
                l_FaturamentoDataTable.Columns.Add("Cliente", typeof(string));
                l_FaturamentoDataTable.Columns.Add("ServicoPMSP", typeof(string));
                l_FaturamentoDataTable.Columns.Add("SerieRPS", typeof(string));
                l_FaturamentoDataTable.Columns.Add("ValorServicos", typeof(decimal));
                l_FaturamentoDataTable.Columns.Add("ValorMaoDeObra", typeof(decimal));
                l_FaturamentoDataTable.Columns.Add("ValorTotal", typeof(decimal));
                l_FaturamentoDataTable.Columns.Add("TotalRetencoes", typeof(decimal));
                l_FaturamentoDataTable.Columns.Add("ValorLiquido", typeof(decimal));
                l_FaturamentoDataTable.Columns.Add("TipoPagamentoContasReceber", typeof(string));
                l_FaturamentoDataTable.Columns.Add("SituacaoPagamentoContasReceber", typeof(string));
                l_FaturamentoDataTable.Columns.Add("DataVencimentoContasReceber", typeof(string));
                l_FaturamentoDataTable.Columns.Add("DataPagamentoContasReceber", typeof(string));
                l_FaturamentoDataTable.Columns.Add("BoletoCadastrado", typeof(string));

                List<Faturamento> l_FaturamentoList = FaturamentoHelper.BindClienteToFaturamentos(p_Query).ToList();

                foreach (var faturamento in l_FaturamentoList)
                {
                    DataRow l_DR = l_FaturamentoDataTable.NewRow();
                    l_DR[0] = faturamento.ID;
                    l_DR[1] = faturamento.OSSBID;
                    l_DR[2] = (faturamento.NumeroNF == null) ? "" : Convert.ToString(faturamento.NumeroNF);
                    l_DR[3] = faturamento.DataCadastro.ToString("dd/MM/yyyy HH:mm");
                    l_DR[4] = (faturamento.DataEmissao.HasValue) ? faturamento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "Não emitida";
                    l_DR[5] = (faturamento.EmpresaEmissao != null) ? faturamento.EmpresaEmissao.Nome : "";
                    l_DR[6] = (faturamento.Cancelada == null) ? "N" : faturamento.Cancelada;
                    l_DR[7] = (faturamento.Cliente != null) ? faturamento.Cliente.NOME : "";
                    l_DR[8] = (faturamento.ServicoPMSP != null) ? faturamento.ServicoPMSP.Descricao : "";
                    l_DR[9] = (faturamento.SerieRPS == null) ? "" : faturamento.SerieRPS;
                    l_DR[10] = faturamento.ValorServico;
                    l_DR[11] = faturamento.ValorMaoDeObra;
                    l_DR[12] = faturamento.GetValorTotal();
                    l_DR[13] = faturamento.GetTotalRetencoes();
                    l_DR[14] = faturamento.GetValorLiquido();
                    l_DR[15] = (faturamento.TipoPagamentoContasReceber == null) ? "" : faturamento.TipoPagamentoContasReceber;
                    l_DR[16] = (faturamento.SituacaoPagamentoContasReceber != null) ? faturamento.SituacaoPagamentoContasReceber.Descricao : "";
                    l_DR[17] = (faturamento.DataVencimentoContasReceber.HasValue) ? faturamento.DataVencimentoContasReceber.Value.ToString("dd/MM/yyyy") : "";
                    l_DR[18] = (faturamento.DataPagamentoContasReceber.HasValue) ? faturamento.DataPagamentoContasReceber.Value.ToString("dd/MM/yyyy") : "";
                    l_DR[19] = (!string.IsNullOrEmpty(faturamento.CaminhoPDFBoletoContasReceber)) ? "BOLETO ANEXADO" : "";

                    //l_FaturamentoDataTable.Rows.Add( faturamento.ID, 
                    //                                 faturamento.OSSBID,
                    //                                 faturamento.NumeroNF,
                    //                                 faturamento.DataCadastro.ToString("dd/MM/yyyy HH:mm"),
                    //                                 (faturamento.DataEmissao.HasValue) ? faturamento.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "Não emitida",
                    //                                 (faturamento.EmpresaEmissao != null) ? faturamento.EmpresaEmissao.Nome : "",
                    //                                 faturamento.Cancelada,
                    //                                 (faturamento.Cliente != null) ? faturamento.Cliente.NOME : "",
                    //                                 (faturamento.ServicoPMSP != null) ? faturamento.ServicoPMSP.Descricao : "",
                    //                                 faturamento.SerieRPS,
                    //                                 faturamento.GetValorTotal(),
                    //                                 faturamento.GetTotalRetencoes(),
                    //                                 faturamento.GetValorLiquido(),
                    //                                 faturamento.TipoPagamentoContasReceber,
                    //                                 (faturamento.SituacaoPagamentoContasReceber != null) ? faturamento.SituacaoPagamentoContasReceber.Descricao : "",
                    //                                 (faturamento.DataVencimentoContasReceber.HasValue) ? faturamento.DataVencimentoContasReceber.Value.ToString("dd/MM/yyyy") : "",
                    //                                 (faturamento.DataPagamentoContasReceber.HasValue) ? faturamento.DataPagamentoContasReceber.Value.ToString("dd/MM/yyyy") : "",
                    //                                 (!string.IsNullOrEmpty(faturamento.CaminhoPDFBoletoContasReceber)) ? "BOLETO ANEXADO" : ""
                    //                                );

                    l_FaturamentoDataTable.Rows.Add(l_DR);
                }
                


                var grid = new GridView();
                grid.DataSource = l_FaturamentoDataTable;
                grid.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Faturamento_" + DateTime.Now.ToString("yyyy_mm_dd_HHmmss")  + ".xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                grid.RenderControl(htw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

                return View("MyView");
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }
    }
}
