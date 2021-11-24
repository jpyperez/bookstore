using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Commands
{
    class AlterarCommand : AbstractCommand
    {
        public override Resultado Execute(EntidadeDominio entidade)
        {
            return fachada.Alterar(entidade);
        }
    }
}