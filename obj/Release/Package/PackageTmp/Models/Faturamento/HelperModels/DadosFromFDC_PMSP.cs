using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ATIMO.Helpers;

namespace ATIMO.Models.Faturamento
{
    public enum enmSituacaoCcm
    {
        eErroNaoIdentificado = 0,
        eCcmEncontrado = 1,
        eCcmCancelado = 4
    }

    public class CadastroPessoaPMSP
    {

        public static int _GlobalCount = 0;
        public static StringBuilder lobjSB = new StringBuilder();
        public static StringBuilder lobjSBLogFavorecidos = new StringBuilder();
        //public static string lstrArqSaida = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\teste.txt";
        public static int CodigoUltimoFornecedor = 0;
        public static int MaximoSolicitacoesConcorrente = 3;
        public static int SolicitacoesConcorrentesAtual = 0;

        public enmSituacaoCcm SituacaoCcm { get; set; }
        public string CcmSite { get; set; }
        public string CpfCnpj { get; set; }
        public string EnderecoSite { get; set; }
        public string CidadeSite { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string RazaoSocial { get; set; }
        public string Telefone { get; set; }
        public List<ServicoPMSP> Servicos { get; set; }
        public string TipoPessoa { get; set; }

        private bool IsSiteFdcOnline()
        {
            return (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.Hour >= 6);
        }
    }

    public class ListResultadoCcm
    {

        private static bool mblnUseWebCliente = false;

        private string mstrCpfCnpj = string.Empty;
        private List<CadastroPessoaPMSP> mobjCadastros = new List<CadastroPessoaPMSP>();
        private string mstrErro = string.Empty;

        public string CpfCnpj { get { return mstrCpfCnpj; } set { mstrCpfCnpj = value; } }
        public int CodigoFavorecido { get; set; }
        public bool FornecedorCadastradoPMSP { get; set; }
        public List<CadastroPessoaPMSP> Cadastros { get { return mobjCadastros; } set { mobjCadastros = value; } }
        public string Erro { get { return mstrErro; } set { mstrErro = value; } }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public int QuantCadastros { get { return Cadastros.Count; } }

        public int QuantCadastrosCancelados
        {
            get
            {
                int lintRet = 0;

                foreach (CadastroPessoaPMSP c in Cadastros)
                {
                    if (c.SituacaoCcm == enmSituacaoCcm.eCcmCancelado)
                        lintRet++;
                }

                return lintRet;
            }

        }

        public int QuantCadastrosOK
        {
            get
            {
                int lintRet = 0;

                foreach (CadastroPessoaPMSP c in Cadastros)
                {
                    if (c.SituacaoCcm == enmSituacaoCcm.eCcmEncontrado)
                        lintRet++;
                }

                return lintRet;
            }

        }

        public int getIndexCadastroOK
        {
            get
            {
                int lintRet = (Cadastros.Count > 0) ? 0 : -1;

                for (int i = 0; i < Cadastros.Count; i++)
                {
                    if (Cadastros[i].SituacaoCcm == enmSituacaoCcm.eCcmEncontrado)
                    {
                        lintRet = i;
                        break;
                    }
                }

                return lintRet;
            }

        }

        public static CadastroPessoaPMSP getDadosFromPMSPByCcm(string astrCcm)
        {
            return getDadosFromPMSPByCcm(astrCcm, 1);
        }

        public static CadastroPessoaPMSP getDadosFromPMSPByCcm(string astrCcm, int aintNumTentativa)
        {
            try
            {
                string lstrUrlPMSP = "https://www3.prefeitura.sp.gov.br/fdc/fdc_imp05_cgc.asp?Ccm_aux=" + astrCcm;
                string lstrRet = getHtmlContent(lstrUrlPMSP, string.Empty, "GET", mblnUseWebCliente);
                return getCadastroFromHtmlFDC(lstrRet);
            }
            catch (Exception Erro)
            {
                if (aintNumTentativa <= 3 && Erro.Message == "tryagain")
                {
                    aintNumTentativa++;
                    return getDadosFromPMSPByCcm(astrCcm, aintNumTentativa);
                }
                else
                    throw;
            }

        }

        public static ListResultadoCcm getDadosFromPMSPByCpfCnpj(string astrCpfCnpj)
        {
            ListResultadoCcm lobjRet = new ListResultadoCcm();
            lobjRet.DataInicio = DateTime.Now;
            string lstrUrlPMSP = "https://www3.prefeitura.sp.gov.br/fdc/fdc_conect_cgc.asp";
            string lstrPostData = "txtCpf_cnpj=" + astrCpfCnpj;
            List<CadastroPessoaPMSP> lobjListaCcmPesquisar = new List<CadastroPessoaPMSP>();
            string lstrRet;

            lstrRet = getHtmlContent(lstrUrlPMSP, lstrPostData, "POST", mblnUseWebCliente);

            if (lstrRet.IndexOf("CCM encontrado:") > 0)
            {
                CadastroPessoaPMSP lobjCadPesq = new CadastroPessoaPMSP();
                string lstrTempCCM = lstrRet.Substring(lstrRet.IndexOf("CCM encontrado:")).Substring(16).Trim();
                lstrTempCCM = lstrTempCCM.Substring(0, lstrTempCCM.IndexOf("-"));
                lstrTempCCM = lstrTempCCM.Replace("\t", "").Replace("<br>", "").Replace("</b>", "").Replace("\r", "").Replace("\n", "").Trim();
                lobjCadPesq.CcmSite = lstrTempCCM;
                lobjListaCcmPesquisar.Add(lobjCadPesq);
                lobjRet.FornecedorCadastradoPMSP = true;
            }
            else if (lstrRet.IndexOf("O CCM/CPF/CNPJ não consta da base de dados do <br>Cadastro de Contribuintes Mobiliários.") > 0)
            {
                lobjRet.FornecedorCadastradoPMSP = false;
            }
            else if (lstrRet.IndexOf("FDC - Ficha de dados cadastrais") > 0)
            {
                lobjRet.Cadastros.Add(getCadastroFromHtmlFDC(lstrRet));
                lobjRet.FornecedorCadastradoPMSP = true;
            }
            else if (lstrRet.IndexOf("Assinale o CCM para o qual deseja emitir a FDC") > 0)
            {
                string[] lstrLinhas = lstrRet.Split('\r');

                foreach (string linha in lstrLinhas)
                {
                    string lstrStrToFind = "INPUT type='radio' name=1 value=";
                    if (linha.IndexOf(lstrStrToFind) > 0)
                    {
                        CadastroPessoaPMSP lobjCadPesq = new CadastroPessoaPMSP();
                        lobjCadPesq.CcmSite = linha.Substring(linha.IndexOf(lstrStrToFind) + lstrStrToFind.Length, 8);
                        if (Utils.IsNumeric(lobjCadPesq.CcmSite))
                            lobjListaCcmPesquisar.Add(lobjCadPesq);
                        else
                            throw new Exception("Não foi possível identificar o CCM na linha aseguir: " + linha);
                    }
                }

            }
            else if (lstrRet.IndexOf("Problemas no acesso ao Cadastro do ISS.") > 0)
                throw new Exception("Erro site: Problemas no acesso ao Cadastro do ISS");
            else
            {
                lobjRet.FornecedorCadastradoPMSP = true;
                //
            }

            if (lobjListaCcmPesquisar.Count > 0)
            {
                foreach (CadastroPessoaPMSP cad in lobjListaCcmPesquisar)
                {
                    if (cad.CcmSite.Length != 8)
                        throw new Exception("O CCM encontrado não corresponde ao padrão (8 digitos), favor avise o analista responsável");
                    else
                    {
                        lobjRet.Cadastros.Add(getDadosFromPMSPByCcm(cad.CcmSite));
                        lobjRet.FornecedorCadastradoPMSP = true;
                    }

                }
            }

            lobjRet.DataFim = DateTime.Now;
            return lobjRet;

        }

        private static string getHtmlContent(string astrUrl, string astrDataParam, string astrMethod, bool ablnUseWebClient)
        {
            string lstrRet;
            WebClient lobjWc = new WebClient();
            WebRequest lobjWR;
            byte[] byteArray = Encoding.ASCII.GetBytes(astrDataParam);
            byte[] responseArray;

            if (ablnUseWebClient)
            {
                //lobjWc.Proxy = WebRequest.GetSystemWebProxy();
                lobjWc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                responseArray = lobjWc.UploadData(astrUrl, "POST", byteArray);
                lstrRet = Encoding.UTF7.GetString(responseArray);
            }
            else
            {

                lobjWR = HttpWebRequest.Create(astrUrl);
                lobjWR.Proxy = new System.Net.WebProxy();
                lobjWR.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                lobjWc.Proxy = lobjWR.Proxy;
                lobjWR.ContentType = "application/x-www-form-urlencoded";
                lobjWR.Timeout = 360000;

                if (astrMethod == "POST")
                {
                    lobjWR.Method = astrMethod;
                    lobjWR.ContentLength = byteArray.Length;
                    Stream lobjStreamReq = lobjWR.GetRequestStream();
                    lobjStreamReq.Write(byteArray, 0, byteArray.Length);
                    lobjStreamReq.Close();
                }
                HttpWebResponse WebResp = (HttpWebResponse)lobjWR.GetResponse();
                StreamReader lobjStreamReader = new StreamReader(WebResp.GetResponseStream(), Encoding.UTF7);
                lstrRet = lobjStreamReader.ReadToEnd();
            }

            return lstrRet;

        }

        private static CadastroPessoaPMSP getCadastroFromHtmlFDC(string astrHtmlFdc)
        {
            try
            {

                CadastroPessoaPMSP lobjRet = new CadastroPessoaPMSP();
                lobjRet.Servicos = new List<ServicoPMSP>();
                bool lblnCcmCancelado = false;
                string lstrFraseCcmCancelado = "A INSCRIÇÃO NO CCM SE ENCONTRA CANCELADA";

                string[] lstrSepTables = { "<table>" };
                string[] lstrSepLinhas = { "<tr>" };
                string[] lstrSepColunas = { "<td>" };
                string lstrNewHtml = astrHtmlFdc.Replace("\n", "").Replace("\r", "");
                lstrNewHtml = lstrNewHtml.Substring(lstrNewHtml.IndexOf("<center>") + 10);
                if (lstrNewHtml.IndexOf("<center>") > 0)
                    lstrNewHtml = lstrNewHtml.Substring(lstrNewHtml.IndexOf("<center>"));
                if (lstrNewHtml.LastIndexOf("<table border='0' width='650'>") > 0)
                    lstrNewHtml = lstrNewHtml.Substring(0, lstrNewHtml.LastIndexOf("<table border='0' width='650'>"));
                else if (lstrNewHtml.LastIndexOf("<table border='0' width='90%'>") > 0)
                    lstrNewHtml = lstrNewHtml.Substring(0, lstrNewHtml.LastIndexOf("<table border='0' width='90%'>"));

                lstrNewHtml = lstrNewHtml.Replace("align='center'", "").Replace("border='0'", "").Replace("size='3'", "").Replace("face='Arial, Verdana, Helvetica, sans-serif'", "").Replace("size='2'", "").Replace("width='2'", "").Replace("align='left'", "").Replace("&nbsp", "").Replace("width='90%'", "").Replace("face='verdana'", "");
                lstrNewHtml = lstrNewHtml.Replace("face=\"Arial, Verdana, Helvetica, sans-serif\"", "");
                while (lstrNewHtml.IndexOf("  ") > 0)
                    lstrNewHtml = lstrNewHtml.Replace("  ", " ");

                lstrNewHtml = lstrNewHtml.Replace(" >", ">").Replace("<b>", "").Replace("</b>", "").Replace("</font>", "").Replace("<br>", "").Replace("<center>", "").Replace("<font>", "");
                lstrNewHtml = lstrNewHtml.Replace("cellpadding='6'", "").Replace("cellspacing='4'", "").Replace("border='1'", "").Replace("colspan = '7'", "").Replace("<!-- <tr> <td> Regime Especial <td> : <td> -->", "");
                lstrNewHtml = lstrNewHtml.Replace("</tr>", "").Replace("</td>", "").Replace("</table>", "").Replace(" >", ">").Replace("\t", "");

                while (lstrNewHtml.IndexOf("  ") > 0)
                    lstrNewHtml = lstrNewHtml.Replace("  ", " ");

                lstrNewHtml = lstrNewHtml.Replace(" >", ">").Replace(" <table>", "<table>");

                string[] lstrTables = lstrNewHtml.Split(lstrSepTables, StringSplitOptions.None);

                /*
                if (lstrTables == null)
                {
                    if (lstrNewHtml.IndexOf("Problemas no acesso ao Cadastro do ISS") > 1 && lstrNewHtml.IndexOf("Por favor, execute novamente a requisição") > 1)
                    {
                        throw new Exception("tryagain");
                    }
                    else
                    {
                        throw new Exception("Erro ao desmembrar as tabelas: Quantidade recebida não bate com quantidade esperada");
                    }
                }
                */

                if (lstrTables.Length == 3 && lstrTables[0] == string.Empty)
                {
                    for (int lintIndexTable = 0; lintIndexTable < lstrTables.Length; lintIndexTable++)
                    {
                        string lstrTable = lstrTables[lintIndexTable];
                        string[] lstrLinhas = lstrTable.Split(lstrSepLinhas, StringSplitOptions.None);

                        foreach (string linha in lstrLinhas)
                        {

                            if (linha.Trim() != string.Empty)
                            {
                                string[] colunas = linha.Split(lstrSepColunas, StringSplitOptions.None);
                                if (colunas.Length > 1)
                                    colunas[1] = colunas[1].Trim();

                                if (colunas.Length == 4 && colunas[0].Trim() == string.Empty && colunas[2].Trim() == ":")
                                {
                                    switch (colunas[1])
                                    {
                                        case "C.C.M.":
                                            lobjRet.CcmSite = colunas[3].Trim();
                                            lobjRet.SituacaoCcm = enmSituacaoCcm.eCcmEncontrado;
                                            lobjRet.CidadeSite = "São Paulo";
                                            break;
                                        case "Pessoa Jurídica":
                                            lobjRet.TipoPessoa = "PJ " + colunas[3].Trim();
                                            break;
                                        case "Contribuinte":
                                            lobjRet.RazaoSocial = colunas[3].Trim();
                                            break;
                                        case "Endereço":
                                            lobjRet.EnderecoSite = colunas[3].Trim();
                                            break;
                                        case "Bairro":
                                            lobjRet.Bairro = colunas[3].Trim();
                                            break;
                                        case "Cep":
                                            lobjRet.Cep = colunas[3].Trim();
                                            break;
                                        case "Telefone":
                                            lobjRet.Telefone = colunas[3].Trim();
                                            break;
                                        case "Data do cancelamento do CCM":
                                            lobjRet.SituacaoCcm = enmSituacaoCcm.eCcmCancelado;
                                            break;
                                        case "CNPJ / CPF":
                                            lobjRet.CpfCnpj = colunas[3].Trim();
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else if (colunas.Length == 8 && colunas[0].Trim() == string.Empty && colunas[7].Trim() == ";")
                                {
                                    string lstrAliquota = colunas[4].Replace("%", "").Replace(" ", "").Replace(";", "");
                                    string lstrData = colunas[2];

                                    if (Utils.IsNumeric(lstrAliquota) == false)
                                        lstrAliquota = "0";

                                    if (Utils.IsDate(lstrData) == false)
                                        lstrData = "01/01/0001";

                                    lobjRet.Servicos.Add(new ServicoPMSP(colunas[1].ToString(), Convert.ToDateTime(lstrData), Convert.ToDecimal(lstrAliquota)));
                                }
                                else if (colunas.Length == 2)
                                {
                                    if (colunas[1] == lstrFraseCcmCancelado)
                                        lblnCcmCancelado = true;
                                }
                            }
                        }

                    }
                }
                else
                    throw new Exception("Erro ao desmembrar as tabelas: Quantidade recebida não bate com quantidade esperada");

                if (lblnCcmCancelado == false && lobjRet.SituacaoCcm == enmSituacaoCcm.eCcmCancelado)
                    throw new Exception("Na leitura da página, foi identificado que o CCM " + lobjRet.CcmSite + " possui uma data de cancelamento, mas na lista de serviços não foi informada a frase '" + lstrFraseCcmCancelado + "'. Favor avise o analista responsavel");
                else if (lblnCcmCancelado && lobjRet.SituacaoCcm != enmSituacaoCcm.eCcmCancelado)
                    throw new Exception("Na leitura da página, foi identificado que o CCM " + lobjRet.CcmSite + " NÃO possui uma data de cancelamento, porém na lista de serviços foi informada a frase '" + lstrFraseCcmCancelado + "'. Favor avise o analista responsavel");
                else if (lobjRet.SituacaoCcm != enmSituacaoCcm.eCcmEncontrado && lobjRet.Servicos.Count > 0)
                    throw new Exception("Na leitura da página, foi identificado que o CCM " + lobjRet.CcmSite + " está cancelado, porém foram encontrados serviços cadastrados. Favor avise o analista responsável");
                //else if (lobjRet.SituacaoCcm == enmSituacaoCcm.eCcmEncontrado && lobjRet.Servicos.Count == 0)
                //    throw new Exception("Na leitura da página, foi identificado que o CCM " + lobjRet.CcmSite + " foi encontrado, porém NÃO foram encontrados serviços cadastrados. Favor avise o analista responsável");

                if (lblnCcmCancelado)
                    lobjRet.CcmSite = string.Empty;

                return lobjRet;

            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }


    }




}
