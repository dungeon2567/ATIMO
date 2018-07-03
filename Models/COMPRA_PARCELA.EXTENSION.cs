using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class COMPRA_PARCELA
    {
        public String VALOR_STRING
        {
            get
            {
                return VALOR.ToString("C");
            }
            set
            {
                VALOR = Convert.ToDecimal(value);
            }
        }
    }
}