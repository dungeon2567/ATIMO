using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models.Faturamento;

namespace ATIMO.Models
{
    public partial class PESSOA
    {

        private HashSet<String> _ACESSOS = null;

        public HashSet<String> ACESSOS
        {
            get
            {
                if (_ACESSOS == null)
                {
                    if (FUNCIONARIO_TIPO_ACESSO != null)
                    {
                        _ACESSOS = new HashSet<string>(FUNCIONARIO_TIPO_ACESSO.ACESSO.Split(',').Select(str => str.Trim()));
                    }
                    else
                    {
                        _ACESSOS = new HashSet<string>();
                    }
                }

                return _ACESSOS;
            }
        }

        public String TIPO
        {
            get
            {
                return String.Join(" - ", TIPOS);
            }
        }


        public String NOME_COMPLETO
        {
            get
            {
                return RAZAO + (RAZAO.Equals(NOME) ? "" : (" (" + NOME + ")"));
            }
        }

        public IEnumerable<ESPECIALIDADE> ESPECIALIDADES
        {
            get
            {
                if (ESPECIALIDADE1 != null)
                    yield return ESPECIALIDADE_1;

                if (ESPECIALIDADE2 != null)
                    yield return ESPECIALIDADE_2;

                if (ESPECIALIDADE3 != null)
                    yield return ESPECIALIDADE_3;
            }
        }

        public bool IS_RESPONSAVEL
        {
            get
            {
                return RESPONSAVEL == 1;
            }
            set
            {
                RESPONSAVEL = value ? 1 : 0;
            }
        }

        public bool IS_CLIENTE
        {
            get
            {
                return CLIENTE == 1;
            }
            set
            {
                CLIENTE = value ? 1 : 0;
            }
        }

        public bool IS_FUNCIONARIO
        {
            get
            {
                return FUNCIONARIO == 1;
            }
            set
            {
                FUNCIONARIO = value ? 1 : 0;
            }
        }

        public bool IS_FORNECEDOR
        {
            get
            {
                return FORNECEDOR == 1;
            }
            set
            {
                FORNECEDOR = value ? 1 : 0;
            }
        }

        public bool IS_TERCEIRO
        {
            get
            {
                return TERCEIRO == 1;
            }
            set
            {
                TERCEIRO = value ? 1 : 0;
            }
        }

        public bool IS_ADMINISTRADOR
        {
            get
            {
                return ADMINISTRADOR == 1;
            }
            set
            {
                ADMINISTRADOR = value ? 1 : 0;
            }
        }

        public bool IS_FINANCEIRO
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public String SALARIO_STRING
        {
            get
            {
                return SALARIO?.ToString("N2");
            }
            set
            {
                SALARIO = decimal.Parse(value);
            }
        }

        public String VALE_ALIMENTACAO_STRING
        {
            get
            {
                return VALE_ALIMENTACAO?.ToString("N2");
            }
            set
            {
                VALE_ALIMENTACAO = decimal.Parse(value);
            }
        }

        public String VALE_TRANSPORTE_STRING
        {
            get
            {
                return VALE_TRANSPORTE?.ToString("N2");
            }
            set
            {
                VALE_TRANSPORTE = decimal.Parse(value);
            }
        }

        public IEnumerable<String> TIPOS
        {
            get
            {
                if (CLIENTE == 1)
                    yield return "CLIENTE";

                if (FUNCIONARIO == 1)
                    yield return "FUNCIONÁRIO";

                if (TERCEIRO == 1)
                    yield return "TERCEIRO";

                if (FORNECEDOR == 1)
                    yield return "FORNECEDOR";
            }
        }

        public static string getDescricaoTipoPessoaTrib(int? aintTipoPessoa)
        {
            string lstrRet = string.Empty;
            //de acordo com a enum enmTipoPessoaTrib
            switch (aintTipoPessoa)
            {
                case 1:
                    lstrRet = "Pessoa Jurídica (Comum)";
                    break;
                case 2:
                    lstrRet = "Pessoa Jurídica (Optante Simples)";
                    break;
                case 3:
                    lstrRet = "Mei (Micro Empreendedor Individual)";
                    break;
                case 4:
                    lstrRet = "Pessoa Física";
                    break;
                case 5:
                    lstrRet = "Pessoa Jurídica (Sem Retenção IR)";
                    break;
                default:
                    break;
            }
            return lstrRet;
        }
    }
}