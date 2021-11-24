using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Commands
{
    interface ICommand
    {
        Resultado Execute(EntidadeDominio entidade);
    }
}
