using ATIMO.Models;
using ATIMO.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Services;
using System.Net;
using Newtonsoft.Json;
using ATIMO;
namespace Atimo.Controllers
{
    public class HomeController : Controller
    {
        const string ReCaptcha_Key = "6LfQ2yYUAAAAACHme91N5j1U-DT5GREs_ZqhlUYj";
        const string ReCaptcha_Secret = "6LfQ2yYUAAAAAMHU2ly7GbcEyYVjQ6DRMfzsrZlv";

        [WebMethod]
        public static string VerifyCaptcha(string response)
        {
            string url = "https://www.google.com/recaptcha/api/siteverify?secret=" + ReCaptcha_Secret + "&response=" + response;

            return (new WebClient())
                .DownloadString(url);
        }


        private readonly ATIMOEntities _db = new ATIMOEntities();

        public ActionResult Index()
        {
            ViewBag.Title = "Login";

            var user = Session["USUARIO"] as ATIMO.Models.PESSOA;

            if (user != null)
            {
                if (user.FUNCIONARIO == 1)
                {
                    return RedirectToAction("Index", "AreaDoFuncionario");
                }
                else
                if (user.CLIENTE == 1)
                {
                    return RedirectToAction("Dashboard", "AreaDoCliente");
                }

            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel viewModel)
        {


            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.Senha))
                {
                    ViewBag.Message = "Senha muito curta.";
                }
                else
                {
                    //         System.Security.Cryptography.MD5CryptoServiceProvider sha256 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                    //       var cryptoSenha = System.Text.Encoding.ASCII.GetString(sha256.ComputeHash(System.Text.Encoding.Unicode.GetBytes(viewModel.Senha)));


                    var user = await _db.PESSOA
                        .Include(p => p.FUNCIONARIO_TIPO_ACESSO)
                        .Where(u => u.EMAIL == viewModel.Email && u.SENHA == viewModel.Senha && u.SITUACAO == "A")
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        Session.Clear();

                        Session.Add("USUARIO", user);

                        if (user.FUNCIONARIO == 1)
                        {
                            return RedirectToAction("Index", "AreaDoFuncionario");
                        }
                        else
                        if (user.CLIENTE == 1)
                        {
                            return RedirectToAction("Dashboard", "AreaDoCliente");
                        }
                        else
                            return HttpNotFound();
                    }
                    else
                    {
                        ViewBag.Message = "Usuario ou senha incorretos.";
                    }

                }
            }


            return View(viewModel);
        }


        public ActionResult AlterarSenha()
        {
            return View(new AlterarSenhaViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> AlterarSenha(AlterarSenhaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(viewModel.SenhaNova))
                {
                    ViewBag.Message = "Senha muito curta.";

                    return View(viewModel);
                }
                else
                if (!string.Equals(viewModel.SenhaNova, viewModel.SenhaNovaConfirmar, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    ViewBag.Message = "Senhas não batem.";

                    return View(viewModel);
                }
                else
                {
                    var pessoa = await _db.PESSOA.Where(pe => pe.EMAIL == viewModel.Email && pe.SENHA == viewModel.SenhaAntiga)
                        .FirstOrDefaultAsync();

                    if (pessoa == null)
                    {
                        ViewBag.Message = "Email ou senha incorretos.";

                        return View(viewModel);
                    }
                    else
                    {
                        pessoa.SENHA = viewModel.SenhaNova
                            .ToUpper();

                        _db.Entry(pessoa).State = EntityState.Modified;

                        await _db.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                ViewBag.Message = "Preencha os campos corretamente.";

                return View(viewModel);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Servicos()
        {
            return View();
        }

    }
}
