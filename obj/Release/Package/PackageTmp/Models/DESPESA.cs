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
    
    public partial class DESPESA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DESPESA()
        {
            this.COMPRA = new HashSet<COMPRA>();
            this.PAGAMENTO = new HashSet<PAGAMENTO>();
            this.CAIXINHA_ITEM = new HashSet<CAIXINHA_ITEM>();
            this.OSSB_SERVICO_TERCEIRO = new HashSet<OSSB_SERVICO_TERCEIRO>();
            this.CONTAS_PAGAR = new HashSet<CONTAS_PAGAR>();
            this.CONTAS_PAGAS = new HashSet<CONTAS_PAGAS>();
            this.DESPESA_RECORRENTE = new HashSet<DESPESA_RECORRENTE>();
        }
    
        public int ID { get; set; }
        public string DESCRICAO { get; set; }
        public int MASCARA { get; set; }
        public int CLASSE { get; set; }
        public string TIPO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMPRA> COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO> PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAIXINHA_ITEM> CAIXINHA_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_SERVICO_TERCEIRO> OSSB_SERVICO_TERCEIRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTAS_PAGAR> CONTAS_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTAS_PAGAS> CONTAS_PAGAS { get; set; }
        public virtual DESPESA_CLASSE DESPESA_CLASSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESPESA_RECORRENTE> DESPESA_RECORRENTE { get; set; }
    }
}
