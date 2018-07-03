using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class OSSB_MATERIAL
    {
        public string QUANTIDADE_STRING
        {
            get { return QUANTIDADE.ToString("N2"); }
            set { QUANTIDADE = Convert.ToDecimal(value); }
        }

        public string VALOR_STRING
        {
            get { return VALOR?.ToString("C"); }
            set { VALOR = value == null ? null : (Decimal?)Convert.ToDecimal(value); }
        }
    }
}