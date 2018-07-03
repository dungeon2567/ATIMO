using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class FuncionarioCartaoPontoSearchViewModel
    {
        public Int32 Funcionario
        {
            get;
            set;
        }

        public IEnumerable<FUNCIONARIO_CARTAO_PONTO> Items
        {
            get;
            set;
        }
    }
}