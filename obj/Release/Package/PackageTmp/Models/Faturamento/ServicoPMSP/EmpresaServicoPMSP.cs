using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    [Table("EMPRESA_SERVICOPMSP")]
    public class EmpresaServicoPMSP
    {
        [Column("ID_EMPRESA", Order = 0)]
        [Key]
        public int EmpresaCodigo { get; set; }
        [ForeignKey("EmpresaCodigo")]
        public virtual Empresa Empresa { get; set; }

        [Column("ID_CODIGOSERVICO", Order = 1)]
        [Key]
        public string CodigoServicoPMSP { get; set; }
        [ForeignKey("CodigoServicoPMSP")]
        public virtual ServicoPMSP ServicoPMSP { get; set; }

    }
}