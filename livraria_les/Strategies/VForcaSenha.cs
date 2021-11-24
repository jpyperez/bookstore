using livraria_les.Models;
using System.Text.RegularExpressions;

namespace livraria_les.Strategies
{
    class VForcaSenha : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";

            var usuario = new Usuario();
            var cliente = new Cliente();

            if (entidade is Usuario)
            {
                usuario = (Usuario)entidade;
            }
            else if(entidade is Cliente)
            {
                cliente = (Cliente)entidade;
                usuario = cliente.Usuario;
            }
            else 
                return null;

            if (usuario.Senha.Length < 8)
                message += "menor que 8 caracteres, ";

            if (!Regex.IsMatch(usuario.Senha, @"[a-z]+"))
                message += "sem caracteres minúsculas, ";

            if (!Regex.IsMatch(usuario.Senha, @"[A-Z]+"))
                message += "sem caracteres maiusculas, ";

            if (!Regex.IsMatch(usuario.Senha, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
                message += "sem caracter especial, ";

            if (message != "")
            {
                message = "A senha está " + message;
                return message.Substring(0, message.Length - 2);
            }

            return null;
        }
    }
}