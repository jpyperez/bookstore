using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class PedidoController : Controller
    {
        private Resultado _resultado;

        // GET: Itens
        public ActionResult Index()
        {
            return View(_ConsultarLivro(new Livro { BuscaAtivo = "1" }));
        }

        // POST: Pedido/Index/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection formCollection)
        {
            var itensPedido = new List<ItemPedido>();
            if (Session["Carrinho"] != null)
                itensPedido = (List<ItemPedido>)Session["Carrinho"];

            if (ModelState.IsValid)
            {
                var id = Convert.ToInt32(formCollection["id"]);
                var quantidade = Convert.ToInt32(formCollection["Quantidade"]);

                var estoque = new Estoque
                {
                    LivroId = id,
                    Quantidade = quantidade
                };

                _resultado = new Fachada().Consultar(estoque);

                if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
                {
                    ViewBag.Erro = _resultado.Message;
                    return View(_ConsultarLivro(new Livro { BuscaAtivo = "1" }));
                }

                bool encontrou = false;
                foreach (var item in itensPedido)
                {
                    if (item.Livro.Id == Convert.ToInt32(id))
                    {
                        item.Quantidade += Convert.ToInt32(quantidade);
                        encontrou = true;
                        break;
                    }
                }
                if (!encontrou)
                {
                    var itemPedido = new ItemPedido();
                    itemPedido.Livro.Id = Convert.ToInt32(id);
                    itemPedido.Quantidade = Convert.ToInt32(quantidade);

                    itensPedido.Add(itemPedido);
                }

                Session["Carrinho"] = itensPedido;
            }
            return RedirectToAction("Carrinho");
        }

        // GET: Carrinho
        public ActionResult Carrinho()
        {
            var itensPedido = new List<ItemPedido>();
            if (Session["Carrinho"] == null)
                Session["Carrinho"] = new List<ItemPedido>();
            else
                itensPedido = (List<ItemPedido>)Session["Carrinho"];

            foreach (var item in itensPedido)
                item.Livro = _ConsultarLivro(item.Livro)[0];

            return View(Session["Carrinho"]);
        }

        [HttpPost]
        public ActionResult Carrinho(FormCollection formCollection)
        {
            return RedirectToAction("Carrinho");
        }

        // GET: Livro/RemoverCarrinho/5
        public ActionResult RemoverCarrinho(int id)
        {
            var itensPedido = new List<ItemPedido>();
            if (Session["Carrinho"] != null)
                itensPedido = (List<ItemPedido>)Session["Carrinho"];

            var itemEncontrado = new ItemPedido();
            foreach (var item in itensPedido)
            {
                if (item.Livro.Id == id)
                {
                    itemEncontrado = item;
                    break;
                }
            }

            if (itemEncontrado != null)
            {
                itensPedido.Remove(itemEncontrado);
                Session["Carrinho"] = itensPedido;
                ViewBag.Message = "Registro removido com sucesso";
            }
            else
                ViewBag.Erro = "Falha ao remover o registro";

            return RedirectToAction("Carrinho");
        }

        // GET Checkout
        public ActionResult Checkout()
        {
            Cliente cliente = _ConsultarCliente(new Cliente { Id = Convert.ToInt32(Session["UserId"]) });
            Pedido pedido = new Pedido();
            var itensPedido = new List<ItemPedido>();

            // Setando o cliente ao pedido
            pedido.Cliente = cliente;

            if (_PreencherDDLEndereco() == 0 || _PreencherDDLCartao() == 0 || _PreencherDDLCupom() == 0)
            {
                ViewBag.Message = "Erro ao consultar DDL";
                return View(new Endereco());
            }

            // setando os itens do carrinho ao pedido
            if (Session["Carrinho"] != null)
                itensPedido = (List<ItemPedido>)Session["Carrinho"];
            pedido.ItensPedido = itensPedido;

            // verificando o total da compra
            pedido.Total = _TotalCompra(itensPedido);
            return View(pedido);
        }

        // POST Pedido/Checkout
        [HttpPost]
        public ActionResult Checkout(FormCollection formCollection)
        {
            var pedido = new Pedido();
            var mensagem = "";
            var enderecoId = string.IsNullOrEmpty(formCollection["Endereco"]) ? 0 : Convert.ToInt32(formCollection["Endereco"]);
            var cupomId = string.IsNullOrEmpty(formCollection["Cliente.Cupons"]) ? 0 : Convert.ToInt32(formCollection["Cliente.Cupons"]);
            var cartoesId = formCollection["CartaoDDL"] == null ? new string[] { } : formCollection["CartaoDDL"].Split(',');
            var valorPago = string.IsNullOrEmpty(formCollection["TotalPago"]) ? 0 : Convert.ToDouble(formCollection["TotalPago"]);

            // Validar se foi selecionado os itens
            if (enderecoId == 0)
                mensagem += "Endereco não selecionado//";
            if (!string.IsNullOrEmpty(mensagem))
            {
                if (_PreencherDDLEndereco() == 0 || _PreencherDDLCartao() == 0 || _PreencherDDLCupom() == 0)
                {
                    ViewBag.Erro = "Erro ao consultar DDL";
                    return View(pedido);
                }
                pedido.ItensPedido = (List<ItemPedido>)Session["Carrinho"];
                pedido.Total = _TotalCompra(pedido.ItensPedido);
                ViewBag.Erro = mensagem;
                return View(pedido);
            }

            foreach (var cartaoId in cartoesId)
                pedido.Cliente.Cartoes.Add(new Cartao { Id = Convert.ToInt32(cartaoId) });

            Cliente cliente = _ConsultarCliente(new Cliente { Id = Convert.ToInt32(Session["UserId"]) });
            var itensPedido = new List<ItemPedido>();

            // Setando o cliente ao pedido
            pedido.Cliente = cliente;

            // setando os itens do carrinho ao pedido
            if (Session["Carrinho"] != null)
                itensPedido = (List<ItemPedido>)Session["Carrinho"];

            foreach (var item in itensPedido)
            {
                var idLivro = item.Livro.Id;
                item.Livro = new Livro { Id = idLivro };
                _resultado = new Fachada().Consultar(item.Livro);

                if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
                {
                    ViewBag.Message = _resultado.Message;
                    return null;
                }

                var livro = new Livro();
                item.Livro = (Livro)_resultado.Entidades[0];
                item.Status = "Em processamento";
            }

            pedido.ItensPedido = itensPedido;

            pedido.Total = _TotalCompra(itensPedido);
            pedido.Status = "Em processamento";

            _resultado = new Fachada().Salvar(pedido);

            if (!string.IsNullOrEmpty(_resultado.Message))
            {
                _PreencherDDLEndereco();
                ViewBag.Erro = _resultado.Message;
                // apresenta erro
                return View(pedido);
            }
            ViewBag.Message = "Registro Salvo com sucesso";
            Session.Remove("Carrinho");
            return RedirectToAction("Index");
        }

        // GET com a lista de todos os pedidos
        public ActionResult Pedidos()
        {
            if (Session["UserType"].ToString() == "2")
            {
                return View(_ConsultarPedido(new Pedido()));
            }
            else if (Session["UserType"].ToString() == "1")
            {
                return View(_ConsultarPedido(new Pedido { Cliente = new Cliente { Id = Convert.ToInt32(Session["UserId"]) } }));
            }
            return RedirectToAction("Index");
        }

        // POST: Pedido/Pedidos
        [HttpPost]
        public ActionResult Pedidos(FormCollection formCollection)
        {
            var pedido = new Pedido
            {
                Cliente = new Cliente { Nome = formCollection["Nome"] },
                Status = formCollection["Status"],
                BuscaAtivo = formCollection["Ativo"]
            };

            pedido.Status = pedido.Status.Substring(1, pedido.Status.Length - 1);

            return View(_ConsultarPedido(pedido));
        }

        public ActionResult Detalhes(int id)
        {
            Pedido pedido = new Pedido
            {
                Id = id
            };

            var pedidos = _ConsultarPedido(pedido);

            if (pedidos == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            _PreencherDDLStatus(pedidos[0].Status);

            return View(pedidos[0]);
        }

        [HttpPost]
        public ActionResult Detalhes(FormCollection formCollection)
        {
            
            var pedido = new Pedido
            {
                Id = Convert.ToInt32(formCollection["Id"]),
                Cliente = new Cliente { Id = Convert.ToInt32(formCollection["Cliente.Id"]) }
            };

            var ped = _ConsultarPedido(pedido)[0];

            if (formCollection["Status"] == "")
                return View(ped);

            if (ped == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            ped.Status = formCollection["Status"];
            foreach (var item in ped.ItensPedido)
            {
                if(item.Status != "Entregue")
                    item.Status = ped.Status;
                if (ped.Status == "Entregue")
                {
                    item.Livro.Estoque -= item.Quantidade;
                    item.Livro.AlteraEstoque = true;
                }
                else if (ped.Status == "Trocado")
                {
                    item.Livro.Estoque += item.Quantidade;
                    item.Livro.AlteraEstoque = true;
                }
            }

            if(ped.Status == "Trocado")
                if (!_gerarCupom(ped))
                    return View(ped);
                    
            _resultado = new Fachada().Alterar(ped);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Erro = _resultado.Message;
                _PreencherDDLStatus(ped.Status);
                return View(pedido);
            }

            return View("Pedidos", _ConsultarPedido(new Pedido()));
        }

        public ActionResult TrocarItem(int idPedido, int idLivro)
        {
            var pedido = _ConsultarPedido(new Pedido { Id = idPedido })[0];
            pedido.ItensPedido = new List<ItemPedido>();
            pedido.Status = "Em troca";
            pedido.ItensPedido.Add(new ItemPedido
            {
                PedidoId = idPedido,
                Livro = new Livro { Id = idLivro },
                Status = "Em troca"
            });
            _resultado = new Fachada().Alterar(pedido);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Erro = _resultado.Message;
                return null;
            }

            return RedirectToAction("Pedidos");
        }

        private bool _gerarCupom(Pedido pedido)
        {
            var cupom = new Cupom
            {
                ClienteId = pedido.Cliente.Id
            };

            foreach (var item in pedido.ItensPedido)
                if(item.Status == "Trocado")
                    cupom.Valor += (item.Quantidade * item.Livro.ValorVenda);

            _resultado = new Fachada().Salvar(cupom);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Erro = _resultado.Message;
                return false;
            }

            return true;
        }

        private Cliente _ConsultarCliente(Cliente cliente)
        {
            _resultado = new Fachada().Consultar(cliente);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            Cliente cli = new Cliente();
            cli = (Cliente)_resultado.Entidades[0];

            return cli;
        }

        private List<Livro> _ConsultarLivro(Livro livro)
        {
            _resultado = new Fachada().Consultar(livro);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var livros = new List<Livro>();
            foreach (Livro item in _resultado.Entidades)
                livros.Add(item);

            return livros;
        }

        private List<Pedido> _ConsultarPedido(Pedido pedido)
        {
            _resultado = new Fachada().Consultar(pedido);

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

        private List<Estoque> _ConsultarEstoque(Estoque estoque)
        {
            _resultado = new Fachada().Consultar(estoque);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var estoques = new List<Estoque>();
            foreach (Estoque item in _resultado.Entidades)
                estoques.Add(item);

            return estoques;
        }

        private int _PreencherDDLEndereco()
        {
            ViewBag.EnderecosClienteDDL = "";
            _resultado = new Fachada().Consultar(new Endereco { ClienteId = Convert.ToInt32(Session["UserId"]) });

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var enderecos = new List<Endereco>();
            foreach (Endereco item in _resultado.Entidades)
                enderecos.Add(item);

            ViewBag.EnderecosClienteDDL = new SelectList(enderecos, "Id", "Alias");

            return 1;
        }

        private int _PreencherDDLCartao()
        {
            ViewBag.CartaoDDL = "";
            _resultado = new Fachada().Consultar(new Cartao { ClienteId = Convert.ToInt32(Session["UserId"]) });

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var cartoes = new List<Cartao>();
            foreach (Cartao card in _resultado.Entidades)
                cartoes.Add(card);

            ViewBag.CartaoDDL = cartoes;

            return 1;
        }

        private int _PreencherDDLCupom()
        {
            ViewBag.CuponsClienteDDL = "";
            _resultado = new Fachada().Consultar(new Cliente());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var cupons = new List<Cupom>();
            foreach (Cliente cliente in _resultado.Entidades)
                foreach (Cupom card in cliente.Cupons)
                    cupons.Add(card);

            ViewBag.CuponsClienteDDL = new SelectList(cupons, "Id", "Valor");

            return 1;
        }

        private int _PreencherDDLStatus(string status)
        {
            //var statusList = new SelectList(
            //    new List<SelectListItem>
            //    {
            //        new SelectListItem {Text = "Em processamento", Value = "1"},
            //        new SelectListItem {Text = "Aprovado", Value = "2"},
            //        new SelectListItem {Text = "Reprovado", Value = "3"},
            //        new SelectListItem {Text = "Em transporte", Value = "4"},
            //        new SelectListItem {Text = "Entregue", Value = "5"},
            //        new SelectListItem {Text = "Em troca", Value = "6"},
            //        new SelectListItem {Text = "Trocado", Value = "7"},
            //        new SelectListItem {Text = "Fora de mercado", Value = "8"}
            //    }, "Value", "Text");

            ViewBag.StatusDDL = new SelectList(new List<SelectList>());

            switch (status)
            {
                case "Em processamento":
                    ViewBag.StatusDDL = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem {Text = "Aprovado", Value = "Aprovado"},
                            new SelectListItem {Text = "Reprovado", Value = "Reprovado"}
                        }, "Value", "Text");
                    break;
                case "Aprovado":
                    ViewBag.StatusDDL = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem {Text = "Em transporte", Value = "Em transporte"}
                        }, "Value", "Text");
                    break;
                case "Em transporte":
                    ViewBag.StatusDDL = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem {Text = "Entregue", Value = "Entregue"}
                        }, "Value", "Text");
                    break;
                case "Em troca":
                    ViewBag.StatusDDL = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem {Text = "Trocado", Value = "Trocado"}
                        }, "Value", "Text");
                    break;
                case "Entregue":
                    ViewBag.StatusDDL = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem {Text = "Em troca", Value = "Em troca"}
                        }, "Value", "Text");
                    break;
                default:
                    return 0;
            }

            return 1;
        }

        private double _TotalCompra(List<ItemPedido> carrinho)
        {
            double soma = 0;
            foreach (var item in carrinho)
                soma += (item.Quantidade * item.Livro.ValorVenda);

            return soma;
        }
    }
}