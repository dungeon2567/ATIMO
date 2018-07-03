using ATIMO.Models.Faturamento;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    [Table("EMPRESA")]
    public class Empresa
    {
        private string mstrCertificado = string.Empty;
        private string mstrCcmPmsp = string.Empty;
        private string mstrCodigoServicoPMSP = string.Empty;
        private string mstrCNPJ = string.Empty;
        private string mstrOptanteSimplesNacional = string.Empty;
        private string mstrFonteCargaTrib = string.Empty;
        private string mstrNome = string.Empty;
        private List<ServicoPMSP> mobjListaServicos = new List<ServicoPMSP>();
        private string mstrEmiteNF = string.Empty;
        private string mstrSerieRPS;
        private decimal mdecAliquotaIssForaMunicipio = 0;


        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        

        [Column("NOME")]
        public string Nome
        {
            get { return mstrNome; }
            set { mstrNome = value; }
        }


        [Column("SERIAL_CERTIFICADO")]
        public string SerialCertificado
        {
            get { return mstrCertificado; }
            set { mstrCertificado = value; }
        }

        [Column("CNPJ")]
        public string CNPJ
        {
            get { return mstrCNPJ; }
            set { mstrCNPJ = value; }
        }

        [Column("COD_SERVICO_PMSP")]
        public virtual string CodigoServicoPMSP { get; set; }
        [ForeignKey("CodigoServicoPMSP")]
        public virtual ServicoPMSP ServicoPMSP { get; set; }

        [NotMapped]
        public List<ServicoPMSP> ListaServicos
        {
            get { return mobjListaServicos; }
            set { mobjListaServicos = value; }
        }

        [Column("CCM_PMSP")]
        public string CcmPmsp
        {
            get { return mstrCcmPmsp; }
            set { mstrCcmPmsp = value; }
        }

        [Column("FLG_EMITE_NF")]
        public string EmiteNF
        {
            get { return mstrEmiteNF; }
            set { mstrEmiteNF = value; }
        }

        [Column("NUM_ULTIMA_RPS")]
        public int? UltimaRPS { get; set; }

        [Column("FLG_OPTANTE_SIMPLES_NACIONAL")]
        public string OptanteSimplesNacional
        {
            get { return mstrOptanteSimplesNacional; }
            set { mstrOptanteSimplesNacional = value; }
        }

        [Column("NOM_FONTE_CARGA_TRIB")]
        public string FonteCargaTrib
        {
            get { return mstrFonteCargaTrib; }
            set { mstrFonteCargaTrib = value; }
        }

        [Column("PERC_CARGA_TRIB")]
        public decimal? PercentualCargaTributaria { get; set; }

        [Column("SERIE_RPS")]
        public string SerieRPS
        {
            get { return mstrSerieRPS; }
            set { mstrSerieRPS = value; }
        }

        [Column("ALIQUOTA_ISS_FORA_MUNICIPIO")]
        public decimal AliquotaIssForaMunicipio
        {
            get { return mdecAliquotaIssForaMunicipio; }
            set { mdecAliquotaIssForaMunicipio = value; }
        }


        /*
        public static EmpresaConfig getDadosFromReceita(bool ablnDadosDoCertificado, string astrCNPJ)
        {
            
            if (string.IsNullOrEmpty(astrCNPJ))
            {
                if (ablnDadosDoCertificado)
                {
                    //pega cnpj do certificado
                    //astrCNPJ =;
                }
                else
                {
                    //throw ();
                }
            }
            
            if (ablnUsaCertificado)
            {
                NFSePMSP.getXmlConsultaCNPJ();
            }
            else
            {
                //
            }

            // https://www3.prefeitura.sp.gov.br/fdc/fdc_imp05_cgc.asp?Ccm_aux=32854110 (NUMERO CCM)
        }
        */

        public virtual ICollection<EmpresaServicoPMSP> ServicosPMSP
        {
            get; set;
        }

    }
}