using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO
{
    public class RelatorioItem
    {
        public IEnumerable<RelatorioItem> CHILDREN
        {
            get;
            set;
        }

        public string DESCRICAO
        {
            get;
            set;
        }
        public decimal VALOR
        {
            get;
            set;
        }

        public decimal TOTAL
        {
            get;
            set;
        }
    }
}