using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class FornecedorViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string TipoDoc { get; set; }
        public string NumDoc { get; set; }
        public string InscEstadual { get; set; }
        public string Razao { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Zona { get; set; }
        public string Telefone1 { get; set; }
        public string Telefone2 { get; set; }
        public string Telefone3 { get; set; }
        public string Contato { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }
        public string Situacao { get; set; }

        public virtual ICollection<FORNECEDOR_TIPO_MATERIAL> Materiais { get; set; }
    }
}