using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("MUNICIPIO")]
    public class Municipio
    {
        private int mintCodigo;
        private string mstrNome = string.Empty;
        private string mstrCodigoIBGE = string.Empty;
        private string mstrUF = string.Empty;
        private decimal? mdecAliquotaIssForaMunicipio = 0;

        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo
        {
            get { return mintCodigo; }
            set { mintCodigo = value; }
        }

        [Column("NOME")]
        public string Nome
        {
            get { return mstrNome; }
            set { mstrNome = value; }
        }

        [Column("COD_IBGE")]
        public string CodigoIBGE
        {
            get { return mstrCodigoIBGE; }
            set { mstrCodigoIBGE = value; }
        }

        [Column("UF")]
        public string UF
        {
            get { return mstrUF; }
            set { mstrUF = value; }
        }
        
        [Column("ALIQUOTA_ISS_FORA_MUNICIPIO")]
        public decimal? AliquotaIssForaMunicipio
        {
            get { return mdecAliquotaIssForaMunicipio; }
            set { mdecAliquotaIssForaMunicipio = value; }
        }
        
    }
}