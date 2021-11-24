using livraria_les.Models;
using System.Text.RegularExpressions;

namespace livraria_les.Strategies
{
    class VPrecificacao : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            var livro = new Livro();

            if (entidade is Livro)
            {
                livro = (Livro)entidade;
            }
            else 
                return null;
            if (livro.ValorNovaVenda == 0)
                return null;

            var novoValorMax = livro.ValorVenda * (1 + ((double)livro.GrupoPrecificacao.Porcentagem / 100));
            var novoValorMin = livro.ValorVenda * (1 - ((double)livro.GrupoPrecificacao.Porcentagem / 100));

            if(livro.ValorNovaVenda > livro.ValorVenda)
            {
                if(livro.ValorNovaVenda > novoValorMax)
                {
                    return "Novo valor de venda maior que o máximo permitido";
                }
            }
            else
            {
                if (livro.ValorNovaVenda < novoValorMin)
                {
                    return "Novo valor de venda maior que o mínimo permitido";
                }
            }

            return null;
        }
    }
}