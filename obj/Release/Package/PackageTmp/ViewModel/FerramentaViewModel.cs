namespace ATIMO.ViewModel
{
    public class FerramentaViewModel
    {
        public int Id { get; set; }
        public int Grupo { get; set; }
        public int Tipo { get; set; }
        public string Descricao { get; set; }
        public int Minimo { get; set; }
        public string Observacao { get; set; }
        public string FlgConsumivel { get; set; }

        public int IdPatrimonio { get; set; }
        public string Numero { get; set; }
        public string FlgDeprecia { get; set; }
        public string DataAquisicao { get; set; }
        public string ValorCompra { get; set; }
        public string TipoDepreciacao { get; set; }
        public string Depreciacao { get; set; }
        public int Ferramenta { get; set; }
    }
}