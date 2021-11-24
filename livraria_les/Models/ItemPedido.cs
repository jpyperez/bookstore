using System.ComponentModel;

namespace livraria_les.Models
{
    public class ItemPedido : EntidadeDominio
    {
        public ItemPedido()
        {
            Livro = new Livro();
        }
        public Livro Livro { get; set; }
        public int Quantidade { get; set; }
        public int PedidoId { get; set; }
        public string Status { get; set; }
    }
}