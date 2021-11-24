using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class Pedido : EntidadeDominio
    {
        public Pedido()
        {
            ItensPedido = new List<ItemPedido>();
            Cliente = new Cliente();
            FormasPagamento = new List<FormaPagamento>();
        }

        [DisplayName("Pedido Id")]
        public override int Id { get; set; }
        public List<ItemPedido> ItensPedido { get; set; }
        public Cliente Cliente { get; set; }
        [DisplayName("Endereço de Entrega")]
        public Endereco Endereco { get; set; }
        public List<FormaPagamento> FormasPagamento { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }
        public double TotalPago { get; set; }
        [DisplayName("Data de compra")]
        public override DateTime DtCadastro { get; set; }
    }
}