using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class OssbServicoTerceiroIndexViewModel
    {
        public int OSSB_SERVICO
        {
            get;
            set;
        }

        public int OSSB
        {
            get;
            set;
        }

        public IEnumerable<OSSB_SERVICO_TERCEIRO> ITEMS
        {
            get;
            set;
        }

    }
}