using livraria_les.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class PedidoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            ItemPedidoDAL itemPedDAL = new ItemPedidoDAL();
            EstoqueDAL estDAL = new EstoqueDAL();
            Pedido pedido = (Pedido)entidade;

            string commandText = "INSERT INTO pedido (cliente_id, valor, /*json_endereco, json_pagamento, */status, dt_compra)" +
                " VALUES (@cliente_id, @valor,/* @json_endereco, @json_pagamento,*/ @status, @dt_compra)";

            var parametros = new Dictionary<string, object>
            {
                { "cliente_id", pedido.Cliente.Id },
                { "valor", pedido.Total },
                //{ "json_endereco", pedido.Total }, // Verificar como fazer a conversão para json aqui
                //{ "json_pagamento", pedido.Total }, // Verificar como fazer a conversão para json aqui
                { "status", pedido.Status },
                { "dt_compra", DateTime.Now }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;

            pedido.Id = conexao.LastInsertedId();

            // Adicionando os itens do pedido
            foreach (var item in pedido.ItensPedido)
            {
                item.PedidoId = pedido.Id;
                if (itemPedDAL.Salvar(item) == 0) // Deu erro?
                    return 0;
            }

            return 1;
        }

        public override int Alterar(IEntidade entidade)
        {
            ItemPedidoDAL itemPedDAL = new ItemPedidoDAL();
            EstoqueDAL estDAL = new EstoqueDAL();
            Pedido pedido = (Pedido)entidade;

            var commandText = "UPDATE pedido SET status = @status where id_pedido = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", pedido.Id},
                { "status", pedido.Status }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;
            
            foreach (var item in pedido.ItensPedido)
            {
                if (itemPedDAL.Alterar(item) == 0)
                    return 0;

                if (item.Status == "Entregue" || item.Status == "Trocado")
                {
                    if (estDAL.Alterar(item.Livro) == 0) // Deu erro?
                        return 0;
                }
                 
            }
            return 1;
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Pedido pedido = (Pedido)entidade;
            var pedidos = new List<IEntidade>();
            bool flgId = false, flgIdCliente = false, flgStatus = false;
            ClienteDAL cliDAL = new ClienteDAL();
            ItemPedidoDAL itemPedDAL = new ItemPedidoDAL();
            string strQuery = "";
            var parametros = new Dictionary<string, object>();
            var rows = new List<Dictionary<string, string>>();

            strQuery = "SELECT * FROM pedido WHERE ";
            if (pedido.Id > 0)
            {
                strQuery += "id_pedido = @id AND  ";
                flgId = true;
            }
            if (pedido.Cliente.Id > 0)
            {
                strQuery += "cliente_id = @cliente_id AND  ";
                flgIdCliente = true;
            }
            if (!string.IsNullOrEmpty(pedido.Status))
            {
                strQuery += "status = @status AND  ";
                flgStatus = true;
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            strQuery += " order by dt_compra";

            parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", pedido.Id);
            if (flgIdCliente)
                parametros.Add("cliente_id", pedido.Cliente.Id);
            if (flgStatus)
                parametros.Add("status", pedido.Status);

            rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempPedido = new Pedido
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_pedido"]) ? row["id_pedido"] : "0"),
                    Total = Convert.ToDouble(row["valor"]),
                    Status = row["status"],
                    //Endereco = JsonConvert.DeserializeObject<Endereco>(row["json_endereco"]),
                    //FormasPagamento = JsonConvert.DeserializeObject<List<FormaPagamento>>(row["json_pagamento"]),
                    ItensPedido = itemPedDAL.Consultar(new ItemPedido { PedidoId = Convert.ToInt32(row["id_pedido"]) }).ConvertAll(x => (ItemPedido)x),
                    Cliente = cliDAL.Consultar(new Cliente { Id = Convert.ToInt32(row["cliente_id"]) }).ConvertAll(x => (Cliente)x)[0],
                    DtCadastro = Convert.ToDateTime(row["dt_compra"])
                };
                pedidos.Add(tempPedido);
            }

            return pedidos;
        }
    }
}