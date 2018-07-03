using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("FAT_FATURAMENTO_RETENCAO")]
    public class FaturamentoRetencoes
    {
        public FaturamentoRetencoes() { }

        public FaturamentoRetencoes(enmTipoImposto aenmTipoImposto, decimal adecBaseCalculo, List<TipoImposto> aobjTiposImposto)
        {
            bool lblnRetemImposto = true;
            TipoImposto lobjTipoImposto = aobjTiposImposto.Find(delegate (TipoImposto t) { return t.ID == Convert.ToInt32(aenmTipoImposto); });

            if (lobjTipoImposto == null)
                throw new Exception("O sistema não está com o tipo de imposto " + aenmTipoImposto.ToString() + " devidamente parametrizado no sistema");
            
            this.TipoImpostoID = Convert.ToInt32(aenmTipoImposto);
            this.TipoImposto = lobjTipoImposto;
                        
            mdecBaseCalculo = adecBaseCalculo;
            mdecPercentUsado = lobjTipoImposto.AliquotaRetencao;
            mdecValorRetencao = Math.Round((adecBaseCalculo * mdecPercentUsado) / 100, 2);
            if (aenmTipoImposto == enmTipoImposto.eINSS && mdecValorRetencao > lobjTipoImposto.TetoINSS)
                mdecValorRetencao = lobjTipoImposto.TetoINSS;

            if (mdecValorRetencao < lobjTipoImposto.ValorMinimo || mdecValorRetencao == 0)
            {
                lblnRetemImposto = false;
            }
            mstrRetemImposto = lblnRetemImposto ? "S" : "N";
            TipoImposto = lobjTipoImposto;
        }

        public FaturamentoRetencoes(enmTipoImposto aenmTipoImposto, decimal adecBaseCalculo, decimal adecPercentual, bool ablnRetemImposto, decimal adecValorMinimo, string astrGovDestinatario)
        {
            this.ID = Convert.ToInt32(aenmTipoImposto);
            mdecBaseCalculo = adecBaseCalculo;
            mdecPercentUsado = adecPercentual;
            mdecValorRetencao = Math.Round((adecBaseCalculo * adecPercentual) / 100, 2);
            mstrRetemImposto = ablnRetemImposto ? "S" : "N";
            TipoImposto = new TipoImposto(aenmTipoImposto, adecPercentual, 0, adecValorMinimo, astrGovDestinatario);
        }

        private int mintIdFaturamento;
        
        private decimal mdecValorRetencao;
        private decimal mdecBaseCalculo;
        private decimal mdecPercentUsado;
        private string  mstrRetemImposto;


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        //[Key]
        [Column("ID_FATURAMENTO")]
        public int FaturamentoID
        {
            get { return mintIdFaturamento; }
            set { mintIdFaturamento = value; }
        }
        [ForeignKey("FaturamentoID")]
        public Faturamento Faturamento { get; set; }

        //[Key]
        [Column("ID_TIPO_IMPOSTO")]
        public virtual int TipoImpostoID { get; set; }
        [ForeignKey("TipoImpostoID")]
        public virtual TipoImposto TipoImposto { get; set; }
        

        [Column("VALOR_RETENCAO")]
        public decimal ValorRetencao
        {
            get { return mdecValorRetencao; }
            set { mdecValorRetencao = value; }
        }

        [Column("BASE_CALCULO")]
        public decimal BaseCalculo
        {
            get { return mdecBaseCalculo; }
            set { mdecBaseCalculo = value; }
        }

        [Column("PERCENT_USADO")]
        public decimal PercentUsado
        {
            get { return mdecPercentUsado; }
            set { mdecPercentUsado = value; }
        }

        [Column("RETEM_IMPOSTO")]
        public string RetemImposto
        {
            get { return mstrRetemImposto; }
            set { mstrRetemImposto = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adecValorNF">Valor total da nota, ja somado o valor de mao de obra</param>
        /// <param name="adecBaseCalculoINSS"></param>
        /// <param name="aobjEmpresa"></param>
        /// <param name="aobjTomador"></param>
        /// <param name="ablnIncluirImpostoZerado"></param>
        /// <returns></returns>
        public static List<FaturamentoRetencoes> getRetencoes(decimal adecValorNF, decimal adecBaseCalculoINSS, Empresa aobjEmpresa, ref PessoaTomador aobjTomador, out string astrOutMotivos)
        {
            List<FaturamentoRetencoes> lobjRet = new List<FaturamentoRetencoes>();

            if (String.IsNullOrEmpty(aobjEmpresa.CodigoServicoPMSP))
                throw new Exception("Não foi informado um código de serviço para esta empresa");

            List<TipoImposto> lobjTiposImpostos = TipoImposto.getTiposImpostos(3550308, aobjTomador.CodigoMunicipioIBGE, aobjEmpresa.CodigoServicoPMSP, aobjEmpresa.AliquotaIssForaMunicipio);
            //bool lblnAddPCC = false, lblnAddIR = false, lblnAddINSS = false, lblnAddISS = false;
            astrOutMotivos = string.Empty;

            if (aobjEmpresa.OptanteSimplesNacional != "N" && aobjEmpresa.OptanteSimplesNacional != "S")
                throw new Exception("Para o cálculo da retenção, é necessário informar se o prestador é ou não optante do Simples Nacional");

            if (aobjTomador.TipoPessoa != "PJ" && aobjTomador.TipoPessoa != "PF")
                throw new Exception("Para o cálculo da retenção, é necessário informar se o cliente é pessoa física ou jurídica");

            if (aobjEmpresa.OptanteSimplesNacional == "N" && aobjTomador.TomadorRetemImpostos())
            {
                lobjRet.Add(new FaturamentoRetencoes(enmTipoImposto.ePisCofinsCSLL, adecValorNF, lobjTiposImpostos));
                
                if (aobjTomador.RetemIR != "N")
                    lobjRet.Add(new FaturamentoRetencoes(enmTipoImposto.eImpostodeRenda, adecValorNF, lobjTiposImpostos));

                if (aobjTomador.TipoPessoa == "PJ" && adecBaseCalculoINSS > 0)
                    lobjRet.Add(new FaturamentoRetencoes(enmTipoImposto.eINSS, adecBaseCalculoINSS, lobjTiposImpostos));

                lobjRet.Add(new FaturamentoRetencoes(enmTipoImposto.eISS, adecValorNF, lobjTiposImpostos));

            }
            
            if (aobjTomador.CodigoMunicipioIBGE != 3550308)
            { 
                lobjRet.Add(new FaturamentoRetencoes(enmTipoImposto.eISSOutroMunicipio, adecValorNF, lobjTiposImpostos));
            }

            if (lobjRet.Count == 0)
            {
                if (aobjEmpresa.OptanteSimplesNacional == "S")
                    astrOutMotivos = "Não foi calculada retenção porque empresas optantes do Simples não devem (confirmar com contabilidade)";
                else if (aobjTomador.TomadorRetemImpostos() == false)
                    astrOutMotivos = "Não foi calculada retenção porque o cliente não se enquadra nesta situação";
            }

            return lobjRet;
        }

    }

    public class FaturamentoRetencoesTableViewModel
    {
        public List<FaturamentoRetencoes> FaturamentoRetencoes { get; set; }
        public decimal TotalRetencoes { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal ValorTotalNota { get; set; }
        public string MensagemRetencoes { get; set; }
    }
}