using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class PAGAMENTO
    {
        public string VALOR_STRING
        {
            get
            {
                return VALOR.ToString("N2");
            }
            set
            {
                VALOR = Decimal.Parse(value);
            }
        }

        public Int32 COPIAS
        {
            get;
            set;
        }
    }
}