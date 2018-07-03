using System;

namespace ATIMO.ViewModel
{
    public class OssbViewModel
    {
        public int Id { get; set; }
        public int Contrato { get; set; }
        public string DataCadastro { get; set; }
        public int Projeto { get; set; }
        public int? Orcamentista { get; set; }
        public int? Responsavel { get; set; }
        public int Cliente { get; set; }
        public int? Loja { get; set; }
        public string Ocorrencia { get; set; }
        public string Porte { get; set; }
        public string Ambiente { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public string ExecucaoInicio { get; set; }
        public string ExecucaoFim { get; set; }
        public string PrevisaoInicio { get; set; }
        public string PrevisaoFim { get; set; }
        public string Observacao { get; set; }
        public string Erro { get; set; }
    }
}