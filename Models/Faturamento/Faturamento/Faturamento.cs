using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;
using ATIMO.Models;
using Nfse.PMSP.RN;
using ATIMO.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using ATIMO.Helpers.ModelsUtils;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace ATIMO.Models.Faturamento
{
    public enum enmTipoPessoaTrib
    {
        eNaoPreenchido = 0,
        /// <summary>
        /// Pessoa Juridica Comum de Direito Privado
        /// </summary>
        ePJ_Comum = 1,
        /// <summary>
        /// Pessoa Juridica Optante pelo Simples Nacional
        /// </summary>
        ePJ_OptanteSimples = 2,
        /// <summary>
        /// Micro Empreendedor Individual
        /// </summary>
        ePJ_Mei = 3,
        /// <summary>
        /// Pessoa Fisica
        /// </summary>
        ePF = 4,
        /// <summary>
        /// Pessoa Juridica que nao retem Imposto de Renda. Exemplo: Condominio
        /// </summary>
        ePJ_SemIR = 5,
    }

    public enum enmTipoImposto
    {
        eNaoDefinido = 0,
        eImpostodeRenda = 1,
        [Obsolete("Usar o enum ePisCofinsCSLL pois ele tem os 3 impostos agrupados")]
        ePis = 2,
        [Obsolete("Usar o enum ePisCofinsCSLL pois ele tem os 3 impostos agrupados")]
        eCofins = 3,
        [Obsolete("Usar o enum ePisCofinsCSLL pois ele tem os 3 impostos agrupados")]
        eCSLL = 4,
        eINSS = 5,
        eISS = 6,
        eISSOutroMunicipio = 7,
        ePisCofinsCSLL = 9,
    }
        
    public class PessoaTomador
    {
        public PessoaTomador() { }

        public PessoaTomador(PESSOA aobjPessoa)
        {
            if (aobjPessoa.TIPO_PESSOA_TRIBUTACAO == null && string.IsNullOrEmpty(aobjPessoa.TIPO_DOC) == false)
            {
                aobjPessoa.TIPO_PESSOA_TRIBUTACAO = Convert.ToInt32((aobjPessoa.TIPO_DOC == "F" ? enmTipoPessoaTrib.ePF : enmTipoPessoaTrib.ePJ_Comum));
            }
            setValues((enmTipoPessoaTrib)aobjPessoa.TIPO_PESSOA_TRIBUTACAO.GetValueOrDefault((int)enmTipoPessoaTrib.eNaoPreenchido), aobjPessoa.CIDADE);
        }

        public PessoaTomador(enmTipoPessoaTrib aobjTipo, string astrNomeCidadeTomador)
        {
            setValues(aobjTipo, astrNomeCidadeTomador);
        }

        private void setValues(enmTipoPessoaTrib aobjTipo, string astrNomeCidadeTomador)
        {
            TipoPessoa = (aobjTipo == enmTipoPessoaTrib.ePF) ? "PF" : "PJ";
            RetemIR = (aobjTipo == enmTipoPessoaTrib.ePJ_Comum) ? "S" : "N";
            OptanteSimples = (aobjTipo == enmTipoPessoaTrib.ePJ_OptanteSimples) ? "S" : "N";
            CodigoMunicipioIBGE = Utils.TrataNullInt32(MunicipioHelper.getCodigoIbgeFromCidade(astrNomeCidadeTomador));
        }

        public string RetemIR { get; set; }
        public string TipoPessoa { get; set; }
        public string OptanteSimples { get; set; }
        public int CodigoMunicipioIBGE { get; set; }
        
        public bool TomadorRetemImpostos()
        {
            return (OptanteSimples == "N" && TipoPessoa == "PJ");
        }

        public static bool IsTomadorPreenchidoCorretamente(PESSOA aobjPessoaTomador, ref List<string> alistMsgErro)
        {
            bool lblnRet = true;

            if (alistMsgErro == null)
                alistMsgErro = new List<string>();

            if (string.IsNullOrEmpty(aobjPessoaTomador.NOME))
            {
                alistMsgErro.Add("O nome do tomador não está preenchido");
                lblnRet = false;
            }

            if (string.IsNullOrEmpty(aobjPessoaTomador.NUM_DOC))
            {
                alistMsgErro.Add("O cpf/cnpj do tomador não está preenchido");
                lblnRet = false;
            }

            if (string.IsNullOrEmpty(aobjPessoaTomador.CIDADE))
            {
                alistMsgErro.Add("A cidade do tomador não está preenchido");
                lblnRet = false;
            }

            if (string.IsNullOrEmpty(aobjPessoaTomador.UF))
            {
                alistMsgErro.Add("O estado do tomador não está preenchido");
                lblnRet = false;
            }

            if (aobjPessoaTomador.TIPO_PESSOA_TRIBUTACAO == (int?)enmTipoPessoaTrib.eNaoPreenchido)
            {
                alistMsgErro.Add("A forma de tributação deste cliente não está preenchido, o sistema poderá calcular incorretamente as retenções");
                lblnRet = false;
            }

            return lblnRet;
        }

    }
    
    [Table("FAT_FATURAMENTO")]
    public class Faturamento
    {

        private int mintID;
        private decimal mdecValorTotal;
        private decimal mdecValorMaoDeObra;
        private int? mintNumeroNF;
        private string mstrCodigoVerificacao = string.Empty;
        private DateTime mdatDataCadastro;
        private string mstrDescricaoNF = string.Empty;
        private string mstrCancelada = string.Empty;
        private int mintNumeroRPS;
        private string mstrCaminhoPdfNf = string.Empty;
        private readonly IEmpresaRepository m_empresaRepository;
        private readonly IFaturamentoRepository m_faturamentoRepository;

        public Faturamento()
        {
            m_empresaRepository = new EmpresaRepository();
            m_faturamentoRepository = new FaturamentoRepository();
        }

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID
        {
            get { return mintID; }
            set { mintID = value; }
        }

        [Column("ID_OSSB")]
        public virtual int OSSBID { get; set; }
        //[ForeignKey("ID_OSSB")] //Não é possível fazer dessa forma pois a classe PESSOA pertence a outro DbContext
        [NotMapped]
        public virtual OSSB OSSB { get; set; }
        
        [Column("DATA_EMISSAO_NF")]
        public DateTime? DataEmissao { get; set; }

        [Column("LOTE_EMISSAO_PMSP")]
        public int? LoteEmissaoPMSP { get; set; }
        

        [Column("NUMERO_NF")]
        public int? NumeroNF
        {
            get { return mintNumeroNF; }
            set { mintNumeroNF = value; }
        }

        [Column("COD_VERIFICACAO_NF")]
        public string CodigoVerificacao
        {
            get { return mstrCodigoVerificacao; }
            set { mstrCodigoVerificacao = value; }
        }

        [Column("ID_EMPRESA")]
        public virtual int EmpresaEmissaoID { get; set; }
        [ForeignKey("EmpresaEmissaoID")]
        public virtual Empresa EmpresaEmissao { get; set ; }
        
        [Column("DATA_CADASTRO")]
        public DateTime DataCadastro
        {
            get { return mdatDataCadastro; }
            set { mdatDataCadastro = value; }
        }

        [Column("DESCRICAO_NF")]
        public string Descricao
        {
            get { return mstrDescricaoNF; }
            set { mstrDescricaoNF = value; }
        }

        [Column("FLAG_CANCELADA")]
        public string Cancelada
        {
            get { return mstrCancelada; }
            set { mstrCancelada = value; }
        }

        [Column("NUMERO_RPS")]
        public int NumeroRPS
        {
            get { return mintNumeroRPS; }
            set { mintNumeroRPS = value; }
        }

        [Column("ID_SERVICO_PMSP")]
        public virtual string ServicoPMSPID { get; set; }
        [ForeignKey("ServicoPMSPID")]
        public virtual ServicoPMSP ServicoPMSP { get; set; }
        
        
        [Column("ID_CLIENTE")]
        public virtual int ClienteID { get; set; }
        //[ForeignKey("ID_Cliente")]  //Não é possível fazer dessa forma pois a classe PESSOA pertence a outro DbContext
        [NotMapped]
        public virtual PESSOA Cliente { get; set; }

        [Column("NUM_SERIE_RPS")]
        public string SerieRPS { get; set; }

        [Column("VALOR_SERVICO")]
        public decimal ValorServico { get; set; }
        
        [Column("VALOR_MAO_DE_OBRA")]
        public decimal ValorMaoDeObra
        {
            get { return mdecValorMaoDeObra; }
            set { mdecValorMaoDeObra = value; }
        }

        [Column("VALOR_BRUTO")]
        public decimal ValorBruto
        {
            get { return mdecValorTotal; }
            set { mdecValorTotal = value; }
        }

        [Column("CAMINHO_PDF_NF")]
        public string CaminhoPdfNf
        {
            get { return mstrCaminhoPdfNf; }
            set { mstrCaminhoPdfNf = value; }
        }

        [Column("CONREC_HISTORICO")]
        public string HistoricoContasReceber { get; set; }

        [Column("CONREC_DATAVENCTO")]
        public DateTime? DataVencimentoContasReceber { get; set; }

        [Column("CONREC_CAMINHO_PDF_BOLETO")]
        public string CaminhoPDFBoletoContasReceber { get; set; }
        
        [Column("CONREC_TIPOPAGTO")]
        public string TipoPagamentoContasReceber { get; set; }

        [Column("CONREC_DATAPAGTO")]
        public DateTime? DataPagamentoContasReceber { get; set; }

        [Column("ID_SITUACAOPAGTO")]
        public int? SituacaoPagamentoContasReceberId { get; set; }
        [ForeignKey("SituacaoPagamentoContasReceberId")]
        public virtual SituacaoPagamento SituacaoPagamentoContasReceber { get; set; }

        [Column("VALOR_LIQUIDO")]
        public decimal? ValorLiquido{ get; set; }
        
        public string Situacao
        {
            get
            {
                string lstrRet = string.Empty;
                if (NumeroNF > 0)
                {
                    if (Cancelada == "S")
                        lstrRet = "Nota Fiscal Cancelada";
                    else
                        lstrRet = "Nota Fiscal Emitida";
                }
                else
                {
                    if (DataEmissao < DateTime.Now.Date)
                        lstrRet = "Nota não emitida";
                    else
                        lstrRet = "Nota a ser emitida";
                }

                return lstrRet;
            }
        }

        public string getUrlLinkNfSitePmsp()
        {
            return "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprintimg.aspx?inscricao=" + EmpresaEmissao.CcmPmsp + "&nf=" + NumeroNF + "&verificacao=" + CodigoVerificacao + "&imprimir=1";
        }

        /// <summary>
        /// Obtem o caminho do PDF a partir do cadastro do faturamento no banco de dados, e se nao preenchido, faz download e update no campo
        /// </summary>
        /// <param name="aintTipo">1 = caminho local (c:\...) | 2 = Url completo (http://...) | 3 = Url relativa</param>
        /// <returns></returns>
        public string getCaminhoPdfNf(int aintTipo)
        {
            string lstrRet = this.CaminhoPdfNf;
            if (string.IsNullOrEmpty(lstrRet))
            {
                if (EmpresaEmissao == null)
                    EmpresaEmissao = new Empresa();

                if (EmpresaEmissaoID > 0 && EmpresaEmissao.ID == 0)
                    EmpresaEmissao.ID = EmpresaEmissaoID;

                if (EmpresaEmissao.ID > 0 && string.IsNullOrEmpty(EmpresaEmissao.CcmPmsp))
                {
                    Empresa lobjEmpDB = m_empresaRepository.All.Where(p => p.ID == EmpresaEmissao.ID).FirstOrDefault<Empresa>();
                    if (lobjEmpDB != null)
                        EmpresaEmissao = lobjEmpDB;
                }

                if (NumeroNF == null || NumeroNF == 0 || string.IsNullOrEmpty(CodigoVerificacao) || string.IsNullOrEmpty(EmpresaEmissao.CcmPmsp))
                    throw new Exception("Não é possível baixar a nota fiscal porque um dos seguintes campos campos vazios: numero da nota, codigo de verificacao ou CCM da emissor");
                else
                {
                    lstrRet = AppDomain.CurrentDomain.BaseDirectory + @"\Arquivos\PdfNotaFiscal\";
                    if (Directory.Exists(lstrRet)==false)
                        Directory.CreateDirectory(lstrRet);

                    lstrRet += "NF" + NumeroNF + "CCM" + EmpresaEmissao.CcmPmsp + "VERIF" + CodigoVerificacao + ".gif";
                    if (File.Exists(lstrRet))
                        File.Delete(lstrRet);

                    new WebClient().DownloadFile(getUrlLinkNfSitePmsp(), lstrRet);

                    Image img = Image.FromFile(lstrRet);
                    
                    int lintNovoWidth = 1000;
                    int lintNovoHeight = 1000;
                    int lintOrigWidth = img.Width; // largura original
                    int lintOrigHeight = img.Height; // altura original

                    // redimensiona se necessario
                    if (lintOrigWidth > lintNovoWidth || lintOrigHeight > lintNovoHeight)
                    {
                        if (lintOrigWidth > lintOrigHeight)
                            // imagem horizontal
                            lintNovoHeight = (lintOrigHeight * lintNovoWidth) / lintOrigWidth;
                        else
                            // imagem vertical
                            lintNovoWidth = (lintOrigWidth * lintNovoHeight) / lintOrigHeight;
                    }
                    
                    Bitmap newImage = new Bitmap(lintNovoWidth, lintNovoHeight);
                    using (Graphics gr = Graphics.FromImage(newImage))
                    {
                        gr.SmoothingMode = SmoothingMode.HighQuality;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        gr.DrawImage(img, new Rectangle(0, 0, lintNovoWidth, lintNovoHeight));
                    }
                    lstrRet = lstrRet.Replace(".gif", ".jpg");
                    if (File.Exists(lstrRet))
                        File.Delete(lstrRet);

                    newImage.SetResolution(96, 96);
                    newImage.Save(lstrRet, ImageFormat.Jpeg);
                    
                    PdfDocument lobjPdfMatriz = new PdfDocument();
                    PdfPage lobjPagina = lobjPdfMatriz.Pages.Add();
                    XGraphics lobjGraphics = XGraphics.FromPdfPage(lobjPagina);

                    if (File.Exists(lstrRet))
                    {
                        XImage lobjLogo = XImage.FromFile(lstrRet);
                        lobjGraphics.DrawImage(lobjLogo, 10, 10);
                        lobjLogo.Dispose();
                        lobjLogo = null;
                    }

                    lobjPdfMatriz.Close();

                    lstrRet = lstrRet.Replace(".jpg", ".pdf");
                    if (File.Exists(lstrRet))
                        File.Delete(lstrRet);

                    lobjPdfMatriz.Save(lstrRet);
                    
                    CaminhoPdfNf = lstrRet;

                    if (this.ID > 0)
                    {
                        
                        Faturamento lobjFatUpdate = m_faturamentoRepository.All.Where(p => p.ID == this.ID).FirstOrDefault<Faturamento>();
                        if (lobjFatUpdate != null)
                        {
                            lobjFatUpdate.CaminhoPdfNf = lstrRet;
                            m_faturamentoRepository.InsertOrUpdate(lobjFatUpdate);
                            m_faturamentoRepository.Save();
                        }
                    }

                }

            }

            if (File.Exists(lstrRet) && lstrRet.ToLower().StartsWith(AppDomain.CurrentDomain.BaseDirectory.ToLower()))
            {
                if (aintTipo == 2)
                {
                    lstrRet = CaminhoPdfNf.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);
                }
                else if (aintTipo == 3)
                {

                }
            }
            else
                lstrRet = string.Empty;

            return lstrRet;
        }
                
        public virtual ICollection<FaturamentoRetencoes> Retencoes { get; set; }

        public decimal GetTotalRetencoes()
        {
            try
            {
                decimal ldecRet = 0;
                if (this.Retencoes != null)
                {
                    foreach (FaturamentoRetencoes r in this.Retencoes)
                    {
                        //if (r.RetemImposto == "S")
                        ldecRet += r.ValorRetencao;
                    }
                }
                return ldecRet;
            }
            catch (Exception Error)
            {
                throw Error;
            }
        }

        public decimal GetValorLiquido()
        {
            return GetValorTotal() - GetTotalRetencoes();
        }

        public decimal GetValorTotal()
        {
            return ValorServico + ValorMaoDeObra;
        }

        [NotMapped]
        //TODO: Tirar esse método daqui e colocar em uma classe FaturamentoHelper
        public string NumDocCliente { get; set; }
        
    }

    public class FaturamentoCrudViewModel
    {
        private List<string> mlistMensagemValidacaoCliente = new List<string>();
        
        public Faturamento Faturamento { get; set; }

        public FaturamentoRetencoesTableViewModel RetencoesViewModel { get; set; }
                                
        public List<string> MensagemValidacaoCliente
        {
            get { return mlistMensagemValidacaoCliente; }
            set { mlistMensagemValidacaoCliente = value; }
        }
        
        public List<Empresa> EmpresasEmissorasDisponiveis { get; set; }

        public List<ServicoPMSP> ServicosPMSP { get; set; }

        public int OpcaoEmissao { get; set; }

        public string CodigoIbgeCidadeTomador { get; set; }

    }

}