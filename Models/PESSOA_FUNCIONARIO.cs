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
    
    public partial class PESSOA_FUNCIONARIO
    {
        public int ID { get; set; }
        public int FUNCIONARIO { get; set; }
        public int FUNCAO { get; set; }
        public System.DateTime DATA_ADMISSAO { get; set; }
        public Nullable<System.DateTime> DATA_DEMISSAO { get; set; }
        public decimal SALARIO { get; set; }
    
        public virtual FUNCAO FUNCAO1 { get; set; }
        public virtual PESSOA PESSOA { get; set; }
    }
}