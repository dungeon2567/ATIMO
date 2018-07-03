using System;
using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class OssbServicoViewModel
    {
        public string OssbStatus { get; set; }
        public string Erro { get; set; }
        public int Contrato { get; set; }
        public string TipoOs { get; set; }
        public int? Terceiro1 { get; set; }
        public int? Terceiro2 { get; set; }
        public int? Terceiro3 { get; set; }
        public int Id { get; set; }
        public int Ossb { get; set; }
        public string AreaManutencaoDescricao { get; set; }
        public string ServicoDescricao { get; set; }
        public int Unidade { get; set; }
        public string Quantidade { get; set; }
        public string ValorMa { get; set; }
        public string ValorMo { get; set; }
    }
}