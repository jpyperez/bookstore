using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class EstoqueDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Livro livro = (Livro)entidade;

            string commandText = "INSERT INTO estoque (livro_id, quantidade)" +
                " VALUES (@livro_id, @estoque)";

            var parametros = new Dictionary<string, object>
            {
                { "livro_id", livro.Id },
                { "estoque", livro.Estoque }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Livro livro = (Livro)entidade;

            var commandText = "UPDATE estoque SET quantidade = @quantidade where livro_id = @idLivro";

            var parametros = new Dictionary<string, object>
            {
                { "IdLivro", livro.Id },
                { "quantidade", livro.Estoque }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Estoque estoque = (Estoque)entidade;
            var estoques = new List<IEntidade>();

            string strQuery = "SELECT * FROM estoque WHERE livro_id = @idLivro";

            var parametros = new Dictionary<string, object>
            {
                { "IdLivro", estoque.Id },
            };

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEstoque = new Estoque
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["livro_id"]) ? row["livro_id"] : "0"),
                    Quantidade = Convert.ToInt32(row["quantidade"])
                };
                estoques.Add(tempEstoque);
            }

            return estoques;
        }
    }
}