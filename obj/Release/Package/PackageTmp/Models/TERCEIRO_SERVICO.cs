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
    
    public partial class TERCEIRO_SERVICO
    {
        public int ID { get; set; }
        public int TERCEIRO { get; set; }
        public int SERVICO { get; set; }
        public string SITUACAO { get; set; }
        public int AREA_MANUTENCAO { get; set; }
    
        public virtual PESSOA PESSOA { get; set; }
        public virtual SERVICO SERVICO1 { get; set; }
        public virtual AREA_MANUTENCAO AREA_MANUTENCAO1 { get; set; }
    }
}
