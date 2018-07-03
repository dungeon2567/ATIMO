using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class LojaEquipamentoViewModel
    {
        public IEnumerable<LOJA_EQUIPAMENTO> EQUIPAMENTOS
        {
            get;
            set;
        }

        public Int32 LOJA
        {
            get;
            set;
        }
    }
}