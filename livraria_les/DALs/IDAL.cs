using livraria_les.Models;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    interface IDAL
    {
        int Salvar(IEntidade entidade);
        int Alterar(IEntidade entidade);
        int Excluir(IEntidade entidade);
        List<IEntidade> Consultar(IEntidade entidade);
    }
}
