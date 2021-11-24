using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class UsuarioController : Controller
    {
        private Resultado _resultado;

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Usuario/Registrar
        [AllowAnonymous]
        public ActionResult Registrar()
        {
            return View();
        }

        // POST: /Usuario/Registrar
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registrar(FormCollection formCollection)
        {
            Cliente cliente = new Cliente();
            var usuario = new Usuario
            {
                Email = formCollection["Usuario.Email"],
                Senha = formCollection["Usuario.Senha"],
                ConfirmSenha = formCollection["Usuario.ConfirmSenha"]
            };

            var users = _ConsultarUsuario(usuario);

            if (ModelState.IsValid)
            {
                cliente.Nome = formCollection["Nome"];
                cliente.Genero = formCollection["Genero"];
                cliente.DtNascimento = Convert.ToDateTime(formCollection["DtNascimento"]);
                cliente.CPF = formCollection["CPF"];
                cliente.Usuario = usuario;
                cliente.Usuario.Tipo = "1";
                cliente.Telefone.Tipo = formCollection["Telefone.Tipo"];
                cliente.Telefone.DDD = formCollection["Telefone.DDD"];
                cliente.Telefone.Numero = formCollection["Telefone.Numero"];

                foreach (var user in users)
                    if (user.Email == usuario.Email)
                    {
                        ViewBag.Erro = "Email já cadastrado";
                        return View(new Cliente { Usuario = usuario });
                    }

                if (usuario.Senha != usuario.ConfirmSenha)
                {
                    ViewBag.Erro = "Novas senhas não correspondem";
                    return View();
                }

                _resultado = new Fachada().Salvar(cliente);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(cliente);
                }
                ViewBag.Message = "Registro Salvo com sucesso";
            }

            return RedirectToAction("Index", "Pedido");
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: /Usuario/Registrar
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(FormCollection formCollection)
        {
            var usuario = new Usuario
            {
                Email = formCollection["Usuario.Email"],
                Senha = formCollection["Usuario.Senha"]
            };

            var users = _ConsultarUsuario(usuario);

            foreach (var user in users)
            {
                if (user.Email == usuario.Email && user.Senha == usuario.Senha)
                {
                    if (user.PessoaId > 0 && user.Tipo == "1")
                    {
                        var cliente = _ConsultarCliente(new Cliente { Id = user.PessoaId })[0];
                        Session["UserType"] = user.Tipo;
                        Session["UserName"] = cliente.Nome;
                        Session["UserId"] = cliente.Id;
                    }
                    else
                    {
                        var funcionario = _ConsultarFuncionario(new Funcionario { Id = user.PessoaId })[0];
                        Session["UserType"] = user.Tipo;
                        Session["UserName"] = funcionario.Nome;
                        Session["UserId"] = funcionario.Id;
                    }
                    return RedirectToAction("Index", "Pedido");
                }
            }

            ViewBag.Erro = "Usuário ou senha incorretos";
            return View(new Cliente { Usuario = usuario });
        }

        // GET: /Usuario/AlterarSenha
        public ActionResult AlterarSenha()
        {
            return View();
        }

        // POST: /Usuario/AlterarSenha
        [HttpPost]
        public ActionResult AlterarSenha(FormCollection formCollection)
        {
            var usuario = new Usuario
            {
                SenhaAtual = formCollection["Usuario.SenhaAtual"],
                Senha = formCollection["Usuario.Senha"],
                ConfirmSenha = formCollection["Usuario.ConfirmSenha"]
            };

            var user = _ConsultarUsuario(new Usuario { Id = Convert.ToInt32(Session["UserId"]) })[0];

            if (usuario.SenhaAtual != user.Senha)
            {
                ViewBag.Erro = "Senha atual não corresponde";
                return View();
            }
            if (usuario.Senha != usuario.ConfirmSenha)
            {
                ViewBag.Erro = "Novas senhas não correspondem";
                return View();
            }

            user.Senha = usuario.Senha;

            _resultado = new Fachada().Alterar(user);

            if (!string.IsNullOrEmpty(_resultado.Message))
            {
                ViewBag.Erro = _resultado.Message;
                // apresenta erro
                return View();
            }
            ViewBag.Message = "Registro Alterado com sucesso";

            return RedirectToAction("Index", "Pedido");
        }

        // POST: /Usuario/LogOff
        public ActionResult LogOff()
        {
            Session["UserType"] = null;
            Session["UserName"] = null;
            Session["UserId"] = null;
            Session["Carrinho"] = null;
            Session.Remove("UserType");
            Session.Remove("UserName");
            Session.Remove("UserId");
            Session.Remove("Carrinho");
            return RedirectToAction("Index", "Pedido");
        }

        private List<Usuario> _ConsultarUsuario(Usuario usuario)
        {
            _resultado = new Fachada().Consultar(usuario);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var usuarios = new List<Usuario>();
            foreach (var user in _resultado.Entidades)
            {
                usuarios.Add((Usuario)user);
            }
            return usuarios;
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

        private List<Funcionario> _ConsultarFuncionario(Funcionario funcionario)
        {
            _resultado = new Fachada().Consultar(funcionario);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var funcionarios = new List<Funcionario>();
            foreach (var item in _resultado.Entidades)
                funcionarios.Add((Funcionario)item);

            return funcionarios;
        }
    }
}