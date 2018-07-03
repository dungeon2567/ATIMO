using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using ATIMO.Models.Faturamento;
using ATIMO.Models;
using System.IO;
using System.Reflection;
using System.Text;

namespace ATIMO.Helpers
{
    public class Utils
    {

        private readonly static IEmailLogEnvioRepository m_logMailRepository = new EmailLogEnvioRepository();

        //public static PESSOA getPessoaFromReceita_PMSP(string astrCpfCnpj, out string astrOutAlert)
        public static PESSOA getPessoaFromReceita_PMSP(string astrCpfCnpj, ref List<string> alistOutAlert)
        {
            PESSOA lobjPessoaRet = new PESSOA();
            if(alistOutAlert == null)
                alistOutAlert = new List<string>();
            try
            {
                string lstrTipo;
                if (string.IsNullOrEmpty(astrCpfCnpj) == false)
                {
                    astrCpfCnpj = Utils.LimpaCpfCnpj(astrCpfCnpj);
                    switch (astrCpfCnpj.Length)
                    {
                        case 11:
                            lstrTipo = "F";
                            break;
                        case 14:
                            lstrTipo = "J";
                            break;
                        default:
                            throw new Exception("O documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)");
                    }

                    if (lstrTipo == "J")
                    {
                        DadosFromReceitaWs lobjDados = DadosFromReceitaWs.getDados(astrCpfCnpj);

                        if (lobjDados.status != "OK")
                            throw new Exception(lobjDados.message);

                        if (lobjDados.situacao != "ATIVA")
                            alistOutAlert.Add("ESTE CNPJ NÃO ESTÁ ATVO NA RECEITA FEDERAL");

                        lobjPessoaRet.BAIRRO = lobjDados.bairro;
                        lobjPessoaRet.CEP = lobjDados.cep;
                        lobjPessoaRet.CIDADE = lobjDados.municipio;
                        lobjPessoaRet.EMAIL = lobjDados.email;
                        lobjPessoaRet.ENDERECO = lobjDados.getEnderecoCompleto();
                        lobjPessoaRet.NOME = lobjDados.nome;
                        lobjPessoaRet.NUM_DOC = lobjDados.cnpj;
                        lobjPessoaRet.TELEFONE1 = lobjDados.telefone;
                        lobjPessoaRet.UF = lobjDados.uf;
                        lobjPessoaRet.TIPO_DOC = "CNPJ";
                        lobjPessoaRet.NUM_DOC = lobjDados.cnpj;
                    }

                    ListResultadoCcm lobjResultCcm = new ListResultadoCcm();
                    try
                    {
                        lobjResultCcm = ListResultadoCcm.getDadosFromPMSPByCpfCnpj(Utils.LimpaCpfCnpj(astrCpfCnpj));
                    }
                    catch (Exception ErroPmsp)
                    {
                        alistOutAlert.Add("Não foi possível obter os dados do site da prefeitura pelo seguinte motivo : " + ErroPmsp.Message);
                    }
                    
                    if (lobjResultCcm.getIndexCadastroOK > -1)
                    {
                        CadastroPessoaPMSP cad = lobjResultCcm.Cadastros[lobjResultCcm.getIndexCadastroOK];

                        if (lstrTipo == "F")
                        {
                            lobjPessoaRet.BAIRRO = cad.Bairro;
                            lobjPessoaRet.CEP = cad.Cep;
                            lobjPessoaRet.CIDADE = cad.CidadeSite;
                            lobjPessoaRet.ENDERECO = cad.EnderecoSite;
                            lobjPessoaRet.NOME = cad.RazaoSocial;
                            lobjPessoaRet.TELEFONE1 = cad.Telefone;
                            lobjPessoaRet.UF = cad.Bairro;
                            lobjPessoaRet.TIPO_DOC = "CPF";
                            lobjPessoaRet.NUM_DOC = cad.CpfCnpj;
                        }

                        if (cad.SituacaoCcm == enmSituacaoCcm.eCcmEncontrado)
                            lobjPessoaRet.INSC_MUNICIPAL = cad.CcmSite;
                        else if (cad.SituacaoCcm == enmSituacaoCcm.eCcmCancelado)
                            alistOutAlert.Add("CCM CANCELADO NA PREFEITURA DE SAO PAULO");

                        switch (cad.TipoPessoa.Trim())
                        {
                            case "PJ SIMPLES":
                                lobjPessoaRet.TIPO_PESSOA_TRIBUTACAO = (int?)enmTipoPessoaTrib.ePJ_OptanteSimples;
                                break;
                            case "PJ COMUM":
                                lobjPessoaRet.TIPO_PESSOA_TRIBUTACAO = (int?)enmTipoPessoaTrib.ePJ_Comum;
                                break;
                            case "PJ":
                                lobjPessoaRet.TIPO_PESSOA_TRIBUTACAO = (int?)enmTipoPessoaTrib.ePF;
                                break;
                            case "PJ MEI":
                                lobjPessoaRet.TIPO_PESSOA_TRIBUTACAO = (int?)enmTipoPessoaTrib.ePJ_Mei;
                                break;
                            default:
                                break;
                        }

                    }
                    else if (lstrTipo == "F")
                    { 
                        //throw new Exception("Este cadastro não foi encontrado no site da prefeitura de São Paulo pelo CPF informado");
                    }

                }

            }
            catch (Exception Erro)
            {
                throw Erro;
            }

            return lobjPessoaRet;
        }

        public static string getPathArquivoTemporario(string astrExtensao)
        {
            string lstrRet = string.Empty;

            if (astrExtensao.Substring(0, 1) != ".")
                astrExtensao = "." + astrExtensao;

            lstrRet = AppDomain.CurrentDomain.BaseDirectory ;

            if (lstrRet.EndsWith(@"\") == false)
                lstrRet += @"\";

            lstrRet += @"Tmp\" + DateTime.Now.Ticks.ToString() + astrExtensao;
            return lstrRet;
        }

        public static string getUrl(string astrPathFile)
        {
            string lstrRet = astrPathFile;

            if (astrPathFile.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
            {
                lstrRet = astrPathFile.Substring(AppDomain.CurrentDomain.BaseDirectory.Length).Replace(@"\", "/");
                lstrRet = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "") + "/" + lstrRet;
            }

            return lstrRet;
        }

        public static List<KeyValuePair<int, string>> getEnumAsList(Type aenmEnum)
        {
            List<KeyValuePair<int, string>> lobjRet = new List<KeyValuePair<int, string>>();

            string[] lstrNames = Enum.GetNames(aenmEnum);
            
            for (int i = 0; i < lstrNames.Length; i++)
            {
                lobjRet.Add(new KeyValuePair<int, string>(Convert.ToInt32(Enum.Parse(aenmEnum, lstrNames[i])), lstrNames[i]));
            }

            return lobjRet;
        }

        public static string TrataNullString(object astrTexto)
        {
            string lstrRet = string.Empty;

            if (astrTexto != null && astrTexto != Convert.DBNull)   
                lstrRet = astrTexto.ToString();

            return lstrRet;
        }

        public static bool IsNumeric(object aobjValue)
        {
            bool lblnRet = false;
            decimal ldec;

            if (aobjValue != null)
                lblnRet = decimal.TryParse(aobjValue.ToString(), out ldec);

            return lblnRet;
        }

        internal static string LimpaCpfCnpj(string astrCNPJ)
        {
            return astrCNPJ.Replace(".", "").Replace("-", "").Replace(@"/", "").Trim();
        }

        public static string removerAcentos(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return texto;
        }

        public static bool IsDate(object aobjValue)
        {
            bool lblnRet = false;
            DateTime ldec;

            if (aobjValue != null)
                lblnRet = DateTime.TryParse(aobjValue.ToString(), out ldec);

            return lblnRet;
        }

        public static int TrataNullInt32(string astrValue)
        {
            int lintRet = 0;

            Int32.TryParse(astrValue, out lintRet);

            return lintRet;
        }
        
        internal static string FormataCpfCnpj(string astrCNPJ)
        {
            astrCNPJ = LimpaCpfCnpj(astrCNPJ);

            if (astrCNPJ.Length > 1)
            {
                if (astrCNPJ.Length == 15 && astrCNPJ.Substring(0, 1) == "0")
                    astrCNPJ = astrCNPJ.Substring(1);

                if (astrCNPJ.Length == 14)
                    astrCNPJ = astrCNPJ.Substring(0, 2) + "." + astrCNPJ.Substring(2, 3) + "." + astrCNPJ.Substring(5, 3) + "/" + astrCNPJ.Substring(8, 4) + "-" + astrCNPJ.Substring(12, 2);
                else if (astrCNPJ.Length == 11)
                    astrCNPJ = astrCNPJ.Substring(0, 3) + "." + astrCNPJ.Substring(3, 3) + "." + astrCNPJ.Substring(6, 3) + "-" + astrCNPJ.Substring(9, 2);

            }
            return astrCNPJ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="astrEmailsDestino">Eamil de destino. Se mais de um, separar por ;</param>
        /// <param name="astrCorpoEmail">HRML ou nao</param>
        /// <param name="astrTitulo"></param>
        /// <param name="astrSenderName"></param>
        /// <param name="aobjArquivosAnexos"></param>
        public static void EnviarEmail(string astrEmailsDestino, string astrCorpoEmail, string astrTitulo, string astrSenderName, List<string> aobjArquivosAnexos)
        {
            decimal ldecMaxAnexosFile = 5.0m;
            decimal ldecSumAnexosSize = 0;
            try
            {
                EmailLogEnvio lobjNewMail = new EmailLogEnvio();
                
                if (astrCorpoEmail.Length > 8000)
                    throw new Exception("O corpo do e-mail pode ter no máximo 8000 caracteres");

                lobjNewMail.CorpoEmail = astrCorpoEmail;
                lobjNewMail.DataCadastrada = DateTime.Now;
                lobjNewMail.EmailDestino = astrEmailsDestino;
                lobjNewMail.Titulo = astrTitulo;
                lobjNewMail.SenderName = astrSenderName;

                /*
                for (int i = 0; i < aobjArquivosAnexos.Count; i++)
                {
                    if (File.Exists(aobjArquivosAnexos[i]) == false)
                        throw new Exception("O arquivo " + aobjArquivosAnexos[i] + " não foi encontrado para ser anexado ao e-mail");
                    else
                    {
                        if (string.IsNullOrEmpty(lobjNewMail.ArquivoAnexo01))
                            lobjNewMail.ArquivoAnexo01 = aobjArquivosAnexos[i];
                        else if (string.IsNullOrEmpty(lobjNewMail.ArquivoAnexo02))
                            lobjNewMail.ArquivoAnexo02 = aobjArquivosAnexos[i];
                        else if (string.IsNullOrEmpty(lobjNewMail.ArquivoAnexo03))
                            lobjNewMail.ArquivoAnexo03 = aobjArquivosAnexos[i];
                        else if (string.IsNullOrEmpty(lobjNewMail.ArquivoAnexo04))
                            lobjNewMail.ArquivoAnexo04 = aobjArquivosAnexos[i];
                        else if (string.IsNullOrEmpty(lobjNewMail.ArquivoAnexo05))
                            lobjNewMail.ArquivoAnexo05 = aobjArquivosAnexos[i];
                    }
                }
                */

                lobjNewMail.HTML = (astrCorpoEmail.IndexOf("<body>") >= 0 || astrCorpoEmail.IndexOf("<div>") >= 0) ? "S" : "N";

                m_logMailRepository.InsertOrUpdate(lobjNewMail);
                m_logMailRepository.Save();


            }
            catch (Exception Erro)
            {
                throw Erro;
            }
        }

        public static string RelativeToAbsoluteUrl(string a_RelativePath)
        {
            string absoluteUrl;
            try
            {
                absoluteUrl = Utils.RelativeToAbsoluteUrl(a_RelativePath, false);
            }
            catch (Exception l_exception)
            {
                //Helpers.LayoutMessageHelper.SetErrorLog("Utils - RelativeToAbsoluteUrl", l_exception);
                throw;
            }
            return absoluteUrl;
        }
        public static string RelativeToAbsoluteUrl(string a_RelativePath, bool ablnAddOnlyDomain)
        {
            string str;
            try
            {
                if (a_RelativePath != null)
                {
                    if (!a_RelativePath.StartsWith("http"))
                    {
                        string empty = string.Empty;
                        string str1 = (string.IsNullOrEmpty(a_RelativePath) ? string.Empty : a_RelativePath);
                        string str2 = Utils.ApplicationRootUrl(!ablnAddOnlyDomain).TrimEnd(new char[] { '/' });
                        char[] chrArray = new char[] { '/' };
                        empty = string.Concat(str2, "/", str1.TrimStart(chrArray));
                        str = empty;
                    }
                    else
                        str = a_RelativePath;
                }
                else
                    str = string.Empty;
            }
            catch (Exception Error)
            {
                //Helpers.LayoutMessageHelper.SetErrorLog("Utils - RelativeToAbsoluteUrl", l_exception);
                throw Error;
            }
            return str;
        }

        public static string ApplicationRootUrl()
        {
            string str;
            try
            {
                str = Helpers.Utils.ApplicationRootUrl(true);
            }
            catch (Exception Error)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - ApplicationRootUrl", l_exception);
                throw Error;
            }
            return str;
        }

        public static string ApplicationRootUrl(bool ablnWithApplicationPath)
        {
            try
            {
                string empty = string.Empty;
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    object[] scheme = new object[] { current.Request.Url.Scheme, current.Request.Url.Host, null, null };
                    scheme[2] = (current.Request.Url.Port == 80 ? string.Empty : string.Concat(":", current.Request.Url.Port));
                    scheme[3] = (ablnWithApplicationPath ? ApplicationUrlPath() : string.Empty);
                    empty = string.Format("{0}://{1}{2}{3}", scheme);
                }
                if (!empty.EndsWith("/"))
                {
                    empty = string.Concat(empty, "/");
                }
                return empty;
            }
            catch (Exception Error)
            {
                //Helpers.Layout.LayoutMessageHelper.SetErrorLog("Utils - ApplicationRootUrl", l_exception);
                throw Error;
            }
        }

        public static string ApplicationUrlPath()
        {
            try
            {
                string l_ApplicationPath = string.Empty;
                HttpContext current = HttpContext.Current;

                if (current != null)
                    l_ApplicationPath = current.Request.ApplicationPath;

                return l_ApplicationPath;
            }
            catch (Exception Error)
            {
                //Helpers.LayoutMessageHelper.SetErrorLog("Utils - ApplicationUrlPath", l_exception);
                throw Error;
            }
        }

        public static void DeleteFileFromDisk(string a_FilePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(a_FilePath))
                {
                    if (System.IO.File.Exists(a_FilePath))
                        System.IO.File.Delete(a_FilePath);
                }
            }
            catch (Exception Error)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - DeleteFileFromDisk", l_exception);
                throw Error;
            }
        }

        public static string GetContentType(string fileName)
        {
            try
            {
                string contentType = "application/octetstream";

                string ext = System.IO.Path.GetExtension(fileName).ToLower();

                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                    contentType = registryKey.GetValue("Content Type").ToString();

                return contentType;
            }
            catch (Exception l_exception)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - GetContentType", l_exception);
                throw;
            }
        }

        public static bool HasMethod(string typeName, string methodName)
        {
            try
            {
                Type type = Type.GetType(typeName);
                return type.GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException ErrorAmbiguous)
            {
                // ambiguous means there is more than one result,
                // which means: a method with that name does exist
                return true;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public static string InvokeStringMethod(string typeName, string methodName)
        {
            try
            {
                Type type = Type.GetType(typeName);
                return (string)type.InvokeMember(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
            }
            catch (Exception Error)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - InvokeStringMethod", l_exception);
                throw Error;
            }
        }
        public static string InvokeStringMethod(string typeName, string methodName, string[] stringParam)
        {
            try
            {
                Type type = Type.GetType(typeName);
                string str = (string)type.InvokeMember(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, (object[])stringParam);
                return str;
            }
            catch (Exception Error)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - InvokeStringMethod", l_exception);
                throw Error;
            }
        }

        public static string RemoveAccents(string input)
        {
            try
            {
                string stFormD = input.Normalize(NormalizationForm.FormD);
                int len = stFormD.Length;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < len; i++)
                {
                    System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                    if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(stFormD[i]);
                    }
                }
                return (sb.ToString().Normalize(NormalizationForm.FormC));
            }
            catch (Exception Error)
            {
                //Helpers.ModelsUtils.LayoutMessageHelper.SetErrorLog("Utils - RemoveAccents", l_exception);
                throw Error;
            }
        }

        public static void ResizeImage(string a_ImageFilePath, int aintMaxWidth, int aintMaxHeight, out int a_NewWidth, out int a_NewHeight)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(a_ImageFilePath);
                int height = image.Height;
                int width = image.Width;
                int num = 0;
                int num1 = 0;
                if (height > aintMaxHeight)
                {
                    num = aintMaxHeight;
                    num1 = num * width / height;
                    if (num1 > aintMaxWidth)
                    {
                        num1 = aintMaxWidth;
                        num = num1 * height / width;
                    }
                }
                else if (width <= aintMaxWidth)
                {
                    num = height;
                    num1 = width;
                }
                else
                {
                    num1 = aintMaxWidth;
                    num = num1 * height / width;
                    if (num > aintMaxHeight)
                    {
                        num = aintMaxHeight;
                        num1 = num * width / height;
                    }
                }
                a_NewWidth = num1;
                a_NewHeight = num;
                System.Drawing.Image thumbnailImage = image.GetThumbnailImage(num1, num, null, IntPtr.Zero);
                image.Dispose();
                thumbnailImage.Save(a_ImageFilePath);
            }
            catch (Exception Error)
            {
                //Helpers.LayoutMessageHelper.SetErrorLog("Utils - ResizeImage", Error);
                throw Error;
            }
        }
    }
}