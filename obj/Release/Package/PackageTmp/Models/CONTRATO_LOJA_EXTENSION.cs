using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class CONTRATO_LOJA
    {
        public string VALOR_CONTRATO_STRING
        {
            get
            {
                return VALOR_CONTRATO.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                VALOR_CONTRATO = decimal.Parse(value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string VALOR_EXTRA_STRING
        {
            get
            {
                return VALOR_EXTRA.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                VALOR_EXTRA = decimal.Parse(value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string VALOR_TERCEIRO_STRING
        {
            get
            {
                return VALOR_TERCEIRO.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                VALOR_TERCEIRO = decimal.Parse(value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}