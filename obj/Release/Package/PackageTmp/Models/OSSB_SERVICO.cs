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
    
    public partial class OSSB_SERVICO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OSSB_SERVICO()
        {
            this.OSSB_SERVICO_TERCEIRO = new HashSet<OSSB_SERVICO_TERCEIRO>();
            this.PAGAMENTO = new HashSet<PAGAMENTO>();
        }
    
        public int ID { get; set; }
        public int UNIDADE { get; set; }
        public decimal QUANTIDADE { get; set; }
        public decimal VALOR_MO { get; set; }
        public string FLG_ABANDONO { get; set; }
        public Nullable<decimal> VALOR_PAGO_ABANDONO { get; set; }
        public int OSSB { get; set; }
        public decimal VALOR_MA { get; set; }
        public string DESCRICAO { get; set; }
        public string SUBDIVISAO { get; set; }
        public decimal VALOR_MA_BDI { get; set; }
        public decimal VALOR_MO_BDI { get; set; }
    
        public virtual UNIDADE UNIDADE1 { get; set; }
        public virtual OSSB OSSB1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_SERVICO_TERCEIRO> OSSB_SERVICO_TERCEIRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO> PAGAMENTO { get; set; }
    }
}
