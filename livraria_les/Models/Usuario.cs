using System.ComponentModel;

namespace livraria_les.Models
{
    public class Usuario : EntidadeDominio
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        [DisplayName("Confirmar senha")]
        public string ConfirmSenha { get; set; }
        [DisplayName("Senha atual")]
        public string SenhaAtual { get; set; }
        public string Tipo { get; set; }
        public int PessoaId { get; set; }
    }
}