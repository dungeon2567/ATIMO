using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace ATIMO.ViewModel
{
    public class DashboardAdminViewModel
    {
        public Dictionary<string, int> StatusOs { get; set; }

        public Dictionary<string, int> StatusOsPreventiva { get; set; }

    }
}