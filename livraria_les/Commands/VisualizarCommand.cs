using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Commands
{
    class VisualizarCommand : AbstractCommand
    {
        public override Resultado Execute(EntidadeDominio entidade)
        {
            return fachada.Visualizar(entidade);
        }
    }
}