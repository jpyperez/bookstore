using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class EnderecoController : Controller
    {
        private Resultado _resultado;

        // GET: Endereco
        public ActionResult Index()
        {
            return View(_ConsultarEndereco(new Endereco()));
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(FormCollection formCollection)
        {
            Endereco endereco = new Endereco();

            if (ModelState.IsValid)
            {
                endereco.Alias = formCollection["Alias"];
                endereco.TipoResidencia.Id = formCollection["TipoResidencia"] == "" ? 0 : Convert.ToInt32(formCollection["TipoResidencia"]);
                endereco.TipoLogradouro = formCollection["TipoLogradouro"];
                endereco.Logradouro = formCollection["Logradouro"];
                endereco.Numero = formCollection["Numero"];
                endereco.Bairro = formCollection["Bairro"];
                endereco.CEP = formCollection["CEP"];
                endereco.Cidade = formCollection["Cidade"];
                endereco.Estado = formCollection["Estado"];
                endereco.Pais = formCollection["Pais"];
                endereco.Observacoes = formCollection["Observacoes"];
                endereco.Preferencial = formCollection["Preferencial"] == null ? 0 : 1;
                endereco.ClienteId = Convert.ToInt32(Session["UserId"]);

                _resultado = new Fachada().Salvar(endereco);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(endereco);
                }
                ViewBag.Message = "Registro Salvo com sucesso";
            }

            return View(new Endereco());
        }

        public ActionResult Detalhes(int id)
        {
            FormCollection formcollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Endereco enderecos = new Endereco
            {
                Id = id
            };

            _resultado = new Fachada().Consultar(new Cliente());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            // ProcurarInformacoesEndereco(result, fachada, enderecos);

            return View(enderecos);
        }

        // GET: Endereco/editar/5
        public ActionResult Editar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Endereco endereco = new Endereco
            {
                Id = id
            };

            var enderecos = _ConsultarEndereco(endereco);

            if (enderecos == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(enderecos[0]);
        }

        // POST: Endereco/editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormCollection formCollection, int id)
        {
            Endereco endereco = new Endereco
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                endereco.Alias = formCollection["Alias"];
                endereco.TipoResidencia.Id = formCollection["TipoResidencia"] == "" ? 0 : Convert.ToInt32(formCollection["TipoResidencia"]);
                endereco.TipoLogradouro = formCollection["TipoLogradouro"];
                endereco.Logradouro = formCollection["Logradouro"];
                endereco.Numero = formCollection["Numero"];
                endereco.Bairro = formCollection["Bairro"];
                endereco.CEP = formCollection["CEP"];
                endereco.Cidade = formCollection["Cidade"];
                endereco.Estado = formCollection["Estado"];
                endereco.Pais = formCollection["Pais"];
                endereco.Observacoes = formCollection["Observacoes"];
                endereco.Preferencial = formCollection["Preferencial"] == null ? 0 : 1;

                Resultado resultado = new Fachada().Alterar(endereco);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(endereco);
                }
                ViewBag.Message = "Registro alterado com sucesso";
            }
            return View(new Endereco());
        }

        // GET: Livro/deletar/5
        public ActionResult Deletar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Endereco endereco = new Endereco
            {
                Id = id
            };

            var enderecos = _ConsultarEndereco(endereco);

            if (enderecos == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(enderecos[0]);
        }

        // POST: Endereco/deletar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar(FormCollection formCollection, int id)
        {
            Endereco endereco = new Endereco
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                Resultado resultado = new Fachada().Excluir(endereco);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(endereco);
                }
                ViewBag.Message = "Registro excluido com sucesso";
            }
            return View(new Endereco());
        }

        private List<Endereco> _ConsultarEndereco(Endereco endereco)
        {
            endereco.ClienteId = Convert.ToInt32(Session["UserId"]);
            _resultado = new Fachada().Consultar(endereco);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var enderecos = new List<Endereco>();
            foreach (var item in _resultado.Entidades)
                enderecos.Add((Endereco)item);

            return enderecos;
        }
    }
}