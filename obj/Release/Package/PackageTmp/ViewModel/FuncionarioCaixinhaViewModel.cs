using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class FuncionarioCaixinhaViewModel
    {
        public IEnumerable<CAIXINHA> CAIXINHAS
        {
            get;
            set;
        }

        public IEnumerable<CAIXINHA_ITEM> CAIXINHAS_DEVOLVIDAS
        {
            get;
            set;
        }
    }
}