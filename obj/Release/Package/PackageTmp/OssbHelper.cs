using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO
{
    public static class OssbHelper
    {
        public static string GetTextoSituacao(char situacao)
        {

                switch (situacao)
                {
                    case 'I':
                        return "VISITA INICIAL";
                    case 'O':
                        return "ORÇAR";
                    case 'R':
                        return "ORÇAMENTO";
                    case 'E':
                        return "EXECUTANDO";
                    case 'C':
                        return "CANCELADA";
                    case 'P':
                        return "PARCELAMENTO";
                case 'F':
                    return "FINALIZADA";
                default:
                        throw new NotSupportedException();
                }
            
        }
    }
}