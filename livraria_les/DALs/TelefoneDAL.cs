using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class TelefoneDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Telefone telefone = (Telefone)entidade;

            string commandText = "INSERT INTO telefone (tipo, ddd, numero)" +
                " VALUES (@tipo, @ddd, @numero)";

            var parametros = new Dictionary<string, object>
            {
                { "tipo", telefone.Tipo },
                { "ddd", telefone.DDD },
                { "numero", telefone.Numero }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0)
                return 0;

            telefone.Id = conexao.LastInsertedId();
            return 1;
        }

        public override int Alterar(IEntidade entidade)
        {
            Telefone telefone = (Telefone)entidade;

            var commandText = "UPDATE telefone SET tipo = @tipo, ddd = @ddd, numero = @numero where id_telefone = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", telefone.Id},
                { "tipo", telefone.Tipo },
                { "ddd", telefone.DDD },
                { "numero", telefone.Numero }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Telefone telefone = (Telefone)entidade;
            var telefones = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM telefone ";
            if (telefone.Id > 0)
            {
                strQuery += "WHERE id_telefone = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", telefone.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempTelefone = new Telefone
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_telefone"]) ? row["id_telefone"] : "0"),
                    Tipo = row["tipo"],
                    DDD = row["ddd"],
                    Numero = row["numero"]
                };
                telefones.Add(tempTelefone);
            }

            return telefones;
        }
    }
}