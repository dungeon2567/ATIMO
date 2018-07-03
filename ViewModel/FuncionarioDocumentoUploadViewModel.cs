using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATIMO.ViewModel
{
    public class FuncionarioDocumentoUploadViewModel
    {
        public HttpPostedFileBase FILE
        {
            get;
            set;
        }

        public string CATEGORIA
        {
            get;
            set;
        }
    }
}