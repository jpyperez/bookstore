using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class CartaoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Cartao cartao = (Cartao)entidade;

            string commandText = "INSERT INTO cartao (numero, nome_impresso, bandeira, codigo_seguranca, preferencial, dt_cadastro, dt_validade)" +
                " VALUES (@numero, @nome_impresso, @bandeira, @codigo_seguranca, @preferencial, @dt_cadastro, @dt_validade)";

            var parametros = new Dictionary<string, object>
            {
                { "numero", cartao.Numero },
                { "nome_impresso", cartao.NomeImpresso },
                { "bandeira", cartao.Bandeira },
                { "codigo_seguranca", cartao.CodSeguranca },
                { "preferencial", cartao.Preferencial },
                { "dt_cadastro", DateTime.Now },
                { "dt_validade", cartao.DtVencimento }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;

            cartao.Id = conexao.LastInsertedId();

            commandText = "INSERT INTO cliente_cartao (cliente_id, cartao_id)" +
                    " VALUES (@cliente_id, @cartao_id)";

            parametros = new Dictionary<string, object>
            {
                { "cliente_id", cartao.ClienteId },
                { "cartao_id", cartao.Id }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Cartao cartao = (Cartao)entidade;

            string commandText = "UPDATE cartao SET numero = @numero, nome_impresso = @nome_impresso, bandeira = @bandeira, " +
                "codigo_seguranca = @codigo_seguranca, preferencial = @preferencial where id_cartao = @id)";

            var parametros = new Dictionary<string, object>
            {
                { "numero", cartao.Numero },
                { "nome_impresso", cartao.NomeImpresso },
                { "bandeira", cartao.Bandeira },
                { "codigo_seguranca", cartao.CodSeguranca },
                { "preferencial", cartao.Preferencial },
                { "id", cartao.Id }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Cartao cartao = (Cartao)entidade;
            var cartoes = new List<IEntidade>();
            bool flgId = false, flgClienteId = false;

            string strQuery = "SELECT * FROM cartao ";

            if (cartao.Id > 0)
            {
                strQuery += "WHERE id_cartao = @id_cartao";
                flgId = true;
            }
            else if (cartao.ClienteId > 0)
            {
                strQuery += "INNER JOIN cliente_cartao ON id_cartao = cartao_id where cliente_id = @cliente_id";
                flgClienteId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id_cartao", cartao.Id);
            if (flgClienteId)
                parametros.Add("cliente_id", cartao.ClienteId);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCartao = new Cartao
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_cartao"]) ? row["id_cartao"] : "0"),
                    Numero = row["numero"],
                    NomeImpresso = row["nome_impresso"],
                    Bandeira = row["bandeira"],
                    CodSeguranca = Convert.ToInt32(row["codigo_seguranca"]),
                    DtVencimento = Convert.ToDateTime(row["dt_validade"]),
                    Preferencial = Convert.ToInt32(row["preferencial"])
                };
                cartoes.Add(tempCartao);
            }
            return cartoes;
        }
    }
}