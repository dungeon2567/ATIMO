using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class FluxoCaixaModel
    {

        public decimal EntradaPrevisto
        {
            get;
            set;
        }

        public decimal EntradaRealizado
        {
            get;
            set;
        }


        public decimal SaidaPrevisto
        {
            get;
            set;
        }

        public decimal SaidaRealizado
        {
            get;
            set;
        }

        public decimal ResultadoPrevisto
        {
            get
            {
                return EntradaPrevisto - SaidaPrevisto;
            }
        }

        public decimal ResultadoRealizado
        {
            get
            {
                return EntradaRealizado - SaidaRealizado;
            }
        }

        public DateTime  Data
        {
            get;
            set;
        }
    }
}