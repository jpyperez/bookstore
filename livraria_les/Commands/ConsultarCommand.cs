using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Commands
{
    class ConsultarCommand : AbstractCommand
    {
        public override Resultado Execute(EntidadeDominio entidade)
        {
            return fachada.Consultar(entidade);
        }
    }
}