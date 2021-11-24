using System;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class EntidadeDominio : IEntidade
    {
        public virtual int Id { get; set; }
        [DisplayName("Data de cadastro")]
        public virtual DateTime DtCadastro { get; set; }
        public string Ativo { get; set; }
        public string BuscaAtivo { get; set; } // "" - Todos, 0 - Inativo, 1 - Ativo
    }
}