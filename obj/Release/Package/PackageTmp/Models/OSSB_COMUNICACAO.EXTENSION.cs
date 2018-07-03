using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
	public partial class OSSB_COMUNICACAO
	{
		public string DATA_STRING
        {
			get
            {
                return DATA.ToString("d");
            }
			set
            {
                DATA = DateTime.Parse(value);
            }
        }
	}
}