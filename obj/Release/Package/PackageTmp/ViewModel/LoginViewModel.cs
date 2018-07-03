using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ATIMO.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
    }
}