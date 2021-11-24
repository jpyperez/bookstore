using System.Collections.Generic;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class Livro : EntidadeDominio
    {
        public Livro()
        {
            Autor = new Autor();
            Editora = new Editora();
            Funcionario = new Funcionario();
            Categorias = new List<Categoria>();
            Subcategoria = new Subcategoria();
            GrupoPrecificacao = new GrupoPrecificacao();
            CategoriaAtivacao = new CategoriaAtivacao();
            CategoriaInativacao = new CategoriaInativacao();
            AlteraEstoque = false;
            Historicos = new List<Livro>();
        }

        public string Titulo { get; set; }
        public string Ano { get; set; }
        [DisplayName("Edição")]
        public string Edicao { get; set; }
        public Autor Autor { get; set; }
        public Editora Editora { get; set; }
        public string ISBN { get; set; }
        [DisplayName("Código de barras")]
        public string CodBarras { get; set; }
        [DisplayName("Número de páginas")]
        public string NumPag { get; set; }
        public string Dimensoes { get; set; }
        public string Sinopse { get; set; }
        [DisplayName("Peso (Kg)")]
        public double Peso { get; set; }
        public string Status { get; set; }
        public Funcionario Funcionario { get; set; }
        [DisplayName("Categoria")]
        public List<Categoria> Categorias { get; set; }
        public Subcategoria Subcategoria { get; set; }
        [DisplayName("Grupo de precificação")]
        public GrupoPrecificacao GrupoPrecificacao { get; set; }
        [DisplayName("Categoria de ativação")]
        public CategoriaAtivacao CategoriaAtivacao { get; set; }
        [DisplayName("Categoria de inativação")]
        public CategoriaInativacao CategoriaInativacao { get; set; }
        public string Justificativa { get; set; }
        [DisplayName("Valor da venda")]
        public double ValorVenda { get; set; }
        [DisplayName("Valor da venda")]
        public double ValorNovaVenda { get; set; }
        [DisplayName("Valor da compra")]
        public double ValorCompra { get; set; }
        public int Estoque { get; set; }
        public bool AlteraEstoque { get; set; }
        public List<Livro> Historicos { get; set; }
    }
}