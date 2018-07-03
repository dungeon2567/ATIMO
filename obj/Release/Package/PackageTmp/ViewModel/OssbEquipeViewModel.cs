using ATIMO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class OssbEquipeViewModel
    {
        public int OSSB
        {
            get;
            set;
        }

        public IEnumerable<OSSB_TECNICO> EQUIPE
        {
            get;
            set;
        }
    }
}