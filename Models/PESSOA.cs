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
    
    public partial class PESSOA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PESSOA()
        {
            this.BENEFICIO = new HashSet<BENEFICIO>();
            this.CLIENTE_COMO_CONHECEU = new HashSet<CLIENTE_COMO_CONHECEU>();
            this.CONTRATO_SERVICO = new HashSet<CONTRATO_SERVICO>();
            this.FORNECEDOR_TIPO_MATERIAL = new HashSet<FORNECEDOR_TIPO_MATERIAL>();
            this.FUNCIONARIO_CHECK_LIST = new HashSet<FUNCIONARIO_CHECK_LIST>();
            this.FUNCIONARIO_DEPARTAMENTO = new HashSet<FUNCIONARIO_DEPARTAMENTO>();
            this.OSSB = new HashSet<OSSB>();
            this.OSSB1 = new HashSet<OSSB>();
            this.OSSB2 = new HashSet<OSSB>();
            this.PESSOA_CLIENTE = new HashSet<PESSOA_CLIENTE>();
            this.PESSOA_FUNCIONARIO = new HashSet<PESSOA_FUNCIONARIO>();
            this.RATEIO = new HashSet<RATEIO>();
            this.TERCEIRO_SERVICO = new HashSet<TERCEIRO_SERVICO>();
            this.OSSB_ORCAMENTISTA = new HashSet<OSSB>();
            this.CONTAS_PAGAR1 = new HashSet<COMPRA>();
            this.OSSB_COMUNICACAO1 = new HashSet<OSSB_COMUNICACAO>();
            this.TAREFA_MEMBRO = new HashSet<TAREFA_MEMBRO>();
            this.OSSB_SERVICO_TERCEIRO = new HashSet<OSSB_SERVICO_TERCEIRO>();
            this.OSSB_TECNICO = new HashSet<OSSB_TECNICO>();
            this.ADIANTAMENTOS_TERCEIRO = new HashSet<PAGAMENTO_TERCEIRO>();
            this.PAGAMENTO = new HashSet<PAGAMENTO>();
            this.CAIXINHA = new HashSet<CAIXINHA>();
            this.CAIXINHA_ITEM = new HashSet<CAIXINHA_ITEM>();
            this.FUNCIONARIO_DOCUMENTO = new HashSet<FUNCIONARIO_DOCUMENTO>();
            this.LOJA = new HashSet<LOJA>();
            this.FUNCIONARIO_CARTAO_PONTO = new HashSet<FUNCIONARIO_CARTAO_PONTO>();
            this.CONTAS_PAGAR = new HashSet<CONTAS_PAGAR>();
            this.CONTAS_PAGAS = new HashSet<CONTAS_PAGAS>();
            this.MATERIAL = new HashSet<MATERIAL>();
            this.DESPESA_RECORRENTE = new HashSet<DESPESA_RECORRENTE>();
            this.OSSB_SITUACAO_HISTORICO = new HashSet<OSSB_SITUACAO_HISTORICO>();
            this.CONTA_BANCARIA = new HashSet<CONTA_BANCARIA>();
            this.OSSB_MATERIAL = new HashSet<OSSB_MATERIAL>();
        }
    
        public int ID { get; set; }
        public string TIPO_DOC { get; set; }
        public string NUM_DOC { get; set; }
        public string INSC_ESTADUAL { get; set; }
        public string RAZAO { get; set; }
        public string NOME { get; set; }
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
        public string SITUACAO { get; set; }
        public string OBSERVACAO { get; set; }
        public int FUNCIONARIO { get; set; }
        public int CLIENTE { get; set; }
        public int FORNECEDOR { get; set; }
        public int TERCEIRO { get; set; }
        public string SENHA { get; set; }
        public int COMO_CONHECEU { get; set; }
        public Nullable<int> TIPO_PESSOA_TRIBUTACAO { get; set; }
        public string INSC_MUNICIPAL { get; set; }
        public int RESPONSAVEL { get; set; }
        public Nullable<int> ADMINISTRADOR { get; set; }
        public Nullable<int> DATA_ADMISSAO { get; set; }
        public Nullable<int> DATA_DEMISSAO { get; set; }
        public Nullable<decimal> SALARIO { get; set; }
        public Nullable<System.DateTime> DATA_EFETIVACAO { get; set; }
        public Nullable<decimal> VALE_TRANSPORTE { get; set; }
        public Nullable<decimal> VALE_ALIMENTACAO { get; set; }
        public Nullable<int> PROJETO { get; set; }
        public Nullable<int> ESPECIALIDADE1 { get; set; }
        public Nullable<int> ESPECIALIDADE2 { get; set; }
        public Nullable<int> ESPECIALIDADE3 { get; set; }
        public Nullable<int> TIPO_ACESSO { get; set; }
        public Nullable<int> IMAGEM { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BENEFICIO> BENEFICIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_COMO_CONHECEU> CLIENTE_COMO_CONHECEU { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRATO_SERVICO> CONTRATO_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_TIPO_MATERIAL> FORNECEDOR_TIPO_MATERIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO_CHECK_LIST> FUNCIONARIO_CHECK_LIST { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO_DEPARTAMENTO> FUNCIONARIO_DEPARTAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB> OSSB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB> OSSB1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB> OSSB2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_CLIENTE> PESSOA_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_FUNCIONARIO> PESSOA_FUNCIONARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RATEIO> RATEIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TERCEIRO_SERVICO> TERCEIRO_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB> OSSB_ORCAMENTISTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMPRA> CONTAS_PAGAR1 { get; set; }
        public virtual COMO_CONHECEU COMO_CONHECEU1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_COMUNICACAO> OSSB_COMUNICACAO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TAREFA_MEMBRO> TAREFA_MEMBRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_SERVICO_TERCEIRO> OSSB_SERVICO_TERCEIRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_TECNICO> OSSB_TECNICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO_TERCEIRO> ADIANTAMENTOS_TERCEIRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGAMENTO> PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAIXINHA> CAIXINHA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAIXINHA_ITEM> CAIXINHA_ITEM { get; set; }
        public virtual PROJETO PROJETO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO_DOCUMENTO> FUNCIONARIO_DOCUMENTO { get; set; }
        public virtual ESPECIALIDADE ESPECIALIDADE_1 { get; set; }
        public virtual ESPECIALIDADE ESPECIALIDADE_2 { get; set; }
        public virtual ESPECIALIDADE ESPECIALIDADE_3 { get; set; }
        public virtual FUNCIONARIO_TIPO_ACESSO FUNCIONARIO_TIPO_ACESSO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOJA> LOJA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNCIONARIO_CARTAO_PONTO> FUNCIONARIO_CARTAO_PONTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTAS_PAGAR> CONTAS_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTAS_PAGAS> CONTAS_PAGAS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MATERIAL> MATERIAL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESPESA_RECORRENTE> DESPESA_RECORRENTE { get; set; }
        public virtual ANEXO IMAGEM1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_SITUACAO_HISTORICO> OSSB_SITUACAO_HISTORICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_BANCARIA> CONTA_BANCARIA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OSSB_MATERIAL> OSSB_MATERIAL { get; set; }
    }
}
