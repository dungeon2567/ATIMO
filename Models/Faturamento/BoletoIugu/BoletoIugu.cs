using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace ATIMO.Models.Faturamento.BoletoIugu
{
    public class BoletoIugu
    {

        public const string UrlApiIugu = "https://api.iugu.com/v1/";
        public const string ApiKey = "";

        public string invoice_id { get; set; }
        public string email { get; set; }
        public bool keep_dunning { get; set; }
        public List<BoletoIuguItens>  items { get; set; }
        public BoletoIuguPayer payer { get; set; }

        /*
        public static async Task<HttpResponseMessage> GerarBoleto(string astrEmailControle, PESSOA aobjPessoa, List<BoletoIuguItens> aobjItemsBoleto)
        {
            BoletoIugu lobjBoleto = new BoletoIugu();
            lobjBoleto.email = astrEmailControle;
            lobjBoleto.invoice_id = string.Empty;
            lobjBoleto.keep_dunning = true;
            lobjBoleto.items = aobjItemsBoleto;
            lobjBoleto.payer = new BoletoIuguPayer();
            lobjBoleto.payer.address.city = "São Paulo";
            lobjBoleto.payer.address.number = "854";
            lobjBoleto.payer.address.state = "SP";
            lobjBoleto.payer.address.street = "AV CONS CARRAO";
            lobjBoleto.payer.address.zip_code = "03402-000";
            lobjBoleto.payer.cpf_cnpj = "22550292863";
            lobjBoleto.payer.email = "dimasalves@gmail.com";
            lobjBoleto.payer.name = "Dimas Alves dos Santos";
            lobjBoleto.payer.phone = "985804863";
            lobjBoleto.payer.phone_prefix = "11";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, UrlApiIugu + "/charge"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(ApiKey)));

                var content = JsonConvert.SerializeObject(lobjBoleto);
                requestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
                
                var response = await client.SendAsync(requestMessage).ConfigureAwait(false);
                return response;
            }

        }
        */
    }

    public class BoletoIuguItens
    {
        public string description { get; set; }
        public int quantity { get; set; }
        public int price_cents { get; set; }
    }

    public class BoletoIuguPayer
    {

        public BoletoIuguPayer() { }
        

        public string cpf_cnpj { get; set; }
        public string name { get; set; }
        public string phone_prefix { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public BoletoIuguAddress address { get; set; }

    }

    public class BoletoIuguAddress
    {
        public string street { get; set; }
        public string number { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
    }

    public interface IHttpClientWrapper : IDisposable
    {
        /// <summary>
        /// Enviar uma requisição
        /// </summary>
        /// <param name="request">Dados da mensagem da requisição</param>
        /// <returns>resposta da requisição</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }

    /// <summary>
    /// Implementação padrão da interface IHpptClientWrapper da IUGU
    /// </summary>
    public class StandardHttpClient : IHttpClientWrapper
    {
        private readonly HttpClient client;

        public StandardHttpClient()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Enviar uma requisição
        /// </summary>
        /// <param name="requestMessage">Dados da mensagem da requisição</param>
        /// <returns>resposta da requisição</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            var response = await client.SendAsync(requestMessage).ConfigureAwait(false);
            return response;
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}