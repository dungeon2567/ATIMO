using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class OssbComunicacaoIndexViewModel
    {
        public OSSB_COMUNICACAO[] Items { get; set; }
        public int Ossb { get; set; }

        public string Ocorrencia
        {
            get;set;
        }
    }
}