//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ATIMO.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CONTRATO_LOJA
    {
        public int ID { get; set; }
        public int CONTRATO { get; set; }
        public int LOJA { get; set; }
        public int QUANTIDADE { get; set; }
        public string SITUACAO { get; set; }
        public int QTD_CORTESIA { get; set; }
        public decimal VALOR_CONTRATO { get; set; }
        public decimal VALOR_TERCEIRO { get; set; }
        public decimal VALOR_EXTRA { get; set; }
        public string PERIODICIDADE { get; set; }
        public decimal VALOR { get; set; }
    
        public virtual CONTRATO CONTRATO1 { get; set; }
        public virtual LOJA LOJA1 { get; set; }
    }
}
