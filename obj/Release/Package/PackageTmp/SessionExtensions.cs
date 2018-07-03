using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ATIMO.Models;

namespace ATIMO
{
    public static class SessionExtensions
    {
        public static Boolean IsFuncionario(this HttpSessionStateBase session)
        {
            var user = session.Usuario();

            if (user == null)
                return false;

            return user.FUNCIONARIO == 1;
        }

        public static Boolean IsAdministrador(this HttpSessionStateBase session)
        {
            var user = session.Usuario();

            if (user == null)
                return false;

            return user.ADMINISTRADOR == 1;
        }


        public static PESSOA Usuario(this HttpSessionStateBase session)
        {
            return session["USUARIO"] as Models.PESSOA;
        }

        public static Int32 QuantidadeTarefas(this HttpSessionStateBase session)
        {
            int usuarioId = session.UsuarioId();

            using (var db = new ATIMOEntities())
            {
                return db
                   .TAREFA_MEMBRO
                   .Where(tm => tm.MEMBRO == usuarioId)
                   .Join(db.TAREFAs, tm => tm.TAREFA, t => t.ID, (tm, t) => t)
                   .Where(t => t.SITUACAO == "P")
                    .Count();
            }
        }

        public static Int32 UsuarioId(this HttpSessionStateBase session)
        {
            var user = session["USUARIO"] as Models.PESSOA;

            if (user == null)
                throw new NotSupportedException();

            return user.ID;
        }
    }
}