using livraria_les.Models;
using System.Collections.Generic;

namespace livraria_les.Aplicacao
{
    public class Resultado : EntidadeAplicacao
    {
        public string Message { get; set; }
        public List<IEntidade> Entidades { get; set; }
    }
}