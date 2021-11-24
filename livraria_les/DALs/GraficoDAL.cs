using livraria_les.Aplicacao;
using livraria_les.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class GraficoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade) { return 0; }

        public override int Alterar(IEntidade entidade) { return 0; }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Grafico grafico = (Grafico)entidade;
            var pedidos = new List<IEntidade>();
            bool flgStatus = false, flgDataInicial = false, flgDataFinal = false;
            ClienteDAL cliDAL = new ClienteDAL();
            ItemPedidoDAL itemPedDAL = new ItemPedidoDAL();
            CategoriaDAL categoriaDAL = new CategoriaDAL();
            string strQuery = "";
            var parametros = new Dictionary<string, object>();
            var rows = new List<Dictionary<string, string>>();

            //strQuery = "select distinct * from pedido WHERE ";
            strQuery = "select distinct p.id_pedido, l.id_livro, lp.quantidade, cat.id_categoria, cli.id_cliente, p.dt_compra from pedido p " +
                "inner join cliente cli on cli.id_cliente = p.cliente_id " +
                "inner join livro_pedido lp on p.id_pedido = lp.pedido_id " +
                "inner join livro l on l.id_livro = lp.livro_id " +
                "inner join livro_categoria lc on lc.livro_id = l.id_livro " +
                "inner join categoria cat on lc.categoria_id = cat.id_categoria WHERE ";

            if (grafico.Categorias.Count > 0)
            {
                strQuery += "cat.id_categoria in(";
                foreach (var item in grafico.Categorias)
                    strQuery += item.Id + ", ";
                strQuery = strQuery.Substring(0, strQuery.Length - 2);
                strQuery += ") AND  ";
            }
            if (!string.IsNullOrEmpty(grafico.Status))
            {
                strQuery += "p.status = @status AND  ";
                flgStatus = true;
            }
            if (grafico.DataInicial.Year > 1900)
            {
                strQuery += "dt_compra >= @dataInicial AND  ";
                flgDataInicial = true;
            }
            if (grafico.DataFinal.Year > 1900)
            {
                strQuery += "dt_compra <= @dataFinal AND  ";
                flgDataFinal = true;
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            strQuery += " order by p.dt_compra, p.id_pedido, l.id_livro";

            parametros = new Dictionary<string, object>();

            if (flgStatus)
                parametros.Add("status", grafico.Status);
            if (flgDataInicial)
                parametros.Add("dataInicial", grafico.DataInicial.ToString("yyyy-MM-dd 00:00:00"));
            if (flgDataFinal)
                parametros.Add("dataFinal", grafico.DataFinal.ToString("yyyy-MM-dd 23:59:59"));

            rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempPedido = new Pedido
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_pedido"]) ? row["id_pedido"] : "0"),
                    Cliente = cliDAL.Consultar(new Cliente { Id = Convert.ToInt32(row["id_cliente"]) }).ConvertAll(x => (Cliente)x)[0],
                    DtCadastro = Convert.ToDateTime(row["dt_compra"])
                };
                var categorias = categoriaDAL.Consultar(new Categoria { Id = Convert.ToInt32(row["id_categoria"]) }).ConvertAll(x => (Categoria)x);
                Livro livro = new Livro { Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_livro"]) ? row["id_livro"] : "0"), Categorias = categorias };
                var itensPedido = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        PedidoId = Convert.ToInt32(!string.IsNullOrEmpty(row["id_pedido"]) ? row["id_pedido"] : "0"),
                        Livro = livro,
                        Quantidade = Convert.ToInt32(row["quantidade"])
                    }
                };
                tempPedido.ItensPedido = itensPedido;
                pedidos.Add(tempPedido);
            }
            return pedidos;
        }
    }
}