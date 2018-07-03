using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class PagarViewModel
    {
        public Int32 Id
        {
            get;
            set;
        }

        public String ValorString
        {
            get
            {
                return Valor.ToString("N2");
            }
            set
            {
                Valor = Decimal.Parse(value);
            }
        }

        public Int32 ContaBancaria
        {
            get;
            set;
        }

        public DateTime DataPagamento
        {
            get;
            set;
        }

        public Int32 FormaPagamento
        {
            get;
            set;
        }

        public Decimal Valor
        {
            get;
            set;
        }
    }
}