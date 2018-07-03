using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;
namespace ATIMO.ViewModel
{
    public class FuncionarioDocumentoViewModel
    {
        public Int32 FUNCIONARIO
        {
            get;
            set;
        }

        public Dictionary<String, IEnumerable<FUNCIONARIO_DOCUMENTO>> DOCUMENTOS
        {
            get;
            set;
        }
    }
}