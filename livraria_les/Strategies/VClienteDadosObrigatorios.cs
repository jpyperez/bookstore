using livraria_les.Models;
using System.Text.RegularExpressions;

namespace livraria_les.Strategies
{
    class VClienteDadosObrigatorios : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";

            var cliente = new Cliente();

            if (entidade is Cliente)
            {
                cliente = (Cliente)entidade;
            }
            else 
                return null;

            if (string.IsNullOrEmpty(cliente.Nome) || string.IsNullOrWhiteSpace(cliente.Nome))
                message += "nome, ";

            if (string.IsNullOrEmpty(cliente.Genero) || string.IsNullOrWhiteSpace(cliente.Genero))
                message += "gênero, ";

            if (cliente.DtNascimento == null)
                message += "data de nascimento, ";

            if (string.IsNullOrEmpty(cliente.CPF) || string.IsNullOrWhiteSpace(cliente.CPF))
                message += "CPF, ";

            if (string.IsNullOrEmpty(cliente.Telefone.Numero))
                message += "telefone, ";

            if (message != "")
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
                return message.Substring(0, message.Length - 2) + " são de preenchimento obrigatório!";
            }

            return null;
        }
    }
}