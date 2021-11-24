using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class ItemPedidoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            ItemPedido itemPedido = (ItemPedido)entidade;

            string commandText = "INSERT INTO livro_pedido (livro_id, pedido_id, quantidade, status, dt_insercao)" +
                " VALUES (@livro_id, @pedido_id, @quantidade, @status, @dt_insercao)";

            var parametros = new Dictionary<string, object>
            {
                { "livro_id", itemPedido.Livro.Id },
                { "pedido_id", itemPedido.PedidoId },
                { "quantidade", itemPedido.Quantidade },
                { "status", itemPedido.Status },
                { "dt_insercao", DateTime.Now }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            ItemPedido itemPedido = (ItemPedido)entidade;

            var commandText = "UPDATE livro_pedido SET status = @status where livro_id = @idLivro and pedido_id = @idPedido";

            var parametros = new Dictionary<string, object>
            {
                { "IdLivro", itemPedido.Livro.Id },
                { "IdPedido", itemPedido.PedidoId },
                { "status", itemPedido.Status }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            ItemPedido itemPedido = (ItemPedido)entidade;
            var itemPedidos = new List<IEntidade>();
            bool flgIdPedido = false, flgIdLivro = false, flgStatus = false;
            LivroDAL livDAL = new LivroDAL();

            string strQuery = "SELECT * FROM livro_pedido WHERE ";
            if (itemPedido.PedidoId > 0)
            {
                strQuery += "pedido_id = @idPedido AND  ";
                flgIdPedido = true;
            }
            if (itemPedido.Livro.Id > 0)
            {
                strQuery += "livro_id = @idLivro AND  ";
                flgIdLivro = true;
            }
            if(!string.IsNullOrEmpty(itemPedido.Status))
            {
                if(itemPedido.Status == "Estoque")
                {
                    strQuery += "status in ('Em processamento', 'Aprovado', 'Em transporte', 'Em troca', 'Carrinho') AND  ";
                }
                else
                {
                    strQuery += "status = @status AND  ";
                    flgStatus = true;
                }   
            }

            if (itemPedido.BuscaAtivo == "0")
                strQuery += "ativo = 0 AND  ";
            else if (itemPedido.BuscaAtivo == "1")
                strQuery += "ativo = 1 AND  ";
            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            var parametros = new Dictionary<string, object>();

            if (flgIdPedido)
                parametros.Add("idPedido", itemPedido.PedidoId);
            if (flgIdLivro)
                parametros.Add("idLivro", itemPedido.Livro.Id);
            if (flgStatus)
                parametros.Add("status", itemPedido.Status);
            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempItemPedido = new ItemPedido
                {
                    Livro = livDAL.Consultar(new Livro { Id = Convert.ToInt32(row["livro_id"]) }).ConvertAll(x => (Livro)x)[0],
                    PedidoId = Convert.ToInt32(row["pedido_id"]),
                    Quantidade = Convert.ToInt32(row["quantidade"]),
                    Status = row["status"],
                    Ativo = row["ativo"],
                    DtCadastro = Convert.ToDateTime(row["dt_insercao"])
                };
                itemPedidos.Add(tempItemPedido);
            }

            return itemPedidos;
        }
    }
}