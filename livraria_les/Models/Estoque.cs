using System;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class Estoque : EntidadeDominio
    {
        public Estoque()
        {
            ItemPedido = new ItemPedido();
        }
        public int LivroId { get; set; }
        public int Quantidade { get; set; }
        public ItemPedido ItemPedido { get; set; }
    }
}