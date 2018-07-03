using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("FAT_TIPO_IMPOSTO")]
    public class TipoImposto
    {
        public TipoImposto() { }
        public TipoImposto(enmTipoImposto aenmTipo, decimal adecAliquota, decimal adecTeto, decimal adecValorMinimo, string astrGovDestinatario)
        {
            mintCodigo = Convert.ToInt32(aenmTipo);
            mdecAliquota = adecAliquota;
            mdecTetoINSS = adecTeto;
            mdecValorMinimo = adecValorMinimo;
            mstrGovDestinatario = astrGovDestinatario;
        }

        private int mintCodigo;
        private decimal mdecTetoINSS;
        private decimal mdecAliquota;
        private string mstrNome;
        private decimal mdecValorMinimo;
        private string mstrGovDestinatario;

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID
        {
            get { return mintCodigo; }
            set { mintCodigo = value; }
        }

        [Column("NOME")]
        public string Nome
        {
            get
            {
                if (string.IsNullOrEmpty(mstrNome))
                {
                    if (mintCodigo == Convert.ToInt32(enmTipoImposto.eCofins))
                    { 
                        mstrNome = "Cofins";
                        mstrGovDestinatario = "FED";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.eCSLL))
                    { 
                        mstrNome = "CSLL";
                        mstrGovDestinatario = "FED";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.eImpostodeRenda))
                    { 
                        mstrNome = "Imposto de Renda";
                        mstrGovDestinatario = "FED";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.eINSS))
                    { 
                        mstrNome = "INSS";
                        mstrGovDestinatario = "FED";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.eISS))
                    { 
                        mstrNome = "ISS";
                        mstrGovDestinatario = "MP";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.ePis))
                    { 
                        mstrNome = "PIS";
                        mstrGovDestinatario = "FED";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.eISSOutroMunicipio))
                    { 
                        mstrNome = "ISS Fora Município";
                        mstrGovDestinatario = "MT";
                    }
                    else if (mintCodigo == Convert.ToInt32(enmTipoImposto.ePisCofinsCSLL))
                    {
                        mstrNome = "PIS/Cofins/CSLL";
                        mstrGovDestinatario = "FED";
                    }
                }
                return mstrNome;
            }
            set { mstrNome = value; }
        }

        [Column("ALIQUOTA")]
        public decimal AliquotaRetencao
        {
            get { return mdecAliquota; }
            set { mdecAliquota = value; }
        }

        [Column("TETO_INSS")]
        public decimal TetoINSS
        {
            get { return mdecTetoINSS; }
            set { mdecTetoINSS = value; }
        }

        [Column("VALOR_MINIMO")]
        public decimal ValorMinimo
        {
            get { return mdecValorMinimo; }
            set { mdecValorMinimo = value; }
        }

        /// <summary>
        /// Tipo de governo destinatario(recebedor do $). Pode ser FED quando Imposto Federal, MP quando municipio do prestador e MT quando municipio do tomador
        /// </summary>
        [Column("GOV_DESTINATARIO")]
        public string GovDestinatario
        {
            get { return mstrGovDestinatario; }
            set { mstrGovDestinatario = value; }
        }
        

        public static List<TipoImposto> getTiposImpostos(int aintCodMunicipioIbgePrestador, int aintCodMunicipioIbgeTomador, string astrCodigoServico, decimal adecAliquotaIssForaMunicipio)
        {
            //TODO:PUXAR DO BANCO DE ACORDO COM O MUNICIPIO INFORMADO
            List<TipoImposto> lobjRet = new List<TipoImposto>();
            IQueryable<TipoImposto> lobjTodosImpostosDB = new TipoImpostoRepository().All;

            foreach (TipoImposto item in lobjTodosImpostosDB)
            {
                if (item.GovDestinatario == "FED")
                    lobjRet.Add(item);
            }
            
            if (aintCodMunicipioIbgePrestador == 3550308 ) //sao paulo
            {
                ServicoPMSPRepository lobjServicosDB = new ServicoPMSPRepository();
                if (ATIMO.Helpers.Utils.IsNumeric(astrCodigoServico) == false)
                    throw new Exception("Obrigatório preecher o codigo de serviço apenas com números");

                ServicoPMSP lobjServico = lobjServicosDB.Find(astrCodigoServico);
                if (lobjServico != null)
                    lobjRet.Add(new TipoImposto(enmTipoImposto.eISS, Convert.ToDecimal(lobjServico.Aliquota), 0, 0, "MP"));
                else
                    throw new Exception("Codigo de servico " + astrCodigoServico + " não encontrado. Favor verifique");
            }

            if (aintCodMunicipioIbgePrestador != aintCodMunicipioIbgeTomador)
            {
                /*
                MunicipioRepository lobjMunicipioDB = new MunicipioRepository();
                Municipio lobjMunicTomador = lobjMunicipioDB.FindByCodigoIBGE(aintCodMunicipioIbgeTomador.ToString());
                if (lobjMunicTomador == null)
                    throw new Exception("Município codigo IBGE " + aintCodMunicipioIbgeTomador + " não encontrado. Favor verifique");
                */
                lobjRet.Add(new TipoImposto(enmTipoImposto.eISSOutroMunicipio, adecAliquotaIssForaMunicipio, 0, 0, "MT"));
                    
            }
            
            return lobjRet;
        }

    }
}