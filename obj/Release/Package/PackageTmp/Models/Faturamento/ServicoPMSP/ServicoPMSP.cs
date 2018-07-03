using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("FAT_SERVICOS_PMSP")]
    public class ServicoPMSP
    {

        public ServicoPMSP() { }
        public ServicoPMSP(string astrCodigoServico, DateTime adatDataInicio, decimal adecAliquota)
        {
            Codigo = astrCodigoServico;
            DataInicio = adatDataInicio;
            Aliquota = adecAliquota;
        }

        private string mstrCodigoServico = string.Empty;
        private string mstrDescricao = string.Empty;
        

        [Column("ID")]
        [Key]
        public string Codigo
        {
            get { return mstrCodigoServico; }
            set { mstrCodigoServico = value; }
        }
        
        [Column("DESCRICAO")]
        public string Descricao
        {
            get { return mstrDescricao; }
            set { mstrDescricao = value; }
        }

        [Column("ALIQUOTA")]
        public decimal? Aliquota { get; set; }
        

        internal static decimal getAliquota(string codigoServico)
        {
            return 0;
        }

        [Column("DATA_INICIO")]
        public DateTime? DataInicio { get; set; }
        

    }
}