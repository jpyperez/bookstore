
using livraria_les.Models;
using System.Text.RegularExpressions;

namespace livraria_les.Strategies
{
    class VEnderecoDadosObrigatorios : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";

            var endereco = new Endereco();

            if (entidade is Endereco)
            {
                endereco = (Endereco)entidade;
            }
            else 
                return null;

            //if (endereco.TipoResidencia.Id == 0)
            //    message += "tipo de residência, ";

            if (string.IsNullOrEmpty(endereco.TipoLogradouro) || string.IsNullOrWhiteSpace(endereco.TipoLogradouro))
                message += "tipo de logradouro, ";
            
            if (string.IsNullOrEmpty(endereco.Logradouro) || string.IsNullOrWhiteSpace(endereco.Logradouro))
                message += "logradouro, ";

            if (string.IsNullOrEmpty(endereco.Numero) || string.IsNullOrWhiteSpace(endereco.Numero))
                message += "número, ";

            if (string.IsNullOrEmpty(endereco.Bairro) || string.IsNullOrWhiteSpace(endereco.Bairro))
                message += "bairro, ";

            if (string.IsNullOrEmpty(endereco.Cidade) || string.IsNullOrWhiteSpace(endereco.Cidade))
                message += "cidade, ";

            if (string.IsNullOrEmpty(endereco.Estado) || string.IsNullOrWhiteSpace(endereco.Estado))
                message += "estado, ";

            if (string.IsNullOrEmpty(endereco.Pais) || string.IsNullOrWhiteSpace(endereco.Pais))
                message += "pais, ";

            if (message != "")
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
                return message.Substring(0, message.Length - 2) + " são de preenchimento obrigatório!";
            }

            return null;
        }
    }
}