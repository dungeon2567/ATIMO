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
    
    public partial class LOJA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LOJA()
        {
            this.CONTRATO_LOJA = new HashSet<CONTRATO_LOJA>();
            this.OSSBs = new HashSet<OSSB>();
            this.LOJA_EQUIPAMENTO = new HashSet<LOJA_EQUIPAMENTO>();
        }
    
        public int ID { get; set; }
        public string NUM_DOC { get; set; }
        public string CEP { get; set; }
        public string ENDERECO { get; set; }
        public string COMPLEMENTO { get; set; }
        public string BAIRRO { get; set; }
        public string CIDADE { get; set; }
        public string UF { get; set; }
        public string ZONA { get; set; }
        public string TELEFONE1 { get; set; }
        public string TELEFONE2 { get; set; }
        public string TELEFONE3 { get; set; }
        public string CONTATO { get; set; }
        public string EMAIL { get; set; }
        public string OBSERVACAO { get; set; }
        public string SITUACAO { get; set; }
        public string APELIDO { get; set; }
        public int CLIENTE { get; set; }
        public string INSC_ESTADUAL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRATO_LOJA> CONTRATO_LOJA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB> OSSBs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOJA_EQUIPAMENTO> LOJA_EQUIPAMENTO { get; set; }
        public virtual PESSOA CLIENTE1 { get; set; }
    }
}