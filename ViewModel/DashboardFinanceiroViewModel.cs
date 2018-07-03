using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class DashboardFinanceiroViewModel
    {
        public IEnumerable<KeyValuePair<DateTime, decimal>> Saida { get; set; }
        public IEnumerable<KeyValuePair<DateTime, decimal>> Entrada { get; set; }
    }
}