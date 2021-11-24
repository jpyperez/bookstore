using System.ComponentModel;

namespace livraria_les.Models
{
    public class Telefone : EntidadeDominio
    {
        [DisplayName("Tipo telefone")]
        public string Tipo { get; set; }
        public string DDD { get; set; }
        public string Numero { get; set; }
    }
}