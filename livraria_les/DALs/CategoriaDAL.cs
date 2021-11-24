using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class CategoriaDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Categoria categoria = (Categoria)entidade;

            string commandText = "INSERT INTO categoria (nome)" +
                " VALUES (@nome)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", categoria.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Categoria categoria = (Categoria)entidade;

            var commandText = "UPDATE categoria SET nome = @nome, where id_categoria = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", categoria.Id},
                { "nome", categoria.Nome }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Categoria categoria = (Categoria)entidade;
            var categorias = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM categoria ";
            if (categoria.Id > 0)
            {
                strQuery += "WHERE id_categoria = @id";
                flgId = true;
            }

            strQuery += flgId ? " AND ativo = 1" : " WHERE ativo = 1";
            strQuery += " order by nome";

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", categoria.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCategoria = new Categoria
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_categoria"]) ? row["id_categoria"] : "0"),
                    Nome = row["nome"]
                };
                categorias.Add(tempCategoria);
            }

            return categorias;
        }

        public List<IEntidade> ConsultarByLivro(IEntidade entidade)
        {
            Livro livro = (Livro)entidade;
            var categorias = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM livro_categoria ";
            if (livro.Id > 0)
            {
                strQuery += "WHERE livro_id = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", livro.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var categoriaId = Convert.ToInt32(row["categoria_id"]);
                var categoria = Consultar(new Categoria { Id = categoriaId })[0];
                categorias.Add(categoria);
            }

            return categorias;
        }
    }
}