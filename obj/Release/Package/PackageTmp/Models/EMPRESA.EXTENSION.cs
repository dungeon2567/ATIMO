using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class EMPRESA
    {
        public string PERC_CARGA_TRIB_STRING
        {
            get
            {
                return PERC_CARGA_TRIB?
                    .ToString("N2");
            }
            set
            {
                PERC_CARGA_TRIB = decimal.Parse(value);
            }
        }

        public IEnumerable<String> FAT_SERVICO_PMSP_ID
        {
            get
            {
                return FAT_SERVICOS_PMSP.Select(s => s.ID);
            }
            set
            {
                FAT_SERVICOS_PMSP.Clear();

              foreach(var id in value)
                {
                    FAT_SERVICOS_PMSP.Add(new FAT_SERVICOS_PMSP() { ID = id });
                }
            }
        }
    }
}