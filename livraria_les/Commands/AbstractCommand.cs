using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;

namespace livraria_les.Commands
{
    abstract class AbstractCommand : ICommand
    {
        protected IFachada fachada = new Fachada();

        public abstract Resultado Execute(EntidadeDominio entidade);
    }
}
