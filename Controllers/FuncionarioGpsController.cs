using ATIMO.Models;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;
using ATIMO;

namespace Atimo.Controllers
{
    public class FuncionarioGpsCreate : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        [HttpPost]
        public async Task<ActionResult> Create(string email, string senha, double latitude, double longitude)
        {
            var usuario = await _db.PESSOA.Where(p => p.EMAIL == email && p.SENHA == senha)
                .Select(p => p.ID)
                .FirstOrDefaultAsync();

            if(usuario != 0)
            {
                _db.FUNCIONARIO_CARTAO_PONTO.Add(new FUNCIONARIO_CARTAO_PONTO()
                {
                    FUNCIONARIO = usuario,
                    LATITUDE = latitude,
                    LONGITUDE = longitude,
                    MOMENTO = DateTime.Now,
                    OBSERVACAO = ""
                });

                return Json(new { status = 0 });
            }

            return Json(new { status = 1 });
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
