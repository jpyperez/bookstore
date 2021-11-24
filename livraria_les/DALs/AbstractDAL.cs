using livraria_les.Util;
using System.Collections.Generic;
using livraria_les.Models;

namespace livraria_les.DAL
{
    public abstract class AbstractDAL : IDAL
    {
        protected readonly Conexao conexao;
        protected string table;
        protected string idTable;
        protected bool ctrlTransaction = true;

        public AbstractDAL()
        {
            conexao = new Conexao();
        }

        public AbstractDAL(Conexao conexao, string table, string idTable)
        {
            this.table = table;
            this.idTable = idTable;
            this.conexao = conexao;
        }

        public AbstractDAL(string table, string idTable)
        {
            this.table = table;
            this.idTable = idTable;
        }

        public int Excluir(IEntidade entidade)
        {
            var entidadeDominio = (EntidadeDominio)entidade;
            string strQuery = $"UPDATE {table} SET ativo = '0' WHERE {idTable} = @Id";
            var parametros = new Dictionary<string, object>
            {
                { "Id", entidadeDominio.Id }
            };

            return conexao.ExecutaComando(strQuery, parametros);
        }

        public abstract int Salvar(IEntidade entidade);

        public abstract int Alterar(IEntidade entidade);

        public abstract List<IEntidade> Consultar(IEntidade entidade);
    }
}