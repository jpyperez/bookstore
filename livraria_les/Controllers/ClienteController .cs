using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class ClienteController : Controller
    {
        private Resultado _resultado;

        // GET: Cliente
        public ActionResult Index()
        {
            return View(_ConsultarCliente(new Cliente()));
        }

        // POST: Cliente
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            var cliente = new Cliente
            {
                Nome = formCollection["Nome"],
                Genero = formCollection["Genero"],
                CPF = formCollection["CPF"],
                Ranking = formCollection["Ranking"],
                BuscaAtivo = formCollection["Ativo"]
            };

            return View(_ConsultarCliente(cliente));
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(FormCollection formCollection)
        {
            Cliente cliente = new Cliente();

            if (ModelState.IsValid)
            {
                cliente.Nome = formCollection["Nome"];
                cliente.Genero = formCollection["Genero"];
                if(!string.IsNullOrEmpty(formCollection["DtNascimento"]))
                    cliente.DtNascimento = Convert.ToDateTime(formCollection["DtNascimento"]);
                cliente.CPF = formCollection["CPF"];
                cliente.Usuario.Tipo = "1";

                _resultado = new Fachada().Salvar(cliente);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(cliente);
                }
                ViewBag.Message = "Registro Salvo com sucesso";
                Session.Remove("userEmail");
                Session.Remove("userSenha");
            }

            return View(new Cliente());
        }

        // Get Cliente/Detalhes/5
        public ActionResult Detalhes(int id)
        {
            Cliente cliente = new Cliente
            {
                Id = id
            };

            var clientes = _ConsultarCliente(cliente);

            if (clientes == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(clientes[0]);
        }

        // GET: Cliente/editar/5
        public ActionResult Editar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Cliente cliente = new Cliente
            {
                Id = id
            };

            var clientes = _ConsultarCliente(cliente);

            if (clientes == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(clientes[0]);
        }

        // POST: Cliente/editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormCollection formCollection, int id)
        {
            Cliente cliente = new Cliente
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                cliente.Nome = formCollection["Nome"];
                cliente.Genero = formCollection["Genero"];
                cliente.DtNascimento = DateTime.Parse(formCollection["DtNascimento"]);
                cliente.CPF = formCollection["CPF"];
                cliente.Usuario.Email = formCollection["Email"];
                cliente.Usuario.Senha = formCollection["Senha"];
                cliente.Usuario.Tipo = "1";

                Resultado resultado = new Fachada().Alterar(cliente);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(cliente);
                }
                ViewBag.Message = "Registro alterado com sucesso";
            }
            return View(new Cliente());
        }

        // GET: Livro/deletar/5
        public ActionResult Deletar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Cliente cliente = new Cliente
            {
                Id = id
            };

            var clientes = _ConsultarCliente(cliente);

            if (clientes == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(clientes[0]);
        }

        // POST: Cliente/deletar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar(FormCollection formCollection, int id)
        {
            Cliente cliente = new Cliente
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                Resultado resultado = new Fachada().Excluir(cliente);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(cliente);
                }
                ViewBag.Message = "Registro excluido com sucesso";
            }
            return View(new Cliente());
        }

        private List<Cliente> _ConsultarCliente(Cliente cliente)
        {
            _resultado = new Fachada().Consultar(cliente);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var clientes = new List<Cliente>();
            foreach (var item in _resultado.Entidades)
                clientes.Add((Cliente)item);

            return clientes;
        }
    }
}