using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class DESPESA
    {
        public string MASCARA_STRING
        {
            get
            {
                return MASCARA.ToString("00000000");
            }
            set
            {

                MASCARA = Int32.Parse(value.PadRight(8, '0'));
            }
        }

        public string CLASSE_DESCRICAO
        {
            get
            {
                return DESPESA_CLASSE.DESCRICAO;
            }
        }

        public Int32 UPPER_MASCARA
        {
            get
            {
                string mascara = MASCARA_STRING;

                Int32 addition = 1;

                for (Int32 it = 7; it >= 0; --it)
                {
                    if (mascara[it] != '0')
                        break;

                    addition *= 10;
                }

                return MASCARA + addition;
            }
        }



    }
}