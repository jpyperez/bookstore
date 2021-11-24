namespace livraria_les.Models
{
    public class Cupom : FormaPagamento
    {
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public int ClienteId { get; set; }
    }
}