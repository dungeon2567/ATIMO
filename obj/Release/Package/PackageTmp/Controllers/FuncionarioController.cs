using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using ATIMO.Models;
using ATIMO.ViewModel;
using System.Data.Entity.Validation;
using ATIMO;

namespace Atimo.Controllers
{

    public class FuncionarioController : Controller
    {
        private readonly ATIMOEntities _db = new ATIMOEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session.Usuario() == null)
            {
                filterContext.Result = RedirectToAction("", "");
            }
            else
            if (Session.IsFuncionario() || Session.IsAdministrador())
            {
                var acessos = Session.Usuario()
                    .ACESSOS;

                if (acessos.Contains(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + filterContext.ActionDescriptor.ActionName)
                    || acessos.Contains(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName))
                {
                    filterContext.Result = HttpNotFound();
                }
                else
                    base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = HttpNotFound();
            }
        }
    }

}