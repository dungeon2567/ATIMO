using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.Models
{
    public partial class OSSB_SERVICO
    {
        public string QUANTIDADE_STRING
        {
            get { return QUANTIDADE.ToString("N2"); }
            set { QUANTIDADE = Convert.ToDecimal(value); }
        }

        public string VALOR_MO_STRING
        {
            get { return VALOR_MO.ToString("N2"); }
            set { VALOR_MO = Convert.ToDecimal(value); }
        }

        public string VALOR_MO_BDI_STRING
        {
            get { return VALOR_MO_BDI.ToString("N2"); }
            set { VALOR_MO_BDI = Convert.ToDecimal(value); }
        }

        public string VALOR_MA_STRING
        {
            get { return VALOR_MA.ToString("N2"); }
            set { VALOR_MA = Convert.ToDecimal(value); }
        }


        public string VALOR_MA_BDI_STRING
        {
            get { return VALOR_MA_BDI.ToString("N2"); }
            set { VALOR_MA_BDI = Convert.ToDecimal(value); }
        }

        public ICollection<Int32> TERCEIRO
        {
            set
            {
                while(value.Count < OSSB_SERVICO_TERCEIRO.Count)
                {
                    OSSB_SERVICO_TERCEIRO.Add(new OSSB_SERVICO_TERCEIRO());
                }

                for(Int32 it= 0; it < value.Count; ++it)
                {
                    OSSB_SERVICO_TERCEIRO
                        .ElementAt(it)
                        .TERCEIRO = value.ElementAt(it);
                }
            }
            get
            {
                return new List<Int32>(OSSB_SERVICO_TERCEIRO.Select(ost => ost.TERCEIRO));
            }
        }

        public ICollection<String> VALOR
        {
            set
            {
                while (value.Count < OSSB_SERVICO_TERCEIRO.Count)
                {
                    OSSB_SERVICO_TERCEIRO.Add(new OSSB_SERVICO_TERCEIRO());
                }

                for (Int32 it = 0; it < value.Count; ++it)
                {
                    OSSB_SERVICO_TERCEIRO
                        .ElementAt(it)
                        .VALOR = Decimal.Parse(value.ElementAt(it));
                }
            }
            get
            {
                return new List<String>(OSSB_SERVICO_TERCEIRO.Select(ost => ost.VALOR.ToString("N2")));
            }
        }

        public ICollection<String> DATA_VENCIMENTO
        {
            set
            {
                while (value.Count < OSSB_SERVICO_TERCEIRO.Count)
                {
                    OSSB_SERVICO_TERCEIRO.Add(new OSSB_SERVICO_TERCEIRO());
                }

                for (Int32 it = 0; it < value.Count; ++it)
                {
                    OSSB_SERVICO_TERCEIRO
                        .ElementAt(it)
                        .DATE_VENCIMENTO = DateTime.Parse(value.ElementAt(it));
                }
            }
            get
            {
                return new List<String>(OSSB_SERVICO_TERCEIRO.Select(ost => ost.DATE_VENCIMENTO == null ? "" : ost.DATE_VENCIMENTO.Value.ToString("dd/MM/yyyy")));
            }
        }
        
    }
}