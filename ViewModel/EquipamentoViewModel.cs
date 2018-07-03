namespace ATIMO.ViewModel
{
    public class EquipamentoViewModel
    {
        public int Id { get; set; }
        public int Grupo { get; set; }
        public int Tipo { get; set; }
        public string Descricao { get; set; }
        public int Marca { get; set; }
        public int Modelo { get; set; }
        public string Fabricante { get; set; }
        public string NumSerie { get; set; }
        public string Observacao { get; set; }
        public string Situacao { get; set; }

        public int IdPatrimonio { get; set; }
        public string Numero { get; set; }
        public string FlgDeprecia { get; set; }
        public string DataAquisicao { get; set; }
        public string ValorCompra { get; set; }
        public string TipoDepreciacao { get; set; }
        public string Depreciacao { get; set; }
        public int Equipamento { get; set; }
    }
}