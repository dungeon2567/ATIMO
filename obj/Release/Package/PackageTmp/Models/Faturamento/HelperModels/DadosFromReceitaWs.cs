using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using ATIMO.Helpers;

namespace ATIMO.Models
{
    public class DadosFromReceitaWs
    {

        public class text_code
        {
            public string text { get; set; }
            public string code { get; set; }
        }

        public List<text_code> atividade_principal { get; set; }
        public string data_situacao { get; set; }
        public string complemento { get; set; }
        public string tipo { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public string email { get; set; }
        public string situacao { get; set; }
        public string bairro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string cep { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string abertura { get; set; }
        public string natureza_juridica { get; set; }
        public string cnpj { get; set; }
        public DateTime ultima_atualizacao { get; set; }
        public string status { get; set; }
        public string fantasia { get; set; }
        public string efr { get; set; }
        public string motivo_situacao { get; set; }
        public string situacao_especial { get; set; }
        public string data_situacao_especial { get; set; }
        public List<text_code> atividades_secundarias { get; set; }
        public string message { get; set; }

        public static DadosFromReceitaWs getDados(string astrCNPJ)
        {
            DadosFromReceitaWs lobjRet = new DadosFromReceitaWs();
            string lstrUrl = "http://receitaws.com.br/v1/cnpj/" + Utils.LimpaCpfCnpj(astrCNPJ);

            WebClient lobjWc = new WebClient();
            lobjWc.Encoding = Encoding.UTF8;
            string lstrJsonRet = lobjWc.DownloadString(lstrUrl);

            lobjRet = JsonConvert.DeserializeObject<DadosFromReceitaWs>(lstrJsonRet);

            if (lobjRet.cep != null)
                lobjRet.cep = lobjRet.cep.Replace(".", "");

            return lobjRet;
        }

        internal string getEnderecoCompleto()
        {
            string lstrRet = logradouro + ", " + numero + " " + complemento;
            return lstrRet;
        }
    }
}