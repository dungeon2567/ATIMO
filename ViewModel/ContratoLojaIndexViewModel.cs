using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ContratoLojaIndexViewModel
    {
        public List<CONTRATO_LOJA> Lista { get; set; }
        public int Contrato { get; set; }
        public int Cliente { get; set; }
    }
}