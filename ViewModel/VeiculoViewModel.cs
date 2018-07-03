namespace ATIMO.ViewModel
{
    public class VeiculoViewModel
    {
        public int Id { get; set; }
        public int Marca { get; set; }
        public int Modelo { get; set; }
        public int Ano { get; set; }
        public string Chassi { get; set; }
        public string Placa { get; set; }
        public string Combustivel { get; set; }
        public string Seguro { get; set; }
        public string Ipva { get; set; }
        public string Observacao { get; set; }
        public string Situacao { get; set; }

        public int IdPatrimonio { get; set; }
        public string Numero { get; set; }
        public string FlgDeprecia { get; set; }
        public string DataAquisicao { get; set; }
        public string ValorCompra { get; set; }
        public string TipoDepreciacao { get; set; }
        public string Depreciacao { get; set; }
        public int Veiculo { get; set; }
    }
}