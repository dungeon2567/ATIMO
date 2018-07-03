using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class PagamentoTerceiroCreateViewModel
    {
        public HttpPostedFileBase File
        {
            get;
            set;
        }

        public PAGAMENTO_TERCEIRO PAGAMENTO_TERCEIRO
        {
            get;
            set;
        }
    }
}