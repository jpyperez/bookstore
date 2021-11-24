using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class UsuarioDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Usuario usuario = (Usuario)entidade;

            string commandText = "INSERT INTO usuario (email, senha, tipo) VALUES (@email, @senha, @tipo)";

            var parametros = new Dictionary<string, object>
            {
                { "email", usuario.Email },
                { "senha", usuario.Senha },
                { "tipo", usuario.Tipo }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0)
                return 0;

            usuario.Id = conexao.LastInsertedId();
            return 1;
        }

        public override int Alterar(IEntidade entidade)
        {
            Usuario usuario = (Usuario)entidade;

            var commandText = "UPDATE usuario SET email = @email, senha = @senha, tipo = @tipo where id_usuario = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", usuario.Id},
                { "email", usuario.Email },
                { "senha", usuario.Senha },
                { "tipo", usuario.Tipo }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Usuario usuario = (Usuario)entidade;
            var usuarios = new List<IEntidade>();
            bool flgId = false, flgEmail = false;

            string strQuery = "SELECT * FROM usuario WHERE ";
            if (usuario.Id > 0)
            {
                strQuery += "id_usuario = @id AND  ";
                flgId = true;
            }
            if (!string.IsNullOrEmpty(usuario.Email))
            {
                //strQuery += "email like '%" + usuario.Email +"%' AND  ";
                strQuery += "email like @email AND  ";
                flgEmail = true;
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", usuario.Id);
            if (flgEmail)
                parametros.Add("email", "%" + usuario.Email + "%");

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempUsuario = new Usuario
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_usuario"]) ? row["id_usuario"] : "0"),
                    Email = row["email"],
                    Senha = row["senha"],
                    Tipo = row["tipo"],
                    PessoaId = Convert.ToInt32(row["pessoa_id"])
                };
                usuarios.Add(tempUsuario);
            }

            return usuarios;
        }
    }
}