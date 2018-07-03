using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class CONTA_BANCARIA
    {
        public String SALDO_INICIAL_STRING
        {
            get
            {
                return SALDO_INICIAL.ToString("0.00");
            }

            set
            {
                SALDO_INICIAL = decimal.Parse(value);
            }
        }

        public String DESCRICAO
        {
            get
            {
                return this.BANCO1.DESCRICAO + " " + this.DIG_CONTA + "-" + this.NUM_CONTA + " " + this.DIG_AGENCIA + "-" + this.NUM_AGENCIA;
            }
        }
    }
}