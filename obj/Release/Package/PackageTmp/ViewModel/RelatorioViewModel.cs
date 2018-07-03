using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class RelatorioViewModel
    {
        public Int32 Id
        {
            get;
            set;
        }

        public String Descricao
        {
            get;
            set;
        }

        public IEnumerable<RelatorioViewModel> Subitems
        {
            get;
            set;
        }

        public decimal ValorPrevistoBase;
        public decimal ValorPagoBase;

        private decimal? CachePrevisto;
        private decimal? CachePago;

        public Decimal ValorPrevisto
        {
            get
            {
                if(CachePrevisto == null)
                {
                    decimal d = ValorPrevistoBase;

                    foreach(var item in Subitems)
                    {
                        d += item.ValorPrevisto;
                    }

                    CachePrevisto = d;
                }

                return CachePrevisto.Value;
            }
            set
            {
                CachePrevisto = value;
            }
        }

        public Decimal ValorPago
        {
            get
            {
                if (CachePago == null)
                {
                    decimal d = ValorPagoBase;

                    foreach (var item in Subitems)
                    {
                        d += item.ValorPago;
                    }

                    CachePago = d;
                }

                return CachePago.Value;
            }
            set
            {
                CachePago = value;
            }
        }
    }
}