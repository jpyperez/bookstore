using livraria_les.Models;

namespace livraria_les.Strategies
{
    class VLivroDadosObrigatorios : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            string message = "";

            var livro = new Livro();

            if (entidade is Livro)
            {
                livro = (Livro)entidade;
            }
            else 
                return null;

            if (string.IsNullOrEmpty(livro.Titulo) || string.IsNullOrWhiteSpace(livro.Titulo))
                message += "título, ";

            if (livro.Autor.Id == 0)
                message += "Autor, ";

            //if (livro.Categorias.Count == 0)
            //    message += "Categoria, ";

            if (livro.Editora.Id == 0)
                message += "Editora, ";

            if (string.IsNullOrEmpty(livro.NumPag) || string.IsNullOrWhiteSpace(livro.NumPag))
                message += "número de páginas, ";

            if (string.IsNullOrEmpty(livro.Sinopse) || string.IsNullOrWhiteSpace(livro.Sinopse))
                message += "sinopse, ";

            if (string.IsNullOrEmpty(livro.Dimensoes) || string.IsNullOrWhiteSpace(livro.Dimensoes))
                message += "dimensões, ";

            if (livro.GrupoPrecificacao.Id == 0)
                message += "Grupo de precificação, ";

            if (livro.Peso == 0)
                message += "peso, ";

            if (string.IsNullOrEmpty(livro.CodBarras) || string.IsNullOrWhiteSpace(livro.CodBarras))
                message += "código de barras, ";

            if (message != "")
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
                return message.Substring(0, message.Length - 2) + " são de preenchimento obrigatório!";
            }

            return null;
        }
    }
}