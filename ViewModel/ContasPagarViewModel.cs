using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ContasPagarViewModel
    {
        public DateTime? DATA_PAGAMENTO
        {
            get;
            set;
        }

        public DESPESA DESPESA
        {
            get;
            set;
        }

        public Decimal VALOR
        {
            get;
            set;
        }
    }
}