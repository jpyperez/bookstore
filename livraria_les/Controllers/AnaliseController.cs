using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace livraria_les.Controllers
{
    public class AnaliseController : Controller
    {
        private Resultado _resultado;

        // GET: Analise
        public ActionResult Index()
        {
            //[
            //    ['Dia', 'Harry Potter', 'Jogos Vorazes', 'Madagascar', 'Papua New Guinea', 'Rwanda', 'Average', 'IDK'],
            //    ['2004/05', 165, 938, 522, 998, 450, 614.6, 100],
            //    ['2005/06', 135, 1120, 599, 1268, 288, 682, 100],
            //    ['2006/07', 157, 1167, 587, 807, 397, 623, 100],
            //    ['2007/08', 139, 1110, 615, 968, 215, 609.4, 100],
            //    ['2008/09', 136, 691, 629, 1026, 366, 569.6, 100],
            //]

            if (_PreencherDDLCategoria() == 0)
            {
                ViewBag.Erro = "Erro ao consultar DDL";
                return null;
            }

            GetChartData(new Grafico());
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            var grafico = new Grafico
            {
                DataInicial = DateTime.TryParse(formCollection["DataInicial"], out DateTime dateOut) ? Convert.ToDateTime(formCollection["DataInicial"]) : dateOut,
                DataFinal = DateTime.TryParse(formCollection["DataFinal"], out dateOut) ? Convert.ToDateTime(formCollection["DataFinal"]) : dateOut
            };

            var categoriasId = formCollection["CategoriasDDL"] == null ? new string[] { } : formCollection["CategoriasDDL"].Split(',');
            foreach (var categoriaId in categoriasId)
                grafico.Categorias.Add(new Categoria { Id = Convert.ToInt32(categoriaId) });

            if (_PreencherDDLCategoria() == 0)
            {
                ViewBag.Erro = "Erro ao consultar DDL";
                return null;
            }

            var dataJson = GetChartData(grafico);

            return View();
        }

        public JsonResult Data()
        {
            return Json(GetChartData(new Grafico()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChartData(Grafico entidade)
        {
            //entidade.Status = "Entregue";
            var pedidos = _ConsultarGrafico(entidade);
            bool flgEncontrou = false;
            int somaHomens = 0, somaMulheres = 0, pedidoId = 0, livroId = 0;

            List<object> chartTitle = new List<object>();
            List<List<object>> chartData = new List<List<object>>();

            chartTitle.Add("Data");
            foreach (var pedido in pedidos)
            {
                foreach (var item in pedido.ItensPedido)
                    foreach (var categoria in item.Livro.Categorias)
                    {
                        foreach (var titulo in chartTitle)
                            if (titulo.ToString() == categoria.Nome)
                                flgEncontrou = true;
                        if (!flgEncontrou)
                            chartTitle.Add(categoria.Nome);
                        flgEncontrou = false;
                    }
            }
            chartTitle.Add("Homens");
            chartTitle.Add("Mulheres");

            //chartData.Add(new List<string> { "0" });
            foreach (var pedido in pedidos)
            {
                if (chartData.Count == 0)
                    chartData.Add(new List<object> { pedido.DtCadastro.ToShortDateString().ToString() }); // Add nova data
                if (chartData[chartData.Count - 1][0].ToString() != pedido.DtCadastro.ToShortDateString().ToString())
                {
                    chartData.Add(new List<object> { pedido.DtCadastro.ToShortDateString().ToString() }); // Add nova data
                    somaHomens = somaMulheres = 0;
                }
                foreach (var item in pedido.ItensPedido) // passar por todos os itens
                {
                    for (int j = 1; j < chartTitle.Count; j++) // passar por todos os titulos
                    {
                        if (chartData[chartData.Count - 1].Count == j) // posição não tem algo?
                            chartData[chartData.Count - 1].Add(0);    // adiciona algo
                        foreach (var categoria in item.Livro.Categorias)
                        {
                            if (chartTitle[j].ToString() == categoria.Nome)         // titulo encontrado?
                            {
                                chartData[chartData.Count - 1][j] = Convert.ToUInt32(chartData[chartData.Count - 1][j]) + item.Quantidade;
                                if(pedidoId != item.PedidoId || livroId != item.Livro.Id)
                                {
                                    if (pedido.Cliente.Genero == "Masculino")
                                        somaHomens += item.Quantidade;
                                    else
                                        somaMulheres += item.Quantidade;
                                    // seta o atual id pedido e livro
                                    pedidoId = item.PedidoId;
                                    livroId = item.Livro.Id;
                                }

                            }
                        }
                    }
                    chartData[chartData.Count - 1][chartTitle.Count - 2] = somaHomens;
                    chartData[chartData.Count - 1][chartTitle.Count - 1] = somaMulheres;
                }
                //chartData.Add(pedido.DtCadastro.ToShortDateString());
            }

            chartData.Insert(0, chartTitle);

            var chartJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData);

            ViewBag.Data = chartJson;
            ViewBag.DataLength = chartTitle.Count;

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        private List<Pedido> _ConsultarGrafico(Grafico grafico)
        {
            _resultado = new Fachada().Consultar(grafico);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var pedidos = new List<Pedido>();
            foreach (Pedido item in _resultado.Entidades)
                pedidos.Add(item);

            return pedidos;
        }

        private int _PreencherDDLCategoria()
        {
            ViewBag.CategoriasDDL = "";
            _resultado = new Fachada().Consultar(new Categoria());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var categorias = new List<Categoria>();
            foreach (Categoria item in _resultado.Entidades)
                categorias.Add(item);

            ViewBag.CategoriasDDL = categorias;

            return 1;
        }
    }
}