using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class RecorrenciaViewModel
    {
        public int LOJA
        {
            get;
            set;
        }

        public int CLIENTE
        {
            get;
            set;
        }


        public int CONTRATO
        {
            get;set;
        }

        public int PROJETO
        {
            get;
            set;
        }

        public string VALOR { get; set; }
        public string VALOR_EXTRA { get; set; }
        public string DE { get; set; }
        public string ATE { get; set; }

        public int UNIDADE { get; set; }

        public ICollection<DateTime> RECORRENCIA
        {
            get;set;
        }
    }
}