using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.ViewModel;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class CaixinhaIndexViewModel
    {
        public PESSOA PESSOA
        {
            get;
            set;
        }

        public decimal LANCADO
        {
            get;
            set;
        }

        public decimal RESTANTE
        {
            get;
            set;
        }
    }
}