using System;
using System.Collections.Generic;
using ATIMO.Models;

namespace ATIMO.ViewModel
{
    public class ContratoServicoViewModel
    {
        public List<CONTRATO_SERVICO> Lista { get; set; }
        public string Erro { get; set; }
        public int Contrato { get; set; }
        public string TipoOs { get; set; }
        public int Id { get; set; }
        public string AreaManutencaoDescricao { get; set; }
        public string ServicoDescricao { get; set; }
        public int Unidade { get; set; }
        public int Terceiro { get; set; }
        public string Quantidade { get; set; }
        public string ValorMo { get; set; }
        public string ValorMat { get; set; }
        public string ValorTerceiro { get; set; }
    }
}