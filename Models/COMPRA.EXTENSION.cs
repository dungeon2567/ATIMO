using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;

namespace ATIMO.Models
{
    public partial class COMPRA
    {
        public Int32 PARCELAS
        {
            get; set;
        }
        public DateTime DATA_VENCIMENTO { get; set; }

        public decimal VALOR
        {
            get
            {
                return COMPRA_ITEM
                    .Select(ci => ci.VALOR * ci.QUANTIDADE)
                    .DefaultIfEmpty()
                    .Sum();
            }
        }

        public String VALOR_STRING
        {
            get { return VALOR.ToString("C"); }
        }

        public String DESCRICAOPROJETO
        {
            get
            {
                if (PROJETO != 0)
                    return "";
                else
                    return "";
            }
        }
    }
}