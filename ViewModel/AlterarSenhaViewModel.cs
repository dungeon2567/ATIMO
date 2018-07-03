using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ATIMO.ViewModel
{
    public class AlterarSenhaViewModel
    {
        public string Email { get; set; }
        public string SenhaAntiga { get; set; }
        public string SenhaNova { get; set; }
        public string SenhaNovaConfirmar { get; set; }
    }
}