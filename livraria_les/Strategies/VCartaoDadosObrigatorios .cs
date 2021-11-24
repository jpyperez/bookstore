using livraria_les.Models;
using System.Text.RegularExpressions;

namespace livraria_les.Strategies
{
    class VCartaoDadosObrigatorios : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";

            var cartao = new Cartao();

            if (entidade is Cartao)
            {
                cartao = (Cartao)entidade;
            }
            else 
                return null;

            if (string.IsNullOrEmpty(cartao.Numero) || string.IsNullOrWhiteSpace(cartao.Numero))
                message += "número do cartão, ";
            
            if (string.IsNullOrEmpty(cartao.NomeImpresso) || string.IsNullOrWhiteSpace(cartao.NomeImpresso))
                message += "nome impresso no cartão, ";

            if (string.IsNullOrEmpty(cartao.Bandeira) || string.IsNullOrWhiteSpace(cartao.Bandeira))
                message += "bandeira, ";

            if (cartao.CodSeguranca == 0)
                message += "código de segurança, ";

            if (message != "")
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
                return message.Substring(0, message.Length - 2) + " são de preenchimento obrigatório!";
            }

            return null;
        }
    }
}