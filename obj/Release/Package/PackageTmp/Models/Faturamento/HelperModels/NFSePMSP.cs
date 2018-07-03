using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using ATIMO.Models.Faturamento;
using System.Net;
using ATIMO.br.gov.sp.prefeitura.nfe;
using ATIMO.Models;
using ATIMO.Helpers;
using ATIMO.Helpers.ModelsUtils;
using System.Linq;

namespace Nfse.PMSP.RN
{
    public enum enmOperacaoNfsePmsp
    {
        eEnvioLote = 1,
        eCancelamentoNF = 2,
        eConsultaRecebidasPeriodo = 3,
        eConsultaEnviadaPeriodo = 4,
        eTesteEnvio = 5,
        eConsultaCNPJ = 6
    }

    public enum enmTipoEventoEnvio
    {
        eAvisoStatus = 1,
        eErro = 2
    }

    public abstract class IEnvioParam
    {
        protected enmOperacaoNfsePmsp menmOperacao;
        public enmOperacaoNfsePmsp TipoOperacao { get { return menmOperacao; } }
        public List<Faturamento> Faturamentos { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

    }

    public class EnvioParamListaFaturamentos : IEnvioParam
    {
        /// <summary>
        /// Deve ser eEnvioLote, eCancelamentoNF ou eTesteEnvio
        /// </summary>
        /// <param name="aenmOperacao"></param>
        public EnvioParamListaFaturamentos(enmOperacaoNfsePmsp aenmOperacao)
        {
            menmOperacao = aenmOperacao;
            Faturamentos = new List<Faturamento>();
        }
        
        
    }

    public class EnvioParamConsultaCNPJ : IEnvioParam
    {
        public EnvioParamConsultaCNPJ()
        {
            menmOperacao = enmOperacaoNfsePmsp.eConsultaCNPJ;
        }
        
    }

    public class EnvioParamPeriodo : IEnvioParam
    {
        
        public EnvioParamPeriodo(enmOperacaoNfsePmsp aenmOperacao, DateTime adatDataInicial, DateTime adatDataFinal)
        {
            menmOperacao = aenmOperacao;
            DataInicial = adatDataInicial;
            DataFinal = adatDataFinal;
        }

    }

    public class NFSePmspSender
    {

        public delegate void EnvioEvent(DateTime adatDataEvento, enmTipoEventoEnvio enmTipo, string astrMensagem);
        public event EnvioEvent NewEvent;


        private List<Faturamento> mojFaturamentosRetorno = new List<Faturamento>();
        private List<string> mstrErros = new List<string>();
        private Empresa mobjEmpresa = new Empresa();
        private List<string> mstrArquivos = new List<string>();
        private bool mblnSucesso;
        private enmOperacaoNfsePmsp menmOperacao;
        private string mstrInscrCcm = string.Empty;
        private bool mblnEmiteNF = false;

        public enmOperacaoNfsePmsp Operacao
        {
            get { return menmOperacao; }
            set { menmOperacao = value; }
        }


        public List<string> Arquivos
        {
            get { return mstrArquivos; }
        }

        public bool Sucesso
        {
            get { return mblnSucesso; }
        }

        public bool EmiteNF
        {
            get { return mblnEmiteNF; }
        }

        public string InscricaoMunicipalCCM
        {
            get { return mstrInscrCcm; }
        }

        public Empresa Empresa
        {
            get { return mobjEmpresa; }
            set { mobjEmpresa = value; }
        }

        public List<string> Erros
        {
            get { return mstrErros; }
            set { mstrErros = value; }
        }


        public List<Faturamento> FaturamentosRetorno
        {
            get { return mojFaturamentosRetorno; }
            set { mojFaturamentosRetorno = value; }
        }

        public string LoteEnvio { get; private set; }

        private void OnNewEvent(enmTipoEventoEnvio aenmTipo, string astrMensagem)
        {
            if (NewEvent != null)
                NewEvent(DateTime.Now, aenmTipo, astrMensagem);
        }

        public void iniciaEnvio(IEnvioParam aobjParamEnvio, Empresa aobjEmpresa, int aintUsuario)
        {
            //DateTime lobjDataEmissaoRPS;

            try
            {
                LoteNFe lobjLote = new LoteNFe();
                string lstrXmlReturn = string.Empty;
                XmlDocument lobjXmlReturn = new XmlDocument();
                int lintQuantRPS;
                string lstrRetSuccess = string.Empty;
                List<XmlDocument> lobjXmlsDoc;
                string lstrDescrOP;
                //FATURAMENTOEntities m_DbContextFaturamento = new FATURAMENTOEntities();
                IFaturamentoRepository m_FaturamentoRepository = new FaturamentoRepository();
                IEmpresaRepository m_EmpresaRepository = new EmpresaRepository();

                List<int> l_IDsFaturamento = aobjParamEnvio.Faturamentos.Select(p => p.ID).ToList();
                List<Faturamento> l_Faturamentos = m_FaturamentoRepository.All.Where(p => l_IDsFaturamento.Contains(p.ID)).ToList();

                Empresa lobjEmpresa = aobjEmpresa;

                if (lobjEmpresa.ID > 0 && string.IsNullOrEmpty(lobjEmpresa.CcmPmsp))
                    m_EmpresaRepository.Find(aobjEmpresa.ID);

                if (lobjEmpresa == null)
                    throw new Exception("Não foi encontrada uma empresa com o codigo " + aobjEmpresa.ID);

                if (string.IsNullOrEmpty(lobjEmpresa.CNPJ))
                    throw new Exception("A empresa deve vir com o CNPJ preenchido");

                if (string.IsNullOrEmpty(lobjEmpresa.SerialCertificado))
                    throw new Exception("A empresa deve vir com o serial do certificado preenchido");

                if (string.IsNullOrEmpty(lobjEmpresa.CcmPmsp) && (aobjParamEnvio.TipoOperacao != enmOperacaoNfsePmsp.eConsultaCNPJ))
                    throw new Exception("A empresa deve vir com o CCM preenchido");

                
                
                Empresa = lobjEmpresa;
                menmOperacao = aobjParamEnvio.TipoOperacao;

                OnNewEvent(enmTipoEventoEnvio.eAvisoStatus, "Inicio processo.");

                switch (aobjParamEnvio.TipoOperacao)
                {
                    case enmOperacaoNfsePmsp.eEnvioLote:
                    case enmOperacaoNfsePmsp.eCancelamentoNF:
                    case enmOperacaoNfsePmsp.eTesteEnvio:

                        bool lblnSaveRPS = false;

                        if (aobjParamEnvio.TipoOperacao != enmOperacaoNfsePmsp.eCancelamentoNF)
                        {

                            //foreach (Faturamento n in aobjParamEnvio.Faturamentos)
                            foreach (Faturamento n in l_Faturamentos)
                            {
                                n.DataEmissao = DateTime.Now.Date;
                                if (n.NumeroRPS == 0)
                                {
                                    lobjEmpresa.UltimaRPS += 1;
                                    n.NumeroRPS = Convert.ToInt32(lobjEmpresa.UltimaRPS);
                                    n.SerieRPS = lobjEmpresa.SerieRPS;
                                    //m_DbContextFaturamento.Entry<Faturamento>(n).State = System.Data.Entity.EntityState.Modified;
                                    lblnSaveRPS = true;
                                }

                                m_FaturamentoRepository.InsertOrUpdate(n);
                                m_FaturamentoRepository.Save();


                            }

                            if (lblnSaveRPS)
                            {


                                m_EmpresaRepository.InsertOrUpdate(lobjEmpresa);
                                m_EmpresaRepository.Save();

                                //m_DbContextFaturamento.Entry<Empresa>(aobjEmpresa).State = System.Data.Entity.EntityState.Modified;
                                //m_DbContextFaturamento.SaveChanges();
                                //todo: deve salvar o numero de rps no banco de dados neste momento
                                //feito:
                            }
                        }
                        lstrDescrOP = (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eEnvioLote || aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eTesteEnvio) ? "envio de lote de rps" + ((aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eTesteEnvio) ? " (TESTE)" : "") : "cancelamento de nota fiscal";
                        if (lobjEmpresa.ServicoPMSP == null)
                            lobjEmpresa.ServicoPMSP = new ServicoPMSP();

                        if (string.IsNullOrEmpty(lobjEmpresa.ServicoPMSP.Codigo))
                        {
                            if (string.IsNullOrEmpty(lobjEmpresa.CodigoServicoPMSP))
                                throw new Exception("Não foi possível identificar o codigo de servico de São Paulo na propriedade lobjEmpresa.ServicoPMSP.Codigo ou lobjEmpresa.CodigoServicoPMSP do metodo NFSePmspSender.iniciaEnvio");
                            else
                                lobjEmpresa.ServicoPMSP.Codigo = lobjEmpresa.CodigoServicoPMSP;
                        }

                        lobjXmlsDoc = NFSePMSPXML.getXmlPedidoWebServicePMSP(aobjParamEnvio.TipoOperacao, l_Faturamentos, lobjEmpresa, aintUsuario, out lintQuantRPS); //, out lobjDataEmissaoRPS
                        break;
                    case enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo:
                    case enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo:
                        lstrDescrOP = (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo) ? "consulta de notas recebidas no periodo" : "consulta de notas enviadas no periodo";
                        lobjXmlsDoc = NFSePMSPXML.getXmlConsulta(aobjParamEnvio.TipoOperacao, lobjEmpresa, aobjParamEnvio.DataInicial, aobjParamEnvio.DataFinal, 1, aintUsuario); //, out lobjDataEmissaoRPS
                        break;
                    case enmOperacaoNfsePmsp.eConsultaCNPJ:
                        lstrDescrOP = "consulta de CNPJ do certificado";
                        lobjXmlsDoc = NFSePMSPXML.getXmlConsultaCNPJ(lobjEmpresa); //, out lobjDataEmissaoRPS
                        break;
                    default:
                        throw new Exception("O tipo de operação informado não existe, favor verifique");
                }

                OnNewEvent(enmTipoEventoEnvio.eAvisoStatus, "Enviando " + lobjXmlsDoc.Count + " arquivos XML de " + lstrDescrOP + "...");

                lobjLote.Proxy = WebRequest.GetSystemWebProxy();
                lobjLote.ClientCertificates.Add(Certificado.getCertificado(lobjEmpresa.SerialCertificado));

                for (int lintIndexArqXml = 0; lintIndexArqXml < lobjXmlsDoc.Count; lintIndexArqXml++)
                {

                    XmlDocument lobjXmlDoc = lobjXmlsDoc[lintIndexArqXml];
                    string lstrFileSaida = string.Empty;

                    try
                    {
                        lstrFileSaida = Utils.getPathArquivoTemporario(".xml");
                        lobjXmlDoc.Save(lstrFileSaida);
                        Arquivos.Add(Utils.getUrl(lstrFileSaida));
                    }
                    catch (Exception ErroSalvaXmlDisco)
                    {
                        lstrFileSaida = "(Erro ao tentar salvar o arquivo : " + ErroSalvaXmlDisco.Message + ")";
                    }

                    OnNewEvent(enmTipoEventoEnvio.eAvisoStatus, "Arquivo " + (lintIndexArqXml + 1).ToString() + " de " + lobjXmlsDoc.Count.ToString() + " sendo enviado... (" + lstrFileSaida + ")");

                    switch (aobjParamEnvio.TipoOperacao)
                    {
                        case enmOperacaoNfsePmsp.eEnvioLote:
                            lstrXmlReturn = lobjLote.EnvioLoteRPS(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                        case enmOperacaoNfsePmsp.eCancelamentoNF:
                            lstrXmlReturn = lobjLote.CancelamentoNFe(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                        case enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo:
                            lstrXmlReturn = lobjLote.ConsultaNFeRecebidas(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                        case enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo:
                            lstrXmlReturn = lobjLote.ConsultaNFeEmitidas(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                        case enmOperacaoNfsePmsp.eTesteEnvio:
                            lstrXmlReturn = lobjLote.TesteEnvioLoteRPS(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                        case enmOperacaoNfsePmsp.eConsultaCNPJ:
                            lstrXmlReturn = lobjLote.ConsultaCNPJ(NFSePMSPXML.VersaoSchema, lobjXmlDoc.InnerXml);
                            break;
                    }

                    lobjXmlReturn.LoadXml(lstrXmlReturn);

                    try
                    {
                        lstrFileSaida = Utils.getPathArquivoTemporario(".xml");
                        lobjXmlReturn.Save(lstrFileSaida);
                        Arquivos.Add(Utils.getUrl(lstrFileSaida));
                    }
                    catch (Exception ErroSalvarXmlRet)
                    {
                        lstrFileSaida = "(Erro ao tentar salvar o arquivo : " + ErroSalvarXmlRet.Message + ")";
                    }

                    OnNewEvent(enmTipoEventoEnvio.eAvisoStatus, "Arquivo enviado. Processando resposta (arquivo " + lstrFileSaida + ")");

                    XmlNode lobjNoRetorno = lobjXmlReturn.ChildNodes[1];
                    XmlNode lobjNoCabecalho = null;
                    XmlNode lobjNoSucesso = null;

                    if (((lobjNoRetorno.Name == "RetornoEnvioLoteRPS") || (lobjNoRetorno.Name == "RetornoCancelamentoNFe") || (lobjNoRetorno.Name == "RetornoConsultaCNPJ") || (lobjNoRetorno.Name == "RetornoConsulta")) && (lobjNoRetorno.ChildNodes.Count > 0))
                    {
                        lobjNoCabecalho = lobjNoRetorno.ChildNodes[0];

                        if ((lobjNoCabecalho.Name == "Cabecalho") && (lobjNoCabecalho.ChildNodes.Count > 0))
                        {
                            lobjNoSucesso = lobjNoCabecalho.ChildNodes[0];

                            if (lobjNoSucesso.Name == "Sucesso")
                            {
                                lstrRetSuccess = lobjNoSucesso.InnerText;
                            }

                        }

                    }

                    bool lblnSucesso;
                    if (Boolean.TryParse(lstrRetSuccess, out lblnSucesso) == false)
                        throw new Exception("Não foi possível avaliar o arquivo de retorno da prefeitura. favor comunique o analista responsável");

                    mblnSucesso = lblnSucesso;

                    if (Sucesso)
                    {
                        OnNewEvent(enmTipoEventoEnvio.eAvisoStatus, "Arquivo aceito pela prefeitura");

                        string lstrLote;
                        DateTime ldatDataEnvio;
                        if (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eConsultaCNPJ)
                        {
                            NFSePMSPXML.getRetornoConsultaCnpjPMSP(lobjXmlReturn, out mstrInscrCcm, out mblnEmiteNF);
                        }
                        else
                        {
                            FaturamentosRetorno = NFSePMSPXML.getNotasRetornoPMSP(aobjParamEnvio.TipoOperacao, lobjXmlReturn, out lstrLote, out ldatDataEnvio, aobjEmpresa);

                            if (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eCancelamentoNF)
                            {
                                foreach (Faturamento fat_item_canc in l_Faturamentos)
                                {
                                    Faturamento lobjFatCanc = new Faturamento();
                                    lobjFatCanc.NumeroNF = fat_item_canc.NumeroNF;
                                    lobjFatCanc.CodigoVerificacao = fat_item_canc.CodigoVerificacao;
                                    lobjFatCanc.NumeroRPS = fat_item_canc.NumeroRPS;
                                    lobjFatCanc.DataEmissao = fat_item_canc.DataEmissao;
                                    lobjFatCanc.LoteEmissaoPMSP = fat_item_canc.LoteEmissaoPMSP;
                                    lobjFatCanc.EmpresaEmissao = aobjEmpresa;
                                    lobjFatCanc.getCaminhoPdfNf(1);
                                    lobjFatCanc.Cancelada = "S";
                                    FaturamentosRetorno.Add(lobjFatCanc);
                                }
                                
                            }

                            if (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eEnvioLote || aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eCancelamentoNF)
                            {
                                if (aobjParamEnvio.Faturamentos.Count == 0)
                                    throw new Exception("Favor avise o analista responsável que a prefeitura aceitou o arquivo, mas não foi possivel identificar as notas no retorno do web service");

                                foreach (Faturamento fat_enviado in l_Faturamentos)
                                {
                                    Faturamento fat_retorno = FaturamentosRetorno.Find(delegate (Faturamento f) { return f.NumeroRPS == fat_enviado.NumeroRPS; });

                                    if (fat_retorno == null)
                                    {
                                        throw new Exception("Favor avise o analista responsável que a prefeitura aceitou o arquivo, mas não foi encontrada na resposta a nota fiscal referente a RPS " + fat_enviado.NumeroRPS + " (ver arquivos " + Arquivos[0] + " e " + Arquivos[1] + ")");
                                    }
                                    else
                                    {
                                        if (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eCancelamentoNF)
                                        {
                                            fat_enviado.Cancelada = "S";
                                        }
                                        else
                                        {
                                            fat_enviado.NumeroNF = fat_retorno.NumeroNF;
                                            fat_enviado.Cancelada = "N";
                                            fat_enviado.CodigoVerificacao = fat_retorno.CodigoVerificacao;
                                            fat_enviado.DataEmissao = fat_retorno.DataEmissao;
                                            fat_enviado.CaminhoPdfNf = fat_retorno.CaminhoPdfNf;
                                            fat_enviado.LoteEmissaoPMSP = fat_retorno.LoteEmissaoPMSP;
                                        }

                                        m_FaturamentoRepository.InsertOrUpdate(fat_enviado);
                                        m_FaturamentoRepository.Save();
                                    }
                                }

                                LoteEnvio = lstrLote;
                            }
                            else if (aobjParamEnvio.TipoOperacao == enmOperacaoNfsePmsp.eTesteEnvio)
                            {
                                
                            }
                            
                        }

                    }
                    else
                    {
                        OnNewEvent(enmTipoEventoEnvio.eErro, "Arquivo RECUSADO pela prefeitura. Veja os detalhes");
                        //XmlNode lobjXmlErros = lobjNoRetorno.RemoveChild(lobjNoCabecalho);
                        foreach (XmlNode item in lobjNoRetorno.ChildNodes)
                        {
                            if (item.Name == "Erro")
                                mstrErros.Add(item.ChildNodes[1].InnerText);
                        }

                    }
                }

                return;

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }


    }

    public class NFSePMSPXML
    {

        public static int VersaoSchema = 1;

        #region private generic methods

        /// <summary>
        /// Cria um XML Raiz simples (operacoes eConsultaRecebidasPeriodo, eConsultaEnviadaPeriodo e eConsultaCNPJ)
        /// </summary>
        /// <param name="aenmOperacao">Operacao de geracao</param>
        /// <param name="astrNomeNoRaiz">Nome do node pedido</param>
        /// <param name="astrCpfCnpj">CNPJ do prestador</param>
        /// <param name="aobjXmlPedido">xmlnode de pedido</param>
        /// <param name="aobjXmlCabecalho">xmlnode de cabecalho</param>
        /// <returns></returns>
        private static XmlDocument createDocument(enmOperacaoNfsePmsp aenmOperacao, string astrNomeNoRaiz, string astrCpfCnpj, out XmlNode aobjXmlPedido, out XmlNode aobjXmlCabecalho)
        {
            return createDocument(aenmOperacao, astrNomeNoRaiz, astrCpfCnpj, 0, 0, 0, new DateTime(), out aobjXmlPedido, out aobjXmlCabecalho);
        }

        /// <summary>
        /// Cria o XML raiz para as operacoes com transacao (eEnvioLote, eCancelamento e eTesteEnvio)))
        /// </summary>
        /// <param name="aenmOperacao"></param>
        /// <param name="astrNomeNoRaiz"></param>
        /// <param name="astrCpfCnpj"></param>
        /// <param name="aintQuantRPS"></param>
        /// <param name="adecTotalServico"></param>
        /// <param name="adatDataEmissaoRPS"></param>
        /// <param name="aobjXmlPedido"></param>
        /// <param name="aobjXmlCabecalho"></param>
        /// <returns></returns>
        private static XmlDocument createDocument(enmOperacaoNfsePmsp aenmOperacao, string astrNomeNoRaiz, string astrCpfCnpj, int aintQuantRPS, decimal adecTotalServico, decimal adecTotalDeducoes, DateTime adatDataEmissaoRPS, out XmlNode aobjXmlPedido, out XmlNode aobjXmlCabecalho) //
        {
            XmlDocument lobjXmlDoc = new XmlDocument();

            lobjXmlDoc.AppendChild(lobjXmlDoc.CreateXmlDeclaration("1.0", "UTF-8", string.Empty));

            aobjXmlPedido = lobjXmlDoc.CreateElement(astrNomeNoRaiz);

            aobjXmlCabecalho = getCabecalho(aenmOperacao, lobjXmlDoc, astrCpfCnpj, aintQuantRPS, adecTotalServico, adecTotalDeducoes, adatDataEmissaoRPS);

            aobjXmlPedido.AppendChild(aobjXmlCabecalho);

            return lobjXmlDoc;
        }

        /// <summary>
        /// Atribui os esquemas de XML no arquivo e assinala o xml com certificado (chama metodo SignXml)
        /// </summary>
        /// <param name="lobjXmlDoc"></param>
        /// <param name="aobjXmlPedido"></param>
        /// <param name="aobjCertificado"></param>
        private static void finalizaXML(ref XmlDocument lobjXmlDoc, XmlNode aobjXmlPedido, X509Certificate2 aobjCertificado)
        {
            lobjXmlDoc.AppendChild(aobjXmlPedido);
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns", "http://www.prefeitura.sp.gov.br/nfe");

            SignXml(ref lobjXmlDoc, aobjCertificado);
        }

        /// <summary>
        /// Retorna o Xmlnode de cabecalho com cabecalho / versao e CPFCNPJRemetente 
        /// </summary>
        /// <param name="aenmOperacao">Operacao de envio de XML</param>
        /// <param name="aobjXmlDoc">Xmldoc pai deste node</param>
        /// <param name="astrCNPJ">CNPJ do remetente</param>
        /// <returns></returns>
        private static XmlNode getCabecalhoGenerico(enmOperacaoNfsePmsp aenmOperacao, XmlDocument aobjXmlDoc, string astrCNPJ)
        {
            //cabecalho
            string lstrCampo = (astrCNPJ.Length) == 11 ? "CPF" : "CNPJ";
            XmlAttribute lobjAttribute;
            XmlNode lobjXmlCabecalho = aobjXmlDoc.CreateElement("Cabecalho");

            lobjAttribute = aobjXmlDoc.CreateAttribute("Versao");
            lobjAttribute.Value = VersaoSchema.ToString();
            lobjXmlCabecalho.Attributes.Append(lobjAttribute);

            if (aenmOperacao != enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo && aenmOperacao != enmOperacaoNfsePmsp.eConsultaCNPJ)
            {
                lobjAttribute = aobjXmlDoc.CreateAttribute("xmlns");
                lobjAttribute.Value = "";
                lobjXmlCabecalho.Attributes.Append(lobjAttribute);
            }

            XmlNode lobjCnpjRemet = aobjXmlDoc.CreateElement("CPFCNPJRemetente");
            XmlNode lobjCNPJ = aobjXmlDoc.CreateElement(lstrCampo);
            lobjCNPJ.InnerText = astrCNPJ;

            lobjCnpjRemet.AppendChild(lobjCNPJ);
            lobjXmlCabecalho.AppendChild(lobjCnpjRemet);
            return lobjXmlCabecalho;
        }

        /// <summary>
        /// Gera o cabecalho simples (quando operacao nao tem lote)
        /// </summary>
        /// <param name="aenmOperacao"></param>
        /// <param name="aobjXmlDoc"></param>
        /// <param name="astrCNPJ"></param>
        /// <returns></returns>
        private static XmlNode getCabecalho(enmOperacaoNfsePmsp aenmOperacao, XmlDocument aobjXmlDoc, string astrCNPJ)
        {
            if ((aenmOperacao == enmOperacaoNfsePmsp.eEnvioLote) || (aenmOperacao == enmOperacaoNfsePmsp.eCancelamentoNF) || (aenmOperacao == enmOperacaoNfsePmsp.eTesteEnvio))
                throw new Exception("Para gerar cabecalho de eEnvioLote, eCancelamento ou eTesteEnvio, eh necessario informar os parametros aintQuantRPS, adecTotalServicos e adatDataEmissaoRPS.");

            return getCabecalho(aenmOperacao, aobjXmlDoc, astrCNPJ, 0, 0, 0, new DateTime());
        }

        /// <summary>
        /// Gera cabecalho quando operacao de transacao
        /// </summary>
        /// <param name="aenmOperacao"></param>
        /// <param name="aobjXmlDoc"></param>
        /// <param name="astrCNPJ"></param>
        /// <param name="aintQuantRPS"></param>
        /// <param name="adecTotalServicos"></param>
        /// <param name="adatDataEmissaoRPS"></param>
        /// <returns></returns>
        private static XmlNode getCabecalho(enmOperacaoNfsePmsp aenmOperacao, XmlDocument aobjXmlDoc, string astrCNPJ, int aintQuantRPS, decimal adecTotalServicos, decimal adecTotalDeducoes, DateTime adatDataEmissaoRPS)
        {
            try
            {

                XmlNode lobjXmlCabecalho = getCabecalhoGenerico(aenmOperacao, aobjXmlDoc, astrCNPJ);

                if ((aenmOperacao == enmOperacaoNfsePmsp.eEnvioLote) || (aenmOperacao == enmOperacaoNfsePmsp.eCancelamentoNF) || (aenmOperacao == enmOperacaoNfsePmsp.eTesteEnvio))
                {
                    XmlNode lobjTransacao = aobjXmlDoc.CreateElement("transacao");
                    lobjTransacao.InnerText = "true";
                    lobjXmlCabecalho.AppendChild(lobjTransacao);

                    if (aenmOperacao == enmOperacaoNfsePmsp.eEnvioLote || aenmOperacao == enmOperacaoNfsePmsp.eTesteEnvio)
                    {

                        XmlNode lobjDataInicio = aobjXmlDoc.CreateElement("dtInicio");
                        lobjDataInicio.InnerText = adatDataEmissaoRPS.ToString("yyyy-MM-dd");
                        lobjXmlCabecalho.AppendChild(lobjDataInicio);

                        XmlNode lobjDataFim = aobjXmlDoc.CreateElement("dtFim");
                        lobjDataFim.InnerText = adatDataEmissaoRPS.ToString("yyyy-MM-dd");
                        lobjXmlCabecalho.AppendChild(lobjDataFim);

                        XmlNode lobjQuantRPS = aobjXmlDoc.CreateElement("QtdRPS");
                        lobjQuantRPS.InnerText = aintQuantRPS.ToString();
                        lobjXmlCabecalho.AppendChild(lobjQuantRPS);

                        XmlNode lobjValorTotal = aobjXmlDoc.CreateElement("ValorTotalServicos");
                        lobjValorTotal.InnerText = adecTotalServicos.ToString("######0.00").Replace(",", ".");
                        lobjXmlCabecalho.AppendChild(lobjValorTotal);

                        XmlNode lobjValorDeducoes = aobjXmlDoc.CreateElement("ValorTotalDeducoes");
                        lobjValorDeducoes.InnerText = adecTotalDeducoes.ToString("######0.00").Replace(",", ".");
                        lobjXmlCabecalho.AppendChild(lobjValorDeducoes);
                    }
                }

                return lobjXmlCabecalho;

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }

        /// <summary>
        /// Cria a assinatura do XML com o certificado
        /// </summary>
        /// <param name="aobjXmlDoc"></param>
        /// <param name="aobjCertificado"></param>
        private static void SignXml(ref XmlDocument aobjXmlDoc, X509Certificate2 aobjCertificado)
        {

            KeyManager lobjKeyManager = new KeyManager(aobjCertificado);
            SignedXml lobjSignedXml = new SignedXml(aobjXmlDoc);
            Reference lobjReference = new Reference();
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            XmlDsigC14NTransform env2 = new XmlDsigC14NTransform();
            XmlElement lobjXmlDigitalSignature;
            KeyInfo lobjKeyInfo = new KeyInfo();

            //Dim Key As New System.Security.Cryptography.RSACryptoServiceProvider() 

            if (aobjXmlDoc == null)
                throw new ArgumentException("Documento XML não informado");
            if (lobjKeyManager == null)
                throw new ArgumentException("KeyManager não inicializado corretamente");

            lobjSignedXml.SigningKey = lobjKeyManager.KeyProvider;
            lobjKeyInfo.AddClause(new KeyInfoX509Data(aobjCertificado));

            lobjReference.Uri = "";
            lobjReference.AddTransform(env);
            lobjReference.AddTransform(env2);

            lobjSignedXml.KeyInfo = lobjKeyInfo;
            lobjSignedXml.AddReference(lobjReference);

            lobjSignedXml.ComputeSignature();

            lobjXmlDigitalSignature = lobjSignedXml.GetXml();

            aobjXmlDoc.DocumentElement.AppendChild(aobjXmlDoc.ImportNode(lobjXmlDigitalSignature, true));

        }

        #endregion

        #region Geracao de XML

        /// <summary>
        /// Gera o XML de envio de lote de RPS e cancelamento de NF
        /// </summary>
        /// <param name="aenmOperacao"></param>
        /// <param name="aobjRPSs"></param>
        /// <param name="aobjEmpresaEmissao"></param>
        /// <param name="aintUsuario"></param>
        /// <param name="aintQuantNotas"></param>
        /// <returns></returns>
        public static List<XmlDocument> getXmlPedidoWebServicePMSP(enmOperacaoNfsePmsp aenmOperacao, List<Faturamento> aobjRPSs, Empresa aobjEmpresaEmissao, int aintUsuario, out int aintQuantNotas) //, out DateTime adatDataEmissaoRPS
        {
            try
            {
                //string lstrCpfCnpj = string.Empty;
                int lintQuantRPS = 0;
                decimal ldecTotalServico = 0, ldecTotalDeducoes = 0;
                //string lstrCCM = string.Empty;
                XmlDocument lobjXmlDoc = new XmlDocument();
                X509Certificate2 lobjCertificado;
                string lstrNomeNoRaiz;
                int lintMaxNotasArquivo = 30;
                int lintDummy;
                List<XmlDocument> lobjXMLS = new List<XmlDocument>();
                XmlNode lobjXmlPedido = null;
                XmlNode lobjXmlCabecalho = null;
                DateTime lobjDataEmissaoRPS = new DateTime(1, 1, 1);
                //adatDataEmissaoRPS = DateTime.Now;

                switch (aenmOperacao)
                {
                    case enmOperacaoNfsePmsp.eEnvioLote:
                    case enmOperacaoNfsePmsp.eTesteEnvio:
                        lstrNomeNoRaiz = "PedidoEnvioLoteRPS";
                        break;
                    case enmOperacaoNfsePmsp.eCancelamentoNF:
                        lstrNomeNoRaiz = "PedidoCancelamentoNFe";
                        break;
                    default:
                        throw new Exception("Operação não programa");
                }


                if (string.IsNullOrEmpty(aobjEmpresaEmissao.CNPJ))
                    throw new Exception("O cpf/cnpj da empresa que esta emitindo a nota está vazio, favor verifique");
                else if ((aobjEmpresaEmissao.CNPJ.Length != 14) && (aobjEmpresaEmissao.CNPJ.Length != 11))
                    throw new Exception("O CNPJ da empresa que esta emitindo não possui 11 ou 14 caracteres, favor verifique");

                if (string.IsNullOrEmpty(aobjEmpresaEmissao.CcmPmsp))
                    throw new Exception("Favor preencha o campo de inscrição municipal (CCM) da empresa de emissão");

                if (string.IsNullOrEmpty(aobjEmpresaEmissao.SerialCertificado))
                    throw new Exception("Favor preencha o campo serial do certificado da empresa de emissão");

                lintQuantRPS = aobjRPSs.Count;

                foreach (Faturamento n in aobjRPSs)
                {
                    ldecTotalServico += n.ValorBruto;
                    //ldecTotalDeducoes += n.getValorRetencao(enmTipoImposto.eNaoDefinido);
                    ldecTotalDeducoes += FaturamentoHelper.getValorRetencao(enmTipoImposto.eNaoDefinido, n);

                    if (lobjDataEmissaoRPS.Year == 1)
                    {
                        if (n.DataEmissao.HasValue)
                            lobjDataEmissaoRPS = n.DataEmissao.Value;
                        else
                            lobjDataEmissaoRPS = DateTime.Now.Date;
                    }
                }
                
                lobjCertificado = Certificado.getCertificado(aobjEmpresaEmissao.SerialCertificado);

                for (int i = 0; i < aobjRPSs.Count; i++)
                {
                    Math.DivRem(i, lintMaxNotasArquivo, out lintDummy);

                    if (lintDummy == 0)
                    {
                        if (i > 0)
                        {
                            finalizaXML(ref lobjXmlDoc, lobjXmlPedido, lobjCertificado);

                            lobjXMLS.Add(lobjXmlDoc);
                        }

                        lintQuantRPS = 0;
                        ldecTotalServico = 0;
                        ldecTotalDeducoes = 0;

                        while (((lintQuantRPS + i) < (lintMaxNotasArquivo + i)) && (lintQuantRPS + i < aobjRPSs.Count))
                        {
                            ldecTotalServico += aobjRPSs[lintQuantRPS + i].ValorBruto;
                            //ldecTotalDeducoes += aobjRPSs[lintQuantRPS + i].getValorRetencao(enmTipoImposto.eNaoDefinido);
                            ldecTotalDeducoes += FaturamentoHelper.getValorRetencao(enmTipoImposto.eNaoDefinido, aobjRPSs[lintQuantRPS + i]);
                            lintQuantRPS++;
                        }

                        lobjXmlPedido = null;
                        lobjXmlCabecalho = null;
                        lobjXmlDoc = createDocument(aenmOperacao, lstrNomeNoRaiz, aobjEmpresaEmissao.CNPJ, lintQuantRPS, ldecTotalServico, ldecTotalDeducoes, lobjDataEmissaoRPS, out lobjXmlPedido, out lobjXmlCabecalho);
                    }

                    Faturamento lobjRPS = aobjRPSs[i];
                    XmlNode lobjXmlRPS;

                    switch (aenmOperacao)
                    {
                        case enmOperacaoNfsePmsp.eEnvioLote:
                        case enmOperacaoNfsePmsp.eTesteEnvio:
                            lobjXmlRPS = FaturamentoHelper.GetXmlRPS(lobjXmlDoc, aobjEmpresaEmissao, lobjCertificado, lobjRPS);
                            break;
                        case enmOperacaoNfsePmsp.eCancelamentoNF:
                            lobjXmlRPS = FaturamentoHelper.getXmlCancelamentoNota(lobjXmlDoc, aobjEmpresaEmissao.CcmPmsp, lobjCertificado, lobjRPS);
                            break;
                        default:
                            throw new Exception("não tratado");
                    }

                    lobjXmlPedido.AppendChild(lobjXmlRPS);

                }

                finalizaXML(ref lobjXmlDoc, lobjXmlPedido, lobjCertificado);

                lobjXMLS.Add(lobjXmlDoc);

                aintQuantNotas = aobjRPSs.Count;
                return lobjXMLS;

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }

        /// <summary>
        /// Gera o XML completo de consulta de CNPJ
        /// </summary>
        /// <param name="aobjEmpresa">Devem estar preenchidas as propriedades SerialCertificado e CNPJ</param>
        /// <returns></returns>
        public static List<XmlDocument> getXmlConsultaCNPJ(Empresa aobjEmpresa)
        {
            List<XmlDocument> lobjRet = new List<XmlDocument>();
            XmlNode lobjXmlPedido, lobjXmlCabecalho;
            X509Certificate2 lobjCertificado = Certificado.getCertificado(aobjEmpresa.SerialCertificado);
            XmlDocument lobjXmlDoc = createDocument(enmOperacaoNfsePmsp.eConsultaCNPJ, "PedidoConsultaCNPJ", aobjEmpresa.CNPJ, out lobjXmlPedido, out lobjXmlCabecalho);

            XmlNode lobjCnpjRemet = lobjXmlDoc.CreateElement("CNPJContribuinte");
            XmlNode lobjCNPJ = lobjXmlDoc.CreateElement("CNPJ");
            lobjCNPJ.InnerText = aobjEmpresa.CNPJ;
            lobjCnpjRemet.AppendChild(lobjCNPJ);

            lobjXmlPedido.AppendChild(lobjXmlCabecalho);
            lobjXmlPedido.AppendChild(lobjCnpjRemet);
            lobjXmlDoc.AppendChild(lobjXmlPedido);
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:p1", "http://www.prefeitura.sp.gov.br/nfe");

            string lstrTemp = lobjXmlDoc.InnerXml.Replace("PedidoConsultaCNPJ", "p1:PedidoConsultaCNPJ");
            lobjXmlDoc = new XmlDocument();
            lobjXmlDoc.LoadXml(lstrTemp);

            SignXml(ref lobjXmlDoc, lobjCertificado);

            lobjRet.Add(lobjXmlDoc);
            return lobjRet;

        }

        /// <summary>
        /// Gera XML de consulta de notas, recebidas ou enviadas, referente ao CNPJ e Inscrição Municiapal informados
        /// </summary>
        /// <param name="aenmOperacao">Deve ser eConsultaRecebidasPeriodo ou eConsultaEnviadaPeriodo. Nao usar outro valor do enumerador neste metodo</param>
        /// <param name="astrCnpj">Cnpj da operação</param>
        /// <param name="astrInscricao">Inscrição municipal da operação</param>
        /// <param name="adatDataInicio">Data inicial da pesquisa</param>
        /// <param name="adatDataFim">Data final da pesquisa</param>
        /// <param name="aintNumeroPagina">Numero da pagina (ver observação da documentação do web service)</param>
        /// <param name="aintUsuario">Usuário logado no sistema (será usado para identificar o certificado)</param>
        /// <returns>XML para envio à PMSP via WS</returns>
        public static List<XmlDocument> getXmlConsulta(enmOperacaoNfsePmsp aenmOperacao, Empresa aobjEmpresa, DateTime adatDataInicio, DateTime adatDataFim, int aintNumeroPagina, int aintUsuario)
        {
            X509Certificate2 lobjCertificado = Certificado.getCertificado(aobjEmpresa.SerialCertificado);
            XmlNode lobjXmlCabecalho, lobjXmlPedido;
            XmlDocument lobjXmlDoc = createDocument(aenmOperacao, "PedidoConsultaNFePeriodo", aobjEmpresa.CNPJ, out lobjXmlPedido, out lobjXmlCabecalho);
            List<XmlDocument> lobjRet = new List<XmlDocument>();


            aobjEmpresa.CcmPmsp = aobjEmpresa.CcmPmsp.Replace(".", "").Replace("-", "").Replace(" ", "");

            if (aenmOperacao == enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo || aenmOperacao == enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo)
            {
                XmlNode lobjCnpjRemet = lobjXmlDoc.CreateElement("CPFCNPJ");
                XmlNode lobjCNPJ = lobjXmlDoc.CreateElement("CNPJ");
                lobjCNPJ.InnerText = aobjEmpresa.CNPJ;
                lobjCnpjRemet.AppendChild(lobjCNPJ);
                lobjXmlCabecalho.AppendChild(lobjCnpjRemet);

                XmlNode lobjInscricao = lobjXmlDoc.CreateElement("Inscricao");
                lobjInscricao.InnerText = aobjEmpresa.CcmPmsp;
                lobjXmlCabecalho.AppendChild(lobjInscricao);

                XmlNode lobjDataInicio = lobjXmlDoc.CreateElement("dtInicio");
                lobjDataInicio.InnerText = adatDataInicio.ToString("yyyy-MM-dd");
                lobjXmlCabecalho.AppendChild(lobjDataInicio);

                XmlNode lobjDataFim = lobjXmlDoc.CreateElement("dtFim");
                lobjDataFim.InnerText = adatDataFim.ToString("yyyy-MM-dd");
                lobjXmlCabecalho.AppendChild(lobjDataFim);

                XmlNode lobjnumeroPagina = lobjXmlDoc.CreateElement("NumeroPagina");
                lobjnumeroPagina.InnerText = aintNumeroPagina.ToString();
                lobjXmlCabecalho.AppendChild(lobjnumeroPagina);

            }

            lobjXmlPedido.AppendChild(lobjXmlCabecalho);
            lobjXmlDoc.AppendChild(lobjXmlPedido);
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            lobjXmlDoc.DocumentElement.SetAttribute("xmlns:p1", "http://www.prefeitura.sp.gov.br/nfe");

            string lstrTemp = lobjXmlDoc.InnerXml.Replace("PedidoConsultaNFePeriodo", "p1:PedidoConsultaNFePeriodo");
            lobjXmlDoc = new XmlDocument();
            lobjXmlDoc.LoadXml(lstrTemp);

            SignXml(ref lobjXmlDoc, lobjCertificado);

            lobjRet.Add(lobjXmlDoc);

            return lobjRet;
        }

        #endregion 


        /// <summary>
        /// Obtem as notas contidas no XML de retorno
        /// </summary>
        /// <param name="aenmOperacao"></param>
        /// <param name="aobjXmlRetornoPmsp"></param>
        /// <param name="astrOutNumeroLote"></param>
        /// <param name="adatOutDataEnvio"></param>
        /// <returns></returns>
        public static List<Faturamento> getNotasRetornoPMSP(enmOperacaoNfsePmsp aenmOperacao, XmlDocument aobjXmlRetornoPmsp, out string astrOutNumeroLote, out DateTime adatOutDataEnvio, Empresa aobjEmpresa)
        {
            List<Faturamento> lobjNotas = new List<Faturamento>();
            XmlNode lobjNoRaiz = aobjXmlRetornoPmsp.ChildNodes[1];
            astrOutNumeroLote = string.Empty;
            adatOutDataEnvio = new DateTime();

            if (aenmOperacao == enmOperacaoNfsePmsp.eEnvioLote || aenmOperacao == enmOperacaoNfsePmsp.eTesteEnvio)
            {
                astrOutNumeroLote = lobjNoRaiz.SelectSingleNode("Cabecalho/InformacoesLote/NumeroLote").InnerText;
                string lstrDataEnvioLote = lobjNoRaiz.SelectSingleNode("Cabecalho/InformacoesLote/DataEnvioLote").InnerText;
                adatOutDataEnvio = Convert.ToDateTime(lstrDataEnvioLote);

                XmlNodeList lobjNoChaveNfeRps = lobjNoRaiz.SelectNodes("ChaveNFeRPS");

                foreach (XmlNode lobjNoAtual in lobjNoChaveNfeRps)
                {
                    Faturamento lobjNota = new Faturamento();

                    lobjNota.NumeroNF = Convert.ToInt32(lobjNoAtual.SelectSingleNode("ChaveNFe/NumeroNFe").InnerText);
                    lobjNota.CodigoVerificacao = lobjNoAtual.SelectSingleNode("ChaveNFe/CodigoVerificacao").InnerText;
                    lobjNota.NumeroRPS = Convert.ToInt32(lobjNoAtual.SelectSingleNode("ChaveRPS/NumeroRPS").InnerText);
                    lobjNota.DataEmissao = Convert.ToDateTime(lstrDataEnvioLote);
                    lobjNota.LoteEmissaoPMSP = Convert.ToInt32(astrOutNumeroLote);
                    lobjNota.EmpresaEmissao = aobjEmpresa;
                    lobjNota.getCaminhoPdfNf(1);
                    lobjNotas.Add(lobjNota);
                }
            }
            else if (aenmOperacao == enmOperacaoNfsePmsp.eCancelamentoNF)
            {
                
            }
            else if (aenmOperacao == enmOperacaoNfsePmsp.eConsultaEnviadaPeriodo || aenmOperacao == enmOperacaoNfsePmsp.eConsultaRecebidasPeriodo)
            {
                foreach (XmlNode lobjNoAtual in lobjNoRaiz)
                {
                    if (lobjNoAtual.Name == "NFe")
                    {
                        Faturamento lobjNota = new Faturamento();
                        lobjNota.Cliente = new PESSOA();
                        lobjNota.ServicoPMSP = new ServicoPMSP();
                        lobjNota.EmpresaEmissao = aobjEmpresa;


                        lobjNota.EmpresaEmissao.CcmPmsp = lobjNoAtual.SelectSingleNode("ChaveNFe/InscricaoPrestador").InnerText;
                        lobjNota.NumeroNF = Convert.ToInt32(lobjNoAtual.SelectSingleNode("ChaveNFe/NumeroNFe").InnerText);
                        lobjNota.CodigoVerificacao = lobjNoAtual.SelectSingleNode("ChaveNFe/CodigoVerificacao").InnerText;
                        lobjNota.DataEmissao = Convert.ToDateTime(lobjNoAtual.SelectSingleNode("DataEmissaoNFe").InnerText);
                        lobjNota.EmpresaEmissao.CNPJ = lobjNoAtual.SelectSingleNode("CPFCNPJPrestador").InnerText; //TODO: Rever como tratar essa propriedade PessoaCliente - Rafael - 02/01/2017
                        lobjNota.ValorBruto = Convert.ToDecimal(lobjNoAtual.SelectSingleNode("ValorServicos").InnerText.Replace(".", ","));
                        lobjNota.NumDocCliente = lobjNoAtual.SelectSingleNode("CPFCNPJTomador").InnerText; //TODO: Rever como tratar essa propriedade PessoaCliente - Rafael - 02/01/2017
                        lobjNota.Descricao = lobjNoAtual.SelectSingleNode("Discriminacao").InnerText;
                        
                        lobjNota.ServicoPMSP.Codigo = lobjNoAtual.SelectSingleNode("CodigoServico").InnerText;
                        lobjNota.ServicoPMSP.Aliquota = Convert.ToDecimal(lobjNoAtual.SelectSingleNode("AliquotaServicos").InnerText.Replace(".", ","));
                        lobjNota.Cliente.NUM_DOC = Utils.FormataCpfCnpj(lobjNota.NumDocCliente);
                        
                        XmlNode lobjNoEndereco = lobjNoAtual.SelectSingleNode("EnderecoTomador");
                        lobjNota.Cliente.NOME = lobjNoAtual.SelectSingleNode("RazaoSocialTomador").InnerText;
                        lobjNota.Cliente.BAIRRO = lobjNoEndereco.SelectSingleNode("Bairro").InnerText;
                        lobjNota.Cliente.CEP = lobjNoEndereco.SelectSingleNode("CEP").InnerText;
                        if (lobjNota.Cliente.CEP.Length == 8)
                            lobjNota.Cliente.CEP = "0" + lobjNota.Cliente.CEP;
                        lobjNota.Cliente.CIDADE = MunicipioHelper.getNomeCidadeFromCodigoIbge(lobjNoEndereco.SelectSingleNode("Cidade").InnerText);
                        lobjNota.Cliente.UF = lobjNoEndereco.SelectSingleNode("UF").InnerText;
                        lobjNota.Cliente.ENDERECO = lobjNoEndereco.SelectSingleNode("TipoLogradouro").InnerText + " " + lobjNoEndereco.SelectSingleNode("Logradouro").InnerText;
                        if (lobjNoEndereco.SelectSingleNode("NumeroEndereco") != null)
                            lobjNota.Cliente.ENDERECO += " " + lobjNoEndereco.SelectSingleNode("NumeroEndereco").InnerText;

                        if (lobjNoEndereco.SelectSingleNode("ComplementoEndereco") != null)
                            lobjNota.Cliente.ENDERECO += " " + lobjNoEndereco.SelectSingleNode("ComplementoEndereco").InnerText;

                        if (lobjNoEndereco.SelectSingleNode("EmailTomador") != null)
                            lobjNota.Cliente.EMAIL = " " + lobjNoEndereco.SelectSingleNode("EmailTomador").InnerText; 

                        switch (lobjNoAtual.SelectSingleNode("StatusNFe").InnerText)
                        {
                            case "N":
                                lobjNota.Cancelada = "N";
                                break;
                            case "C":
                                lobjNota.Cancelada = "S";
                                break;
                            default:
                                lobjNota.Cancelada = "S";
                                break;
                        }

                        if (lobjNota.EmpresaEmissao.CNPJ != aobjEmpresa.CNPJ || lobjNota.EmpresaEmissao.CcmPmsp != aobjEmpresa.CcmPmsp || aobjEmpresa.ID == 0)
                            throw new Exception("A nota fiscal identificada nao pertence a empresa " + aobjEmpresa.ID);

                        lobjNota.EmpresaEmissaoID = aobjEmpresa.ID;
                        lobjNota.getCaminhoPdfNf(1);

                        lobjNotas.Add(lobjNota);
                    }
                }
            }
            else
                throw new Exception("A operação " + aenmOperacao.ToString() + " não foi tratada no metodo frmEnviaNfsePmsp.getNotasRetornoPMSP");

            return lobjNotas;

        }

        public static bool getRetornoConsultaCnpjPMSP(XmlDocument aobjXmlRetornoPmsp, out string astrOutCCM, out bool ablnOutEmiteNF)
        {
            List<Faturamento> lobjNotas = new List<Faturamento>();
            XmlNode lobjNoRaiz = aobjXmlRetornoPmsp.ChildNodes[1];
            astrOutCCM = string.Empty;
            ablnOutEmiteNF = false;
            foreach (XmlNode y in lobjNoRaiz.ChildNodes)
            {
                foreach (XmlNode x in y.ChildNodes)
                {
                    if (x.Name == "InscricaoMunicipal")
                        astrOutCCM = x.InnerText;
                    if (x.Name == "EmiteNFe")
                        bool.TryParse(x.InnerText, out ablnOutEmiteNF);
                }
            }
            
            return (astrOutCCM != string.Empty);

        }

    }
    
    public class Certificado
    {
        public static string getCurrentSerial(out Empresa aobjEmpresa)
        {
            X509Certificate2 lobjCertificado = new X509Certificate2();
            X509Store lobjCertificadosStore = new X509Store("MY", StoreLocation.CurrentUser);
            string lstrRet = string.Empty;

            lobjCertificadosStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection lobjCertificados = (X509Certificate2Collection)lobjCertificadosStore.Certificates;
            aobjEmpresa = new Empresa();

            foreach (X509Certificate2 certificado in lobjCertificados)
            {
                try
                {
                    if (certificado.PrivateKey != null)
                    {
                        lstrRet = certificado.SerialNumber;
                        string[] lstrPartes = certificado.Subject.Split(',');
                        if (lstrPartes[0].Substring(0, 3) == "CN=")
                        {
                            lstrPartes = lstrPartes[0].Split(':');
                            aobjEmpresa.Nome = lstrPartes[0].Substring(3);
                            if (lstrPartes.Length == 2)
                                aobjEmpresa.CNPJ = lstrPartes[1];
                        }
                        else
                            throw new Exception("Não foi possível identificar o nome da empresa pelo titulo do certificado (Subject deve iniciar por 'CN=')");

                        if (string.IsNullOrEmpty(aobjEmpresa.CNPJ)==false)
                            break;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            
            return lstrRet;

        }

        public static X509Certificate2 getCertificado(string astrSerialNumberCertificado)
        {
            X509Certificate2    lobjCertificado = new X509Certificate2();
            X509Store           lobjCertificadosStore = new X509Store("MY", StoreLocation.CurrentUser);
            bool                lblnCertiValido = false;

            lobjCertificadosStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection lobjCertificados = (X509Certificate2Collection)lobjCertificadosStore.Certificates;

            foreach (X509Certificate2 certificado in lobjCertificados)
            {
                if (certificado.SerialNumber.ToLower() == astrSerialNumberCertificado.ToLower())
                {
                    lobjCertificado = certificado;
                    lblnCertiValido = true;
                    break;
                }
            }

            if (lblnCertiValido == false)
                throw new Exception("Não foi encontrado um certificado com o serial number " + astrSerialNumberCertificado);

            return lobjCertificado;

        }

        public static string getAssinatura(string astrTextToSign, X509Certificate2 aobjCertificado)
        {
            try
            {

                ASCIIEncoding enc = new ASCIIEncoding();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                rsa = aobjCertificado.PrivateKey as RSACryptoServiceProvider;
                byte[] sAssinaturaByte = enc.GetBytes(astrTextToSign);
                byte[] hash = sha1.ComputeHash(sAssinaturaByte);
                sAssinaturaByte = rsa.SignHash(hash, "SHA1");
                string convertido = Convert.ToBase64String(sAssinaturaByte);
                return convertido;

            }
            catch (Exception Erro)
            {
                throw new Exception(Erro.Message + "\nFavor verifique se o cartão inserido é da empresa correta (" + aobjCertificado.Subject + ")");
            }
        }
            
    }

    public sealed class KeyManager
    {
        public static int KeySize = 1024;  // Tamanho da chave

        #region Private Properties
        private RSACryptoServiceProvider m_rsa = null;  // Provider RSA 
        #endregion

        #region Public Properties

        public RSACryptoServiceProvider KeyProvider     // Propriedade que exporta o provider
        {
            get { return m_rsa; }
        }

        #endregion

        #region Constructors

        // Inicializa o Provider com uma chave de tamanho igual à KeySize
        public KeyManager()
        {
            m_rsa = new RSACryptoServiceProvider(KeySize);
        }

        // Inicializa o Provider com um ficheiro
        public KeyManager(string filename)
        {
            m_rsa = new RSACryptoServiceProvider(KeySize);
            _loadKey(m_rsa, filename);
        }

        // Inicializa o Provider com um ficheiro XML de chave public e um ficheiro XML de chave privada
        public KeyManager(string publicKeyFilename, string privateKeyFilename)
        {
            m_rsa = new RSACryptoServiceProvider(KeySize);
            LoadPublicKeyFromXmlFile(publicKeyFilename);
            LoadPrivateKeyFromXmlFile(privateKeyFilename);
        }

        // Inicializa o Provider com um certificado 
        public KeyManager(X509Certificate2 certificate)
        {

            if (certificate == null)
                throw new Exception("certificate not initialized");

            if (certificate.HasPrivateKey)
            {
                try
                {
                    m_rsa = certificate.PrivateKey as RSACryptoServiceProvider;
                }
                catch (Exception Erro)
                {
                    throw new Exception(Erro.Message + "\nPor favor confirme se o cartão está conectado corretamente à máquina");
                }
                
            }
            else
                throw new Exception("certificate does not contains the PrivateKey ");
        }

        #endregion

        #region SAVE
        // Exporta a chave privada para um ficheiro XML
        public void SavePrivateKeyToXmlFile(string filename, bool overwrite)
        {
            _saveKey(m_rsa, filename, overwrite, true);
        }

        // Exporta a chave publica para um ficheiro XML
        public void SavePublicKeyToXmlFile(string filename, bool overwrite)
        {
            _saveKey(m_rsa, filename, overwrite, false);
        }

        private void _saveKey(RSACryptoServiceProvider rsa, string filename, bool overwrite, bool privateKey)
        {
            if (rsa == null)
                throw new Exception("Service Provider NULL Exception");

            if (System.IO.File.Exists(filename) && !overwrite)
                return;

            string Key = rsa.ToXmlString(privateKey);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
            {
                sw.WriteLine(Key);
                sw.Close();
            }
        }

        #endregion

        #region LOAD

        // Carrega uma chave privada de um ficheiro XML
        public void LoadPrivateKeyFromXmlFile(string filename)
        {
            _loadKey(m_rsa, filename);
        }

        // Carrega uma chave publica de um ficheiro XML
        public void LoadPublicKeyFromXmlFile(string filename)
        {
            _loadKey(m_rsa, filename);
        }

        private void _loadKey(RSACryptoServiceProvider rsa, string filename)
        {
            if (rsa == null)
                throw new Exception("Service Provider NULL Exception");

            if (!System.IO.File.Exists(filename))
                throw new System.IO.FileNotFoundException(filename);

            using (System.IO.StreamReader sr = new System.IO.StreamReader(filename, Encoding.UTF8))
            {
                string Key = sr.ReadToEnd();
                sr.Close();
                rsa.FromXmlString(Key);
            }
        }

        #endregion

    }
        
}
