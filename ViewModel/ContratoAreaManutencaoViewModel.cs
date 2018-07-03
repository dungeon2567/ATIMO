using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ContratoAreaManutencaoViewModel
    {
        public List<CONTRATO_AREA_MANUTENCAO> Lista { get; set; }
        public int Cliente { get; set; }

        public int Id { get; set; }
        public int Contrato { get; set; }
        public int Area_Manutencao { get; set; }
        public string Situacao { get; set; }
    }
}