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
    
    public partial class OSSB_SERVICO_TERCEIRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OSSB_SERVICO_TERCEIRO()
        {
            this.PAGAMENTO1 = new HashSet<PAGAMENTO_TERCEIRO>();
        }
    
        public decimal VALOR { get; set; }
        public int OSSB_SERVICO { get; set; }
        public int TERCEIRO { get; set; }
        public Nullable<System.DateTime> DATE_VENCIMENTO { get; set; }
        public int DESPESA { get; set; }
    
        public virtual OSSB_SERVICO OSSB_SERVICO1 { get; set; }
        public virtual PESSOA TERCEIRO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO_TERCEIRO> PAGAMENTO1 { get; set; }
        public virtual DESPESA DESPESA1 { get; set; }
    }
}
