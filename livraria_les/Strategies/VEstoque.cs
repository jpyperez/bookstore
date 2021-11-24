using livraria_les.DAL;
using livraria_les.Models;

namespace livraria_les.Strategies
{
    class VEstoque : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";
            var itemPedDAL = new ItemPedidoDAL();
            var estDAL = new EstoqueDAL();

            var estoque = new Estoque();

            if (entidade is Estoque)
            {
                estoque = (Estoque)entidade;
            }
            else 
                return null;

            var livroEstoque = estDAL.Consultar(new Estoque { Id = estoque.LivroId }).ConvertAll(x => (Estoque)x)[0];
            var livroReserva = itemPedDAL.Consultar(new ItemPedido
            {
                Livro = new Livro { Id = estoque.LivroId },
                Status = "Estoque"
            }).ConvertAll(x => (ItemPedido)x);

            int estoqueReserva = 0;
            foreach(var item in livroReserva)
                estoqueReserva += item.Quantidade;

            int estoqueLivre = livroEstoque.Quantidade - estoqueReserva;

            if (estoque.Quantidade > estoqueLivre)
                return message += "Estoque insuficiente";

            return null;
        }
    }
}