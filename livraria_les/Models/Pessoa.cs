namespace livraria_les.Models
{
    public class Pessoa : EntidadeDominio
    {
        public Pessoa()
        {
            Usuario = new Usuario();
        }
        public virtual string Nome { get; set; }
        public Usuario Usuario { get; set; }
    }
}