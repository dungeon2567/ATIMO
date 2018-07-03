using ATIMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class CaixinhaDevolvidaViewModel
    {
        public IEnumerable<CAIXINHA_ITEM> ITEMS
        {
            get;
            set;
        }

        public Int32 PESSOA
        {
            get;
            set;
        }
    }
}