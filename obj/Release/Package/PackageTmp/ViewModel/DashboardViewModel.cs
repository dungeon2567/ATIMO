using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ATIMO.ViewModel
{
    public class DashboardViewModel
    {
        public Dictionary<string, int> StatusOs { get; set; }

        public Dictionary<string, int> TipoOs { get; set; }
    }
}