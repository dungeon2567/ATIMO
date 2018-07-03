using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ExtratoViewModel
    {
        public class TRANSACAO
        {
            public DateTime DATA { get; set; }
            public decimal VALOR { get;          set; }
            public CONTA_BANCARIA CONTA_BANCARIA { get;             set;}
            public String PESSOA { get; set; }
            public Int32? OSSB { get; set; }
            public String NOTA_FISCAL { get; set; }
            public String DESPESA { get; set; }
        }

        public Dictionary<CONTA_BANCARIA, IEnumerable<TRANSACAO> > TRANSACOES
        {
            get;
            set;
        }

        public Dictionary<Int32, Decimal> SALDO
        {
            get;
            set;
        }
    }
}