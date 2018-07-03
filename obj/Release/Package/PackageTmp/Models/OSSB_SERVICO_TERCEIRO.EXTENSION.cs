using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
	public partial class OSSB_SERVICO_TERCEIRO
	{
		public string VALOR_STRING
        {
            get { return VALOR.ToString("C"); }
            set { VALOR= Convert.ToDecimal(value); }
        }
	}
}