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
    
    public partial class TIPO_CONTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPO_CONTA()
        {
            this.CONTA_BANCARIA = new HashSet<CONTA_BANCARIA>();
        }
    
        public int ID { get; set; }
        public string DESCRICAO { get; set; }
        public string SIGLA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCARIA> CONTA_BANCARIA { get; set; }
    }
}