using livraria_les.Models;

namespace livraria_les.Strategies
{
    interface IStrategy
    {
        string Processar(IEntidade entidade);
    }
}
