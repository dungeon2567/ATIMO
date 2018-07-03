using System;
using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class OssbServicoIndexViewModel
    {
        public OSSB_SERVICO[] Servicos { get; set; }

        public UNIDADE[] Unidades { get; set; }
        public int Ossb { get; set; }
    }
}