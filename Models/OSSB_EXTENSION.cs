using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class OSSB
    {
        public String OK
        {
            get
            {
                if (SITUACAO == "I" && DATA_VISITA == null)
                    return "Data da visita não cadastrada!";

                if (PESSOA == null || PESSOA.NUM_DOC == "")
                    return "Documento do Cliente não cadastrado!";

                if (LOJA != null && LOJA1.NUM_DOC == "")
                    return "Documento da Loja não cadastrado!";

                if (SITUACAO == "E")
                {
                    if (!OSSB_CHECK_LIST.Any() || OSSB_CHECK_LIST.Any(ocl => ocl.VISITADO == null))
                        return "Data de término não cadastrada!";
                    /*
                    if (CLIENTE_SATISFACAO == null)
                        return "Pesquisa de satisfação não cadastrada!";

                    if (!OSSB_ALBUM.Any())
                        return "Imagens não cadastradas!";
                        */
                }

                if (SITUACAO == "P")
                {
                    if (CONTAS_RECEBER.Any())
                        return "Os já emitida no contas a receber!";
                }

                return null;
            }
        }

        public string PROXIMA_SITUACAO
        {
            get
            {
                switch (SITUACAO)
                {
                    case "I":
                        return "O";
                    case "O":
                        return "R";
                    case "R":
                        return "E";
                    case "E":
                        return "P";
                    case "P":
                        return "K";
                    case "F":
                        return "F";
                    case "K":
                        return "K";
                    case "C":
                        return "C";
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public string TEXTO_SITUACAO
        {
            get
            {
                switch (SITUACAO)
                {
                    case "I":
                        return "VISITA INICIAL";
                    case "O":
                        return "ORÇAR";
                    case "R":
                        return "ORÇAMENTO";
                    case "E":
                        return "EXECUTANDO";
                    case "C":
                        return "CANCELADA";
                    case "P":
                        return "PARCELAMENTO";
                    case "F":
                        return "FINALIZADA";
                    case "K":
                        return "COBRANÇA";
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        public string TEXTO_TIPO
        {
            get
            {
                switch (TIPO)
                {
                    case "P":
                        return "PREVENTIVA";
                    case "C":
                        return "CORRETIVA";
                    case "O":
                        return "ACOMPANHAMENTO";
                    case "E":
                        return "EMERGENCIAL";
                    case "X":
                        return "EXTRA CONTRATUAL";
                    case "G":
                        return "GARANTIA";

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}