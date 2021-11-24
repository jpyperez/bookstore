using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class AutorDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Autor autor = (Autor)entidade;

            string commandText = "INSERT INTO autor (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", autor.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Autor autor = (Autor)entidade;

            var commandText = "UPDATE autor SET nome = @nome where id_autor = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", autor.Id},
                { "nome", autor.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Autor autor = (Autor)entidade;
            var autors = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM autor ";
            if (autor.Id > 0)
            {
                strQuery += "WHERE id_autor = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", autor.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempAutor = new Autor
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_autor"]) ? row["id_autor"] : "0"),
                    Nome = row["nome"]
                };
                autors.Add(tempAutor);
            }

            return autors;
        }
    }
}