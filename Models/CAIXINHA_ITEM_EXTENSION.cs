using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class CAIXINHA_ITEM
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
    }
}