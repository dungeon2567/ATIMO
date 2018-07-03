using System;
using ATIMO.Models;

namespace Atimo.Infra
{
    public class FormatarDados
    {
        public static DadosCNPJ MontarObjEmpresa(string cnpj, string resp)
        {
            var empresa = new DadosCNPJ(
                cnpj, RecuperaColunaValor(resp, Coluna.RazaoSocial),
                RecuperaColunaValor(resp, Coluna.NomeFantasia),
                RecuperaColunaValor(resp, Coluna.EnderecoLogradouro) + RecuperaColunaValor(resp, Coluna.EnderecoNumero),
                RecuperaColunaValor(resp, Coluna.EnderecoBairro),
                RecuperaColunaValor(resp, Coluna.EnderecoCep),
                RecuperaColunaValor(resp, Coluna.AtividadeEconomicaPrimaria),
                RecuperaColunaValor(resp, Coluna.EnderecoCidade),
                RecuperaColunaValor(resp, Coluna.EnderecoEstado)
            );

            return empresa;
        }

        private static string RecuperaColunaValor(string pattern, Coluna col)
        {
            var s = pattern.Replace("\n", "").Replace("\t", "").Replace("\r", "");

            switch (col)
            {
                case Coluna.RazaoSocial:
                    {
                        s = StringEntreString(s, "<!-- Início Linha NOME EMPRESARIAL -->", "<!-- Fim Linha NOME EMPRESARIAL -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.NomeFantasia:
                    {
                        s = StringEntreString(s, "<!-- Início Linha ESTABELECIMENTO -->", "<!-- Fim Linha ESTABELECIMENTO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.NaturezaJuridica:
                    {
                        s = StringEntreString(s, "<!-- Início Linha NATUREZA JURÍDICA -->", "<!-- Fim Linha NATUREZA JURÍDICA -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.AtividadeEconomicaPrimaria:
                    {
                        s = StringEntreString(s, "<!-- Início Linha ATIVIDADE ECONOMICA -->", "<!-- Fim Linha ATIVIDADE ECONOMICA -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.AtividadeEconomicaSecundaria:
                    {
                        s = StringEntreString(s, "<!-- Início Linha ATIVIDADE ECONOMICA SECUNDARIA-->", "<!-- Fim Linha ATIVIDADE ECONOMICA SECUNDARIA -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.NumeroDaInscricao:
                    {
                        s = StringEntreString(s, "<!-- Início Linha NÚMERO DE INSCRIÇÃO -->", "<!-- Fim Linha NÚMERO DE INSCRIÇÃO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.MatrizFilial:
                    {
                        s = StringEntreString(s, "<!-- Início Linha NÚMERO DE INSCRIÇÃO -->", "<!-- Fim Linha NÚMERO DE INSCRIÇÃO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoLogradouro:
                    {
                        s = StringEntreString(s, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoNumero:
                    {
                        s = StringEntreString(s, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoComplemento:
                    {
                        s = StringEntreString(s, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoCep:
                    {
                        s = StringEntreString(s, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoBairro:
                    {
                        s = StringEntreString(s, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoCidade:
                    {
                        s = StringEntreString(s, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.EnderecoEstado:
                    {
                        s = StringEntreString(s, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringSaltaString(s, "</b>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.SituacaoCadastral:
                    {
                        s = StringEntreString(s, "<!-- Início Linha SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha SITUACAO CADASTRAL -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.DataSituacaoCadastral:
                    {
                        s = StringEntreString(s, "<!-- Início Linha SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha SITUACAO CADASTRAL -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringSaltaString(s, "</b>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }
                case Coluna.MotivoSituacaoCadastral:
                    {
                        s = StringEntreString(s, "<!-- Início Linha MOTIVO DE SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha MOTIVO DE SITUAÇÃO CADASTRAL -->");
                        s = StringEntreString(s, "<tr>", "</tr>");
                        s = StringEntreString(s, "<b>", "</b>");
                        return s.Trim();
                    }

                default:
                    {
                        return s;
                    }
            }
        }

        private static string StringEntreString(string str, string strInicio, string strFinal)
        {
            var ini = str.IndexOf(strInicio, StringComparison.Ordinal);
            var fim = str.IndexOf(strFinal, StringComparison.Ordinal);

            if (ini > 0)
                ini = ini + strInicio.Length;

            if (fim > 0)
                fim = fim + strFinal.Length;

            var diff = ((fim - ini) - strFinal.Length);

            if ((fim > ini) && (diff > 0))
                return str.Substring(ini, diff);

            return string.Empty;
        }

        private static string StringSaltaString(string str, string strInicio)
        {
            var ini = str.IndexOf(strInicio, StringComparison.Ordinal);

            if (ini <= 0) 
                return str;

            ini = ini + strInicio.Length;
            return str.Substring(ini);
        }

        public string StringPrimeiraLetraMaiuscula(string str)
        {
            return
                str.Length > 0 ?
                    str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1).ToLower() :
                    string.Empty;
        }
    }
}