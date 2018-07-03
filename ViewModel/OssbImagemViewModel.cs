using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class OssbImagemViewModel
    {
        public int Ossb
        {
            get;
            set;
        }

        public IEnumerable<OSSB_ALBUM> Items
        {
            get;
            set;
        }
    }
}