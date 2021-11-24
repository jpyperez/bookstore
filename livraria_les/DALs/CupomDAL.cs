using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class CupomDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Cupom cupom = (Cupom)entidade;

            string commandText = "INSERT INTO cupom (codigo, valor";
            commandText += cupom.ClienteId > 0 ? ", cliente_id" : "";
            commandText += ") VALUES (@codigo, @valor";
            commandText += cupom.ClienteId > 0 ? ", @cliente_id)" : ")";

            var parametros = new Dictionary<string, object>
            {
                { "codigo", cupom.Codigo },
                { "Valor", cupom.Valor }
            };

            if (cupom.ClienteId > 0)
                parametros.Add("@cliente_id", cupom.ClienteId);

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Cupom cupom = (Cupom)entidade;

            var commandText = "UPDATE cupom SET codigo = @codigo, valor = @valor, ativo = @ativo where id_cupom = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", cupom.Id },
                { "codigo", cupom.Codigo },
                { "valor", cupom.Valor },
                { "ativo", cupom.Ativo }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Cupom cupom = (Cupom)entidade;
            var cupoms = new List<IEntidade>();
            bool flgId = false, flgCodigo = false, flgValor = false, flgAtivo = false;

            string strQuery = "SELECT * FROM cupom WHERE ";
            if (cupom.Id > 0)
            {
                strQuery += "id_cupom = @id AND  ";
                flgId = true;
            }
            if (!string.IsNullOrEmpty(cupom.Codigo))
            {
                strQuery += "codigo = @codigo AND  ";
                flgCodigo = true;
            }
            if (cupom.Valor > 0)
            {
                strQuery += "valor = @valor AND  ";
                flgValor = true;
            }
            if (!string.IsNullOrEmpty(cupom.Ativo))
            {
                strQuery += "ativo = @ativo AND  ";
                flgAtivo = true;
            }
            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", cupom.Id);
            if (flgCodigo)
                parametros.Add("codigo", cupom.Codigo);
            if (flgValor)
                parametros.Add("valor", cupom.Valor);
            if (flgAtivo)
                parametros.Add("ativo", cupom.Ativo);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCupom = new Cupom
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_cupom"]) ? row["id_cupom"] : "0"),
                    Codigo = row["codigo"],
                    Valor = Convert.ToDouble(row["valor"]),
                    Ativo = row["ativo"]
                };
                cupoms.Add(tempCupom);
            }

            return cupoms;
        }
    }
}