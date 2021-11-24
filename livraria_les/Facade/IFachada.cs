using livraria_les.Aplicacao;
using livraria_les.Models;

namespace livraria_les.Facade
{
    public interface IFachada
    {
        Resultado Salvar(IEntidade entidade);
        Resultado Alterar(IEntidade entidade);
        Resultado Excluir(IEntidade entidade);
        Resultado Consultar(IEntidade entidade);
        Resultado Visualizar(IEntidade entidade);
    }
}
