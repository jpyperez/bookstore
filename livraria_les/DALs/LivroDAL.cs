using livraria_les.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    public class LivroDAL : AbstractDAL
    {
        public LivroDAL()
        {
            table = "livro";
            idTable = "id_livro";
        }

        public override int Salvar(IEntidade entidade)
        {
            EstoqueDAL estDAL = new EstoqueDAL();
            Livro livro = (Livro)entidade;

            string commandText = "INSERT INTO livro (titulo, ano, edicao, isbn, cod_barras, num_pag, dimensoes, dt_cadastro, subcategoria_id, " +
                "valor_compra, valor_venda, autor_id, editora_id, grupo_precificacao_id, sinopse, peso) VALUES (@titulo, @ano, @edicao, @isbn, @cod_barras, " +
                "@num_pag, @dimensoes, @dt_cadastro, @subcategoria_id, @valor_compra, @valor_venda, @autor_id, @editora_id, @grupo_precificacao_id, @sinopse, @peso)";

            var parametros = new Dictionary<string, object>
            {
                { "titulo", livro.Titulo },
                { "ano", livro.Ano },
                { "edicao", livro.Edicao },
                { "isbn", livro.ISBN },
                { "cod_barras", livro.CodBarras },
                { "num_pag", livro.NumPag },
                { "dimensoes", livro.Dimensoes },
                { "dt_cadastro", DateTime.Now },
                { "subcategoria_id", livro.Subcategoria.Id },
                { "autor_id", livro.Autor.Id },
                { "editora_id", livro.Editora.Id },
                { "grupo_precificacao_id", livro.GrupoPrecificacao.Id },
                { "sinopse", livro.Sinopse },
                { "peso", livro.Peso },
                { "valor_compra", livro.ValorCompra },
                { "valor_venda", livro.ValorCompra }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;

            livro.Id = conexao.LastInsertedId();
            
            // Adicionando as categorias do livro
            foreach (var categoria in livro.Categorias)
            {
                commandText = "INSERT INTO livro_categoria (livro_id, categoria_id)" +
                    " VALUES (@livro_id, @categoria_id)";

                parametros = new Dictionary<string, object>
                {
                    { "livro_id", livro.Id },
                    { "categoria_id", categoria.Id }
                };

                if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                    return 0;
            }

            if (estDAL.Salvar(livro) == 0) // Deu erro?
                return 0;
            
            return 1;
        }

        public override int Alterar(IEntidade entidade)
        {
            var estDAL = new EstoqueDAL();
            var livro = (Livro)entidade;
            bool flgCatAtiv = false, flgCatInativ = false, flgJustificativa = false;

            var livroHistorico = Consultar(new Livro { Id = livro.Id })[0];
            if (SalvarHistorico(livroHistorico) == 0) // Deu erro?
                return 0;

            var commandText = "UPDATE livro SET titulo = @titulo, ano = @ano, edicao = @edicao, isbn = @isbn, cod_barras = @cod_barras, " +
                "num_pag = @num_pag, dimensoes = @dimensoes, subcategoria_id = @subcategoria_id, valor_venda = @valor_venda, " +
                "autor_id = @autor_id, editora_id = @editora_id, grupo_precificacao_id = @grupo_precificacao_id, sinopse = @sinopse, ativo = @ativo, " +
                "peso = @peso ";
            if (livro.CategoriaAtivacao.Id > 0)
            {
                commandText += ", categoria_ativacao_id = @categoria_ativacao_id ";
                flgCatAtiv = true;
            }
            else if(livro.CategoriaInativacao.Id > 0)
            {
                commandText += ", categoria_inativacao_id = @categoria_inativacao_id ";
                flgCatInativ = true;
            }
            if (!string.IsNullOrEmpty(livro.Justificativa))
            {
                commandText += ", justificativa = @justificativa ";
                flgJustificativa = true;
            }
            commandText += "where id_livro = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", livro.Id },
                { "titulo", livro.Titulo },
                { "ano", livro.Ano },
                { "edicao", livro.Edicao },
                { "isbn", livro.ISBN },
                { "cod_barras", livro.CodBarras },
                { "num_pag", livro.NumPag },
                { "dimensoes", livro.Dimensoes },
                { "subcategoria_id", livro.Subcategoria.Id },
                { "valor_venda", livro.ValorNovaVenda },
                { "autor_id", livro.Autor.Id },
                { "editora_id", livro.Editora.Id },
                { "grupo_precificacao_id", livro.GrupoPrecificacao.Id },
                { "sinopse", livro.Sinopse },
                { "peso", livro.Peso },
                { "ativo", livro.Ativo }
            };

            if (flgCatAtiv)
                parametros.Add("categoria_ativacao_id", livro.CategoriaAtivacao.Id);
            if (flgCatInativ)
                parametros.Add("categoria_inativacao_id", livro.CategoriaInativacao.Id);
            if (flgJustificativa)
                parametros.Add("justificativa", livro.Justificativa);

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;
            

            if (estDAL.Alterar(livro) == 0 && livro.AlteraEstoque) // Deu erro?
                return 0;

            return 1;
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            EstoqueDAL estDAL = new EstoqueDAL();
            Livro livro = (Livro)entidade;
            AutorDAL autorDAL = new AutorDAL();
            GrupoPrecificacaoDAL grupoPrecificacaoDAL = new GrupoPrecificacaoDAL();
            SubcategoriaDAL subcategoriaDAL = new SubcategoriaDAL();
            EditoraDAL editoraDAL = new EditoraDAL();
            CategoriaAtivacaoDAL categoriaAtivacaoDAL = new CategoriaAtivacaoDAL();
            CategoriaInativacaoDAL categoriaInativacaoDAL = new CategoriaInativacaoDAL();
            CategoriaDAL categoriaDAL = new CategoriaDAL();
            var livros = new List<IEntidade>();
            bool flgId = false, flgBuscaAtivo = false, flgTitulo = false,
                flgAno = false, flgEdicao = false, flgISBN = false;

            string strQuery = "SELECT * FROM livro WHERE ";
            if (livro.Id > 0)
            {
                strQuery += "id_livro = @id AND  ";
                flgId = true;
            }
            if (!string.IsNullOrEmpty(livro.Titulo))
            {
                strQuery += "titulo like @titulo AND  ";
                flgTitulo = true;
            }
            if (!string.IsNullOrEmpty(livro.Ano))
            {
                strQuery += "ano like @ano AND  ";
                flgAno = true;
            }
            if (!string.IsNullOrEmpty(livro.Edicao))
            {
                strQuery += "edicao like @edicao AND  ";
                flgEdicao = true;
            }
            if (!string.IsNullOrEmpty(livro.ISBN))
            {
                strQuery += "isbn like @isbn AND  ";
                flgISBN = true;
            }
            if (!string.IsNullOrEmpty(livro.BuscaAtivo))
            {
                strQuery += "ativo = @ativo AND  ";
                flgBuscaAtivo = true;
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", livro.Id);
            if (flgBuscaAtivo)
                parametros.Add("ativo", livro.BuscaAtivo);
            if (flgTitulo)
                parametros.Add("titulo", "%" + livro.Titulo + "%");
            if (flgAno)
                parametros.Add("ano", "%" + livro.Ano + "%");
            if (flgEdicao)
                parametros.Add("edicao", "%" + livro.Edicao + "%");
            if (flgISBN)
                parametros.Add("isbn", "%" + livro.ISBN + "%");

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var estoque = estDAL.Consultar(new Estoque { Id = livro.Id }).ConvertAll(x => (Estoque)x);
                var tempLivro = new Livro
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_livro"]) ? row["id_livro"] : "0"),
                    Titulo = row["titulo"],
                    Ano = row["ano"],
                    Edicao = row["edicao"],
                    Autor = autorDAL.Consultar(new Autor { Id = Convert.ToInt32(row["autor_id"]) }).ConvertAll(x => (Autor)x)[0],
                    Editora = editoraDAL.Consultar(new Editora { Id = Convert.ToInt32(row["editora_id"]) }).ConvertAll(x => (Editora)x)[0],
                    ValorCompra = Convert.ToDouble(row["valor_compra"]),
                    ValorVenda = Convert.ToDouble(row["valor_venda"]),
                    ISBN = row["isbn"],
                    CodBarras = row["cod_barras"],
                    NumPag = row["num_pag"],
                    Dimensoes = row["dimensoes"],
                    Estoque = estoque.Count != 0 ? estoque[0].Quantidade : 0,
                    Status = row["status"],
                    Ativo = row["ativo"],
                    Sinopse = row["sinopse"],
                    Peso = Convert.ToDouble(row["peso"]),
                    DtCadastro = Convert.ToDateTime(row["dt_cadastro"]),
                    Categorias = categoriaDAL.ConsultarByLivro(new Livro { Id = Convert.ToInt32(row["id_livro"]) }).ConvertAll(x => (Categoria)x),
                    //Funcionario = row["funcionario_id"],
                    Subcategoria = subcategoriaDAL.Consultar(new Subcategoria { Id = Convert.ToInt32(row["subcategoria_id"]) }).ConvertAll(x => (Subcategoria)x)[0],
                    GrupoPrecificacao = grupoPrecificacaoDAL.Consultar(new GrupoPrecificacao { Id = Convert.ToInt32(row["grupo_precificacao_id"]) }).ConvertAll(x => (GrupoPrecificacao)x)[0],
                    CategoriaAtivacao = categoriaAtivacaoDAL.Consultar(new CategoriaAtivacao { Id = Convert.ToInt32(row["categoria_ativacao_id"]) }).ConvertAll(x => (CategoriaAtivacao)x)[0],
                    CategoriaInativacao = categoriaInativacaoDAL.Consultar(new CategoriaInativacao { Id = Convert.ToInt32(row["categoria_inativacao_id"]) }).ConvertAll(x => (CategoriaInativacao)x)[0],
                    Justificativa = row["justificativa"]
                };
                tempLivro.Historicos = ConsultarHistorico(new Livro { Id = Convert.ToInt32(row["id_livro"]) }).ConvertAll(x => (Livro)x);
                livros.Add(tempLivro);
            }

            return livros;
        }

        public int SalvarHistorico(IEntidade entidade)
        {
            var livro = (Livro)entidade;

            string commandText = "INSERT INTO historico (id_class, class_name, json_dados, dt_alteracao)" +
                " VALUES (@id_class, @class_name, @json_dados, @dt_alteracao)";

            var parametros = new Dictionary<string, object>
            {
                { "id_class", livro.Id },
                { "class_name", livro.GetType().Name },
                { "json_dados", JsonConvert.SerializeObject(livro) },
                { "dt_alteracao", DateTime.Now }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public List<IEntidade> ConsultarHistorico(IEntidade entidade)
        {
            var livro = (Livro)entidade;
            var livros = new List<IEntidade>();

            var strQuery = "SELECT * FROM historico where id_class = @id and class_name = @class_name order by dt_alteracao desc";

            var parametros = new Dictionary<string, object> {
                { "id", livro.Id },
                { "class_name", livro.GetType().Name }
            };

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var livroHistorico = JsonConvert.DeserializeObject<Livro>(row["json_dados"]);

                livros.Add(livroHistorico);
            }

            return livros;
        }
    }
}