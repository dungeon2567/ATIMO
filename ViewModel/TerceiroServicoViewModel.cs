using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class TerceiroServicoViewModel
    {
        public List<TERCEIRO_SERVICO> Lista { get; set; }
        public string Erro { get; set; }

        public int Id { get; set; }
        public int Terceiro { get; set; }
        public int Area_Manutencao { get; set; }
        public int Servico { get; set; }
        public string Situacao { get; set; }
    }
}