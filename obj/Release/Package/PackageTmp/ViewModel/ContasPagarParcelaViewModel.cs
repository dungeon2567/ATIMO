
using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class CompraParcelaViewModel
    {
        public int ContaPagar { get; set; }

        public IEnumerable<COMPRA_PARCELA> Parcelas { get; set; }
    }
}