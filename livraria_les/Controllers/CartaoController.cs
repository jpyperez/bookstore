using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class CartaoController : Controller
    {
        private Resultado _resultado;

        // GET: Cartao
        public ActionResult Index()
        {
            return View(_ConsultarCartao(new Cartao()));
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(FormCollection formCollection)
        {
            Cartao cartao = new Cartao();

            if (ModelState.IsValid)
            {
                cartao.Numero = formCollection["Numero"];
                cartao.NomeImpresso = formCollection["NomeImpresso"];
                cartao.Bandeira = formCollection["Bandeira"];
                cartao.CodSeguranca = formCollection["CodSeguranca"] == "" ? 0 : int.Parse(formCollection["CodSeguranca"]);
                cartao.Preferencial = formCollection["Preferencial"] == null ? 0 : 1;
                cartao.DtVencimento = formCollection["DtVencimento"] == "" ? DateTime.Parse("01/01/1000") : DateTime.Parse(formCollection["DtVencimento"]);
                cartao.ClienteId = Convert.ToInt32(Session["UserId"]);

                _resultado = new Fachada().Salvar(cartao);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(cartao);
                }
                ViewBag.Message = "Registro Salvo com sucesso";
            }

            return View(new Cartao());
        }

        public ActionResult Detalhes(int id)
        {
            FormCollection formcollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Cartao cartaos = new Cartao
            {
                Id = id
            };

            _resultado = new Fachada().Consultar(new Cliente());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            // ProcurarInformacoesCartao(result, fachada, cartaos);

            return View(cartaos);
        }

        // GET: Cartao/editar/5
        public ActionResult Editar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Cartao cartao = new Cartao
            {
                Id = id
            };

            var cartaos = _ConsultarCartao(cartao);

            if (cartaos == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(cartaos[0]);
        }

        // POST: Cartao/editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormCollection formCollection, int id)
        {
            Cartao cartao = new Cartao
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                cartao.Numero = formCollection["Numero"];
                cartao.NomeImpresso = formCollection["NomeImpresso"];
                cartao.Bandeira = formCollection["Bandeira"];
                cartao.CodSeguranca = int.Parse(formCollection["CodSeguranca"]);
                cartao.Numero = formCollection["Numero"];
                cartao.DtVencimento = DateTime.Parse(formCollection["DtVencimento"]);

                Resultado resultado = new Fachada().Alterar(cartao);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(cartao);
                }
                ViewBag.Message = "Registro alterado com sucesso";
            }
            return View(new Cartao());
        }

        // GET: Livro/deletar/5
        public ActionResult Deletar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Cartao cartao = new Cartao
            {
                Id = id
            };

            var cartaos = _ConsultarCartao(cartao);

            if (cartaos == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(cartaos[0]);
        }

        // POST: Cartao/deletar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar(FormCollection formCollection, int id)
        {
            Cartao cartao = new Cartao
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                Resultado resultado = new Fachada().Excluir(cartao);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(cartao);
                }
                ViewBag.Message = "Registro excluido com sucesso";
            }
            return View(new Cartao());
        }

        private List<Cartao> _ConsultarCartao(Cartao cartao)
        {
            Cliente cliente = new Cliente { Id = Convert.ToInt32(Session["UserId"]) };
            cliente.Cartoes.Add(cartao);
            _resultado = new Fachada().Consultar(cliente);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var cartaos = new List<Cartao>();
            foreach (Cliente cli in _resultado.Entidades)
                foreach(var card in cli.Cartoes)
                    cartaos.Add(card);

            return cartaos;
        }
    }
}