using System.ComponentModel;

namespace livraria_les.Models
{
    public class Autor : Pessoa
    {
        [DisplayName("Autor")]
        public override string Nome { get; set; }
    }
}