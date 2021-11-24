using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class CategoriaAtivacaoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            CategoriaAtivacao categoriaAtivacao = (CategoriaAtivacao)entidade;

            string commandText = "INSERT INTO categoria_ativacao (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", categoriaAtivacao.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            CategoriaAtivacao categoriaAtivacao = (CategoriaAtivacao)entidade;

            var commandText = "UPDATE categoriaAtivacao SET nome = @nome where id_categoria_ativacao = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", categoriaAtivacao.Id},
                { "nome", categoriaAtivacao.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            CategoriaAtivacao categoriaAtivacao = (CategoriaAtivacao)entidade;
            var categoriaAtivacaos = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM categoria_ativacao ";
            if (categoriaAtivacao.Id > 0)
            {
                strQuery += "WHERE id_categoria_ativacao = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", categoriaAtivacao.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCategoriaAtivacao = new CategoriaAtivacao
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_categoria_ativacao"]) ? row["id_categoria_ativacao"] : "0"),
                    Nome = row["nome"]
                };
                categoriaAtivacaos.Add(tempCategoriaAtivacao);
            }

            return categoriaAtivacaos;
        }
    }
}