using livraria_les.Models;

namespace livraria_les.Strategies
{
    class ValidarCNPJ : IStrategy
    {
        public string Processar(IEntidade entidade)
        {
            //if (entidade is Cliente){
                //Cliente cliente = (Cliente)entidade;

                //if (cliente.getCnpj().length() < 14)
                //{
                //    return "CNPJ deve conter 14 digitos!";
                //}

            //}else{
            //    return "CNPJ não pode ser válidado, pois entidade não é um fornecedor!";
            //}

            return null;
        }
    }
}