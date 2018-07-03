using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class RelatorioGastoGanhosViewModel
    {
        public class Item
        {
            public Int32? Ossb
            {
                get;
                set;
            }

            public Decimal Valor
            {
                get;
                set;
            }

            public PESSOA Pessoa
            {
                get;
                set;
            }

            public PROJETO Projeto
            {
                get;
                set;
            }

            public DateTime Data
            {
                get;
                set;
            }

            public String Descricao
            {
                get;
                set;
            }
        }

        public Dictionary<PROJETO, Decimal> ReceitaPorProjeto
        {
            get;set;
        }

        public Dictionary<PROJETO, Decimal> DespesaPorProjeto
        {
            get; set;
        }

        public Dictionary<PROJETO, Decimal> SaldoPorProjeto
        {
            get; set;
        }

        public IEnumerable<Item> Items
        {
            get;
            set;
        }

    }
}