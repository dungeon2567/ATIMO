using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ContasReceberIndexViewModel
    {
        public IEnumerable<CONTAS_RECEBER> ITEMS
        {
            get;
            set;
        }

        public DateTime? De
        {
            get;
            set;
        }

        public DateTime? Ate
        {
            get;
            set;
        }
    }
}