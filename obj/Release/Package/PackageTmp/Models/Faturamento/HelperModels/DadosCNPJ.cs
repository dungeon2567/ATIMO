namespace ATIMO.Models
{
    public class DadosCNPJ
    {

        public DadosCNPJ(string cnpj, string razaosocial, string nomefantasia, string endereco, string bairro, string cep, string cnae, string cidade, string estado)
        {
            Cnpj = cnpj;
            Razaosocial = razaosocial;
            NomeFantasia = nomefantasia;
            Endereco = endereco;
            Bairro = bairro;
            Cep = cep;
            Cnae = cnae;
            Cidade = cidade;
            Estado = estado;
        }

        public string Cnpj { get; private set; }

        public string Razaosocial { get; private set; }

        public string NomeFantasia { get; private set; }

        public string Endereco { get; private set; }

        public string Bairro { get; private set; }

        public string Cep { get; private set; }

        public string Cnae { get; private set; }

        public string Cidade { get; private set; }

        public string Estado { get; private set; }
    }
}