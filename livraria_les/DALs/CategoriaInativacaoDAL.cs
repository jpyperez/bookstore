using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class CategoriaInativacaoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            CategoriaInativacao categoriaInativacao = (CategoriaInativacao)entidade;

            string commandText = "INSERT INTO categoria_inativacao (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", categoriaInativacao.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            CategoriaInativacao categoriaInativacao = (CategoriaInativacao)entidade;

            var commandText = "UPDATE categoriaInativacao SET nome = @nome where id_categoria_inativacao = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", categoriaInativacao.Id},
                { "nome", categoriaInativacao.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            CategoriaInativacao categoriaInativacao = (CategoriaInativacao)entidade;
            var categoriaInativacaos = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM categoria_inativacao ";
            if (categoriaInativacao.Id > 0)
            {
                strQuery += "WHERE id_categoria_inativacao = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", categoriaInativacao.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCategoriaInativacao = new CategoriaInativacao
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_categoria_inativacao"]) ? row["id_categoria_inativacao"] : "0"),
                    Nome = row["nome"]
                };
                categoriaInativacaos.Add(tempCategoriaInativacao);
            }

            return categoriaInativacaos;
        }
    }
}