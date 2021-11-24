using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class SubcategoriaDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Subcategoria subcategoria = (Subcategoria)entidade;

            string commandText = "INSERT INTO subcategoria (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", subcategoria.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Subcategoria subcategoria = (Subcategoria)entidade;

            var commandText = "UPDATE subcategoria SET nome = @nome, where id_subcategoria = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", subcategoria.Id},
                { "nome", subcategoria.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Subcategoria subcategoria = (Subcategoria)entidade;
            var subcategorias = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM subcategoria ";
            if (subcategoria.Id > 0)
            {
                strQuery += "WHERE id_subcategoria = @id";
                flgId = true;
            }

            strQuery += flgId ? " AND ativo = 1" : " WHERE ativo = 1";
            strQuery += " order by nome";

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", subcategoria.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempSubcategoria = new Subcategoria
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_subcategoria"]) ? row["id_subcategoria"] : "0"),
                    Nome = row["nome"]
                };
                subcategorias.Add(tempSubcategoria);
            }

            return subcategorias;
        }
    }
}