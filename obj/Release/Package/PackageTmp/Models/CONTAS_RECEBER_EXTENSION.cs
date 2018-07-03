using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;

namespace ATIMO.Models
{
    public partial class CONTAS_RECEBER
    {
        public string VALOR_BRUTO_STRING
        {
            get
            {
                return VALOR_BRUTO.ToString("N2");
            }
            set
            {
                VALOR_BRUTO = Convert.ToDecimal(value);
            }
        }

        public string VALOR_LIQUIDO_STRING
        {
            get
            {
                return VALOR_LIQUIDO.ToString("N2");
            }
            set
            {
                VALOR_LIQUIDO = Convert.ToDecimal(value);
            }
        }
    }
}