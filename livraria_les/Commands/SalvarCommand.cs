using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Commands
{
    class SalvarCommand : AbstractCommand
    {
        public override Resultado Execute(EntidadeDominio entidade)
        {
            return fachada.Salvar(entidade);
        }
    }
}