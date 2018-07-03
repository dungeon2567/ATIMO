using ATIMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class LojaIndexViewModel
    {
        public IEnumerable<LOJA> LOJAS
        {
            get;
            set;
        }

        public Int32 CLIENTE
        {
            get;
            set;
        }
    }
}