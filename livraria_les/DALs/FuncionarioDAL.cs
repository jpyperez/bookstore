using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class FuncionarioDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Funcionario funcionario = (Funcionario)entidade;
            UsuarioDAL usuDAL = new UsuarioDAL();

            if (usuDAL.Salvar(funcionario.Usuario) == 0) // Deu erro?
                return 0;

            string commandText = "INSERT INTO funcionario (nome, usuario_id)" +
                " VALUES (@nome, @usuario_id)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", funcionario.Nome },
                { "usuario_id", funcionario.Usuario.Id }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Funcionario funcionario = (Funcionario)entidade;

            var commandText = "UPDATE funcionario SET nome = @nome where id_funcionario = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", funcionario.Id},
                { "nome", funcionario.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Funcionario funcionario = (Funcionario)entidade;
            var funcionarios = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM funcionario ";
            if (funcionario.Id > 0)
            {
                strQuery += "WHERE id_funcionario = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", funcionario.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempFuncionario = new Funcionario
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_funcionario"]) ? row["id_funcionario"] : "0"),
                    Nome = row["nome"]
                };
                funcionarios.Add(tempFuncionario);
            }

            return funcionarios;
        }
    }
}