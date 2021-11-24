using System.ComponentModel;

namespace livraria_les.Models
{
    public class Editora : EntidadeDominio
    {
        [DisplayName("Editora")]
        public string Nome { get; set; }
    }
}