using ATIMO.Controllers;
using ATIMO.Models;
using ATIMO.Models.Faturamento;
using Nfse.PMSP.RN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace ATIMO.Helpers.ModelsUtils
{
    public static class FaturamentoHelper
    {
        private static readonly ATIMOEntities _db = new ATIMOEntities();

        private const int TIMEOUT_STOP = 5000;
        private const int TIME_CHECK_IF_IS_TIME_TO_SEND_EMAIL = 300000; // ((1000 * 60) * 5) = Verificar a cada 5 minutos
        private static NotasFiscaisController m_controller;
        private static IEmailSMTPRepository m_emailSMTPRepository = null;
        private static IEmailLogEnvioRepository m_emailLOGRepository = null;
        private static Thread m_threadCheckIfIsTimeToSendEmail;
        private static AutoResetEvent m_autoEventThreadSendEmailFaturamento;

        public static XmlNode getXmlCancelamentoNota(XmlDocument aobjXmlDoc, string astrInscricaoMunicipal, X509Certificate2 aobjCertificado, Faturamento aobjFaturamento)
        {
            string lstrStringAssinatura = Convert.ToInt32(astrInscricaoMunicipal).ToString().PadLeft(8, '0') + aobjFaturamento.NumeroNF.ToString().PadLeft(12, '0');

            XmlNode lobjXmlDetalhe = aobjXmlDoc.CreateElement("Detalhe");
            lobjXmlDetalhe.Attributes.Append(aobjXmlDoc.CreateAttribute("xmlns"));

            XmlNode lobjXmlChaveNFe = aobjXmlDoc.CreateElement("ChaveNFe");

            XmlNode lobjXmlInscricao = aobjXmlDoc.CreateElement("InscricaoPrestador");
            lobjXmlInscricao.InnerText = astrInscricaoMunicipal;
            lobjXmlChaveNFe.AppendChild(lobjXmlInscricao);

            XmlNode lobjNumeroNFe = aobjXmlDoc.CreateElement("NumeroNFe");
            lobjNumeroNFe.InnerText = aobjFaturamento.NumeroNF.ToString();
            lobjXmlChaveNFe.AppendChild(lobjNumeroNFe);

            lobjXmlDetalhe.AppendChild(lobjXmlChaveNFe);

            XmlNode lobjAssinatura = aobjXmlDoc.CreateElement("AssinaturaCancelamento");
            lobjAssinatura.InnerText = Certificado.getAssinatura(lstrStringAssinatura, aobjCertificado);
            lobjXmlDetalhe.AppendChild(lobjAssinatura);

            return lobjXmlDetalhe;

        }

        public static decimal getValorRetencao(enmTipoImposto aenmTipo, Faturamento aobjFaturamento)
        {
            decimal ldecRet = 0;

            if (aobjFaturamento.Retencoes == null)
                aobjFaturamento.Retencoes = new List<FaturamentoRetencoes>();

            foreach (FaturamentoRetencoes retencao in aobjFaturamento.Retencoes)
            {
                if (retencao.TipoImposto.ID == Convert.ToInt32(aenmTipo) || aenmTipo == enmTipoImposto.eNaoDefinido)
                    ldecRet += retencao.ValorRetencao;
            }

            return ldecRet;
        }

        public static decimal getAliquotaRetencao(enmTipoImposto aenmTipo, Faturamento aobjFaturamento)
        {
            decimal ldecRet = 0;

            foreach (FaturamentoRetencoes retencao in aobjFaturamento.Retencoes)
            {
                if (retencao.TipoImposto.ID == Convert.ToInt32(aenmTipo))
                    ldecRet += retencao.PercentUsado;
            }

            return ldecRet;
        }

        public static XmlNode GetXmlRPS(XmlDocument aobjXmlDoc, Empresa aobjEmpresaPrestador, X509Certificate2 aobjCertificado, Faturamento aobjFaturamento)
        {
            try
            {

                string lstrTipoDocto;
                string lstrStringAssinatura;
                string lstrIndicadorTomador;
                string lstrDescrRetencoes = string.Empty;
                string lstrCodIbgeCidadeTomador = string.Empty;
                decimal ldecValorCargaTributaria = 0;

                if (aobjFaturamento.ServicoPMSP == null)
                    aobjFaturamento.ServicoPMSP = new ServicoPMSP();

                aobjFaturamento.ServicoPMSP.Codigo = aobjEmpresaPrestador.ServicoPMSP.Codigo;

                PESSOA l_Cliente = _db.PESSOA.Find(aobjFaturamento.ClienteID);

                if (string.IsNullOrEmpty(l_Cliente.NUM_DOC))
                {
                    if (l_Cliente.ID == 0)
                        throw new Exception("Não foi informado um cliente para a RPS " + aobjFaturamento.NumeroRPS.ToString());
                }

                if (string.IsNullOrEmpty(l_Cliente.CIDADE) == false)
                {
                    lstrCodIbgeCidadeTomador = MunicipioHelper.getCodigoIbgeFromCidade(Utils.removerAcentos(l_Cliente.CIDADE));

                    if (string.IsNullOrEmpty(lstrCodIbgeCidadeTomador))
                        throw new Exception("A cidade '" + l_Cliente.CIDADE + "' não está cadastrada no cadastro de cidades. Por favor verifique se o nome da cidade está correto, e caso esteja, favor consulte ao analista responsável");

                }
                else
                    throw new Exception("É necessário informar uma cidade para o cliente código " + l_Cliente.ID.ToString());

                l_Cliente.NUM_DOC = l_Cliente.NUM_DOC.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

                if (l_Cliente.NUM_DOC.Length == 11)
                {
                    lstrTipoDocto = "CPF";
                    lstrIndicadorTomador = "1";
                }
                else if (l_Cliente.NUM_DOC.Length == 14)
                {
                    lstrTipoDocto = "CNPJ";
                    lstrIndicadorTomador = "2";
                }
                else
                {
                    lstrTipoDocto = "";
                    lstrIndicadorTomador = "3";

                }

                aobjFaturamento.ServicoPMSP.Aliquota = getAliquotaRetencao(enmTipoImposto.eISS, aobjFaturamento) / 100;

                XmlNode lobjXmlRPS = aobjXmlDoc.CreateElement("RPS");
                lobjXmlRPS.Attributes.Append(aobjXmlDoc.CreateAttribute("xmlns"));

                XmlNode lobjXmlAssinatura = aobjXmlDoc.CreateElement("Assinatura");
                XmlNode lobjXmChaveRPS = aobjXmlDoc.CreateElement("ChaveRPS");

                XmlNode lobjXmlInscrPrest = aobjXmlDoc.CreateElement("InscricaoPrestador");
                XmlNode lobjXmlSerieRPS = aobjXmlDoc.CreateElement("SerieRPS");
                XmlNode lobjXmlNumeroRPS = aobjXmlDoc.CreateElement("NumeroRPS");

                lobjXmlInscrPrest.InnerText = aobjEmpresaPrestador.CcmPmsp;
                lobjXmChaveRPS.AppendChild(lobjXmlInscrPrest);
                lobjXmlSerieRPS.InnerText = aobjFaturamento.SerieRPS;
                lobjXmChaveRPS.AppendChild(lobjXmlSerieRPS);
                lobjXmlNumeroRPS.InnerText = aobjFaturamento.NumeroRPS.ToString();
                lobjXmChaveRPS.AppendChild(lobjXmlNumeroRPS);

                XmlNode lobjXmlTipoRPS = aobjXmlDoc.CreateElement("TipoRPS");
                XmlNode lobjXmlDataEmissao = aobjXmlDoc.CreateElement("DataEmissao");
                XmlNode lobjXmlStatusEmissao = aobjXmlDoc.CreateElement("StatusRPS");
                XmlNode lobjXmlTributacao = aobjXmlDoc.CreateElement("TributacaoRPS");
                XmlNode lobjXmlValorServico = aobjXmlDoc.CreateElement("ValorServicos");
                XmlNode lobjXmlValorDeducoes = aobjXmlDoc.CreateElement("ValorDeducoes");
                XmlNode lobjXmlValorPIS = aobjXmlDoc.CreateElement("ValorPIS");
                XmlNode lobjXmlValorCOFINS = aobjXmlDoc.CreateElement("ValorCOFINS");
                XmlNode lobjXmlValorINSS = aobjXmlDoc.CreateElement("ValorINSS");
                XmlNode lobjXmlValorIR = aobjXmlDoc.CreateElement("ValorIR");
                XmlNode lobjXmlValorCSLL = aobjXmlDoc.CreateElement("ValorCSLL");
                XmlNode lobjXmlCodigoServico = aobjXmlDoc.CreateElement("CodigoServico");
                XmlNode lobjXmlAliquotaISS = aobjXmlDoc.CreateElement("AliquotaServicos");
                XmlNode lobjXmlISSRetido = aobjXmlDoc.CreateElement("ISSRetido");
                XmlNode lobjXmlCpfCnpjTomador = null;
                XmlNode lobjNoXmlCcmTomador = aobjXmlDoc.CreateElement("InscricaoMunicipalTomador"); //NAO PREENCHIDO
                XmlNode lobjNoXmlInscrEstadualTomador = aobjXmlDoc.CreateElement("InscricaoEstadualTomador"); //NAO PRENECHIDO
                XmlNode lobjXmlRazaoTomador = aobjXmlDoc.CreateElement("RazaoSocialTomador");
                XmlNode lobjXmlEmail = aobjXmlDoc.CreateElement("EmailTomador");
                XmlNode lobjXmlDiscrimicacaoServ = aobjXmlDoc.CreateElement("Discriminacao");
                XmlNode lobjXmlCargaTribValor = aobjXmlDoc.CreateElement("ValorCargaTributaria");
                XmlNode lobjXmlCargaTribPercent = aobjXmlDoc.CreateElement("PercentualCargaTributaria");
                XmlNode lobjXmlCargaTribFonte = aobjXmlDoc.CreateElement("FonteCargaTributaria");

                if (aobjFaturamento.DataEmissao.HasValue == false)
                    aobjFaturamento.DataEmissao = DateTime.Now.Date;

                lobjXmlTipoRPS.InnerText = "RPS";
                lobjXmlDataEmissao.InnerText = aobjFaturamento.DataEmissao.Value.ToString("yyyy-MM-dd");
                lobjXmlStatusEmissao.InnerText = "N";
                lobjXmlTributacao.InnerText = "T";
                lobjXmlValorServico.InnerText = aobjFaturamento.ValorBruto.ToString("######0.00").Replace(",", ".");
                lobjXmlValorDeducoes.InnerText = getValorRetencao(enmTipoImposto.eNaoDefinido, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlValorPIS.InnerText = getValorRetencao(enmTipoImposto.ePis, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlValorCOFINS.InnerText = getValorRetencao(enmTipoImposto.eCofins, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlValorINSS.InnerText = getValorRetencao(enmTipoImposto.eINSS, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlValorIR.InnerText = getValorRetencao(enmTipoImposto.eImpostodeRenda, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlValorCSLL.InnerText = getValorRetencao(enmTipoImposto.eCSLL, aobjFaturamento).ToString("######0.00").Replace(",", ".");
                lobjXmlCodigoServico.InnerText = Convert.ToInt32(aobjFaturamento.ServicoPMSP.Codigo).ToString("00000");
                lobjXmlAliquotaISS.InnerText = aobjFaturamento.ServicoPMSP.Aliquota.GetValueOrDefault(0).ToString("##0.00").Replace(",", ".");
                lobjXmlISSRetido.InnerText = (getValorRetencao(enmTipoImposto.eISS, aobjFaturamento) > 0) ? "true" : "false";

                if (lstrTipoDocto != string.Empty)
                {
                    lobjXmlCpfCnpjTomador = aobjXmlDoc.CreateElement("CPFCNPJTomador");
                    XmlNode lobjXmlCpfCnpj = aobjXmlDoc.CreateElement(lstrTipoDocto);
                    lobjXmlCpfCnpj.InnerText = l_Cliente.NUM_DOC;
                    lobjXmlCpfCnpjTomador.AppendChild(lobjXmlCpfCnpj);
                }

                lobjNoXmlInscrEstadualTomador.InnerText = Utils.TrataNullString(l_Cliente.INSC_ESTADUAL);
                lobjXmlRazaoTomador.InnerText = l_Cliente.NOME;
                lobjXmlEmail.InnerText = Utils.TrataNullString(l_Cliente.EMAIL);

                XmlNode lobjXmlEndTomador = aobjXmlDoc.CreateElement("EnderecoTomador");
                XmlNode lobjXmlEndTomadorTipoLog = aobjXmlDoc.CreateElement("TipoLogradouro");
                XmlNode lobjXmlEndTomadorLogradouro = aobjXmlDoc.CreateElement("Logradouro");
                lobjXmlEndTomadorLogradouro.InnerText = l_Cliente.ENDERECO;
                XmlNode lobjXmlEndTomadorNumero = aobjXmlDoc.CreateElement("NumeroEndereco");
                XmlNode lobjXmlEndTomadorCompl = aobjXmlDoc.CreateElement("ComplementoEndereco");
                XmlNode lobjXmlEndTomadorBairro = aobjXmlDoc.CreateElement("Bairro");
                XmlNode lobjXmlEndTomadorCidade = aobjXmlDoc.CreateElement("Cidade");
                lobjXmlEndTomadorCidade.InnerText = lstrCodIbgeCidadeTomador;
                XmlNode lobjXmlEndTomadorUF = aobjXmlDoc.CreateElement("UF");
                lobjXmlEndTomadorUF.InnerText = l_Cliente.UF;
                XmlNode lobjXmlEndTomadorCEP = aobjXmlDoc.CreateElement("CEP");
                lobjXmlEndTomadorCEP.InnerText = l_Cliente.CEP.Replace("-", "").Trim();

                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorTipoLog);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorLogradouro);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorNumero);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorCompl);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorBairro);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorCidade);
                lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorUF);
                if (lobjXmlEndTomadorCEP.InnerText != string.Empty)
                    lobjXmlEndTomador.AppendChild(lobjXmlEndTomadorCEP);
                //XmlNode lobjXmlEmail = aobjXmlDoc.CreateElement("EmailTomador");
                //lobjXmlEmail.InnerText = " ";


                lobjXmlDiscrimicacaoServ.InnerText = aobjFaturamento.Descricao;

                /*
                if ((ValorRetencaoIR > 0) || (ValorRetencaoPisCofinsCSLL > 0))
                {
                    lstrDescrRetencoes = lstrDescrRetencoes + "|RETENÇÕES:";

                    if (ValorRetencaoIR > 0)
                        lstrDescrRetencoes = lstrDescrRetencoes + "|\tIMPOSTO DE RENDA:" + FraUtils.Utils.getFormattedValueToDB(ValorRetencaoIR);

                    if (ValorRetencaoPisCofinsCSLL > 0)
                        lstrDescrRetencoes = lstrDescrRetencoes + "|\tPIS/COFINS/CSLL:" + FraUtils.Utils.getFormattedValueToDB(ValorRetencaoPisCofinsCSLL);

                }
                */

                if (aobjEmpresaPrestador.PercentualCargaTributaria > 0)
                {
                    lobjXmlCargaTribFonte.InnerText = aobjEmpresaPrestador.FonteCargaTrib;
                    lobjXmlCargaTribPercent.InnerText = aobjEmpresaPrestador.PercentualCargaTributaria.GetValueOrDefault(0).ToString("##0.00").Replace(",", ".");
                    ldecValorCargaTributaria = Math.Round((aobjFaturamento.ValorBruto * aobjEmpresaPrestador.PercentualCargaTributaria.GetValueOrDefault(0)) / 100, 2);
                    lobjXmlCargaTribValor.InnerText = ldecValorCargaTributaria.ToString("##0.00").Replace(",", ".");
                }

                lstrStringAssinatura = aobjEmpresaPrestador.CcmPmsp.PadLeft(8, '0') + 
                                       aobjFaturamento.SerieRPS.PadRight(5) + 
                                       aobjFaturamento.NumeroRPS.ToString().PadLeft(12, '0') + 
                                       aobjFaturamento.DataEmissao.Value.ToString("yyyyMMdd") + 
                                       lobjXmlTributacao.InnerText + 
                                       lobjXmlStatusEmissao.InnerText + 
                                       ((lobjXmlISSRetido.InnerText) == "true" ? "S" : "N") + 
                                       Convert.ToInt32(aobjFaturamento.ValorBruto * 100).ToString().PadLeft(15, '0') + 
                                       Convert.ToInt32(getValorRetencao(enmTipoImposto.eNaoDefinido, aobjFaturamento) * 100).ToString().PadLeft(15, '0') + 
                                       aobjFaturamento.ServicoPMSP.Codigo.ToString().PadLeft(5, '0') + 
                                       lstrIndicadorTomador + 
                                       l_Cliente.NUM_DOC.PadLeft(14, '0');

                if (lstrStringAssinatura.Length != 86)
                    throw new Exception("A string de assinatura deve ter 86 caracteres, favor verifique o metodo FaturamentoNota.GetXmlRPS");

                lobjXmlAssinatura.InnerText = Certificado.getAssinatura(lstrStringAssinatura, aobjCertificado);

                lobjXmlRPS.AppendChild(lobjXmlAssinatura);
                if (lobjXmChaveRPS != null)
                    lobjXmlRPS.AppendChild(lobjXmChaveRPS);
                if (lobjXmlTipoRPS != null)
                    lobjXmlRPS.AppendChild(lobjXmlTipoRPS);

                lobjXmlRPS.AppendChild(lobjXmlDataEmissao);
                lobjXmlRPS.AppendChild(lobjXmlStatusEmissao);
                lobjXmlRPS.AppendChild(lobjXmlTributacao);
                lobjXmlRPS.AppendChild(lobjXmlValorServico);
                lobjXmlRPS.AppendChild(lobjXmlValorDeducoes);
                lobjXmlRPS.AppendChild(lobjXmlValorPIS);
                lobjXmlRPS.AppendChild(lobjXmlValorCOFINS);
                lobjXmlRPS.AppendChild(lobjXmlValorINSS);
                lobjXmlRPS.AppendChild(lobjXmlValorIR);
                lobjXmlRPS.AppendChild(lobjXmlValorCSLL);
                lobjXmlRPS.AppendChild(lobjXmlCodigoServico);
                lobjXmlRPS.AppendChild(lobjXmlAliquotaISS);
                lobjXmlRPS.AppendChild(lobjXmlISSRetido);
                if (lobjXmlCpfCnpjTomador != null)
                    lobjXmlRPS.AppendChild(lobjXmlCpfCnpjTomador);
                if (lobjNoXmlCcmTomador.InnerText != string.Empty)
                    lobjXmlRPS.AppendChild(lobjNoXmlCcmTomador);
                if (lobjNoXmlInscrEstadualTomador.InnerText != string.Empty)
                    lobjXmlRPS.AppendChild(lobjNoXmlInscrEstadualTomador);
                lobjXmlRPS.AppendChild(lobjXmlRazaoTomador);
                lobjXmlRPS.AppendChild(lobjXmlEndTomador);
                if (lobjXmlEmail.InnerText != string.Empty)
                    lobjXmlRPS.AppendChild(lobjXmlEmail);
                lobjXmlRPS.AppendChild(lobjXmlDiscrimicacaoServ);

                if (ldecValorCargaTributaria > 0)
                {
                    lobjXmlRPS.AppendChild(lobjXmlCargaTribValor);
                    lobjXmlRPS.AppendChild(lobjXmlCargaTribPercent);
                    lobjXmlRPS.AppendChild(lobjXmlCargaTribFonte);
                }
                return lobjXmlRPS;

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }

        public static IEnumerable<Faturamento> BindClienteToFaturamentos(IEnumerable<Faturamento> aobjListaFaturamentos)
        {
            try
            {
                int[] l_IdClientes = aobjListaFaturamentos.Select(p => p.ClienteID).Distinct().ToArray();
                ATIMOEntities l_DbContext = new ATIMOEntities();
                List<PESSOA> l_Clientes = l_DbContext.PESSOA.Where(p => l_IdClientes.Contains(p.ID)).ToList();

                foreach (var lobjFaturamento in aobjListaFaturamentos)
                {
                    if(lobjFaturamento.ClienteID != 0)
                        lobjFaturamento.Cliente = l_Clientes.Where(p => p.ID == lobjFaturamento.ClienteID).FirstOrDefault();
                }

                return aobjListaFaturamentos;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public static Faturamento BindClienteToFaturamento(Faturamento aobjFaturamento)
        {
            try
            {
                IEnumerable<Faturamento> lobjListFat = new List<Faturamento>();
                lobjListFat = lobjListFat.Concat(new[] { aobjFaturamento });
                lobjListFat = BindClienteToFaturamentos(lobjListFat);
                return lobjListFat.First();
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public static void StartSendEmailMonitor()
        {
            if (m_emailSMTPRepository == null)
            {
                m_controller = new NotasFiscaisController();
                m_emailSMTPRepository = new EmailSMTPRepository();
                m_emailLOGRepository = new EmailLogEnvioRepository();
                m_autoEventThreadSendEmailFaturamento = new AutoResetEvent(false);
                ParameterizedThreadStart l_ParamThreadStart = new ParameterizedThreadStart(CheckIfItsTimeToSendEmail);
                m_threadCheckIfIsTimeToSendEmail = new Thread(l_ParamThreadStart);

                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "NotasFiscaisController");
                ControllerContext fakeControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, m_controller);

                m_threadCheckIfIsTimeToSendEmail.Start(fakeControllerContext);
            }
        }

        public static void StopSendEmailMonitor()
        {
            try
            {
                if (m_autoEventThreadSendEmailFaturamento != null)
                {
                    m_autoEventThreadSendEmailFaturamento.Set();
                }

                if (m_threadCheckIfIsTimeToSendEmail.Join(TIMEOUT_STOP) == false)
                {
                    m_threadCheckIfIsTimeToSendEmail.Abort();
                }
            }
            catch (Exception Error)
            {
            }
        }

        private static void CheckIfItsTimeToSendEmail(object x)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(3000);
                    try
                    {
                        List<EmailLogEnvio> lobjEmailsPendentes = m_emailLOGRepository.All.Where(P => P.DataEnviada == null).ToList();

                        if (lobjEmailsPendentes.Count() > 0)
                        {
                            EmailSMTP lobjSMTPConfig = m_emailSMTPRepository.All.FirstOrDefault();
                            SmtpClient lobjSMTP = new SmtpClient(lobjSMTPConfig.EnderecoSMTP, lobjSMTPConfig.Porta);
                            lobjSMTP.Credentials = new NetworkCredential(lobjSMTPConfig.EmailOrigem, lobjSMTPConfig.Senha);
                            lobjSMTP.EnableSsl = false;

                            foreach (EmailLogEnvio email in lobjEmailsPendentes)
                            {
                                MailMessage lobjMM = email.getAsMailMessage();
                                lobjMM.From = new MailAddress(lobjSMTPConfig.EmailOrigem, email.SenderName);
                                try
                                {
                                    lobjSMTP.Send(lobjMM);
                                    email.ErroEnvio = string.Empty;
                                    email.DataEnviada = DateTime.Now;
                                }
                                catch (Exception ErroEnvio)
                                {
                                    email.ErroEnvio = ErroEnvio.Message;
                                    if (email.ErroEnvio.Length > 100)
                                        email.ErroEnvio = email.ErroEnvio.Substring(0, 100);
                                    throw;
                                }
                                m_emailLOGRepository.InsertOrUpdate(email);
                            }

                            m_emailLOGRepository.Save();
                        }
                    }
                    catch (Exception Error)
                    {
                        
                    }

                    /*
                    if (m_autoEventThreadSendEmailFaturamento.WaitOne(TIME_CHECK_IF_IS_TIME_TO_SEND_EMAIL) == true) //Aqui é o tempo que ele espera até a próxima verificação, é tipo um Thread.Sleep(5000)
                    {
                        //O StopMonitor muda o estado desse controle e se ele for chamado, aqui nesse ponto ele sai fora do Loop do While e para de monitorar.
                        //PackedLunchHelper.PackedLunchDailyEmailMonitor.IsSendDailyEmailMonitorRunning = false;
                        break;
                    }
                    */
                }
            }
            catch (Exception Error)
            {
                //PackedLunchHelper.PackedLunchDailyEmailMonitor.IsSendDailyEmailMonitorRunning = false;
                //PackedLunchHelper.PackedLunchDailyEmailMonitor.StatusMessage = "Erro:" + Error.ToString();
                throw Error;
            }
        }


        public static string FileUploadRelativePath()
        {
            string l_FilePath;
            try
            {
                l_FilePath = FileUploadRelativePath("");
            }
            catch (Exception exception)
            {
                throw;
            }
            return l_FilePath;
        }

        public static string FileUploadRelativePath(string a_Param)
        {
            string l_FilePath;
            try
            {
                if (string.IsNullOrEmpty(a_Param))
                    l_FilePath = string.Concat("Temp", "\\");
                else
                {
                    l_FilePath = string.Concat("Faturamento", "\\", DateTime.Now.ToString("yyyy"), "\\", DateTime.Now.ToString("mm"));
                }
                
            }
            catch (Exception exception)
            {
                throw;
            }
            return l_FilePath;
        }

        public static string FileUploadRenameFile(string a_OriginalFileName, string a_Param)
        {
            string l_FileName = string.Empty;
            string l_Extension;
            try
            {
                if (!string.IsNullOrEmpty(a_OriginalFileName))
                {
                    l_FileName = a_OriginalFileName.Substring(0, a_OriginalFileName.LastIndexOf('.'));
                    l_Extension = a_OriginalFileName.Substring(a_OriginalFileName.LastIndexOf('.'));

                    l_FileName = l_FileName.Trim('.');
                    l_Extension = l_Extension.Trim('.');

                    l_FileName = "boleto_" + l_FileName + "_" + DateTime.Now.Ticks.ToString();
                    l_FileName += "." + l_Extension;
                }
            }
            catch (Exception exception)
            {
                //throw exception;
            }
            
            return l_FileName;
        }
    }
}