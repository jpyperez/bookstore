using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class EditoraDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Editora editora = (Editora)entidade;

            string commandText = "INSERT INTO editora (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", editora.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Editora editora = (Editora)entidade;

            var commandText = "UPDATE editora SET nome = @nome where id_editora = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", editora.Id},
                { "nome", editora.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Editora editora = (Editora)entidade;
            var editoras = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM editora ";
            if (editora.Id > 0)
            {
                strQuery += "WHERE id_editora = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", editora.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEditora = new Editora
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_editora"]) ? row["id_editora"] : "0"),
                    Nome = row["nome"]
                };
                editoras.Add(tempEditora);
            }

            return editoras;
        }
    }
}