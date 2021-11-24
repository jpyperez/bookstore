using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class CategoriaController : Controller
    {
        private Resultado _resultado;

        // GET: Categoria
        public ActionResult Index()
        {
            return View(_ConsultarCategoria(new Categoria()));
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(FormCollection formCollection)
        {
            Categoria categoria = new Categoria();

            if (ModelState.IsValid)
            {
                categoria.Nome = formCollection["Nome"];

                _resultado = new Fachada().Salvar(categoria);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(categoria);
                }
                ViewBag.Message = "Registro Salvo com sucesso";
            }


            return View(new Categoria());
        }

        public ActionResult Detalhes(int id)
        {
            FormCollection formcollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Categoria categorias = new Categoria
            {
                Id = id
            };

            _resultado = new Fachada().Consultar(new Categoria());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            // ProcurarInformacoesCategoria(result, fachada, categorias);

            return View(categorias);
        }

        // GET: Categoria/editar/5
        public ActionResult Editar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Categoria categoria = new Categoria
            {
                Id = id
            };

            var categorias = _ConsultarCategoria(categoria);

            if (categorias == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(categorias[0]);
        }

        // POST: Categoria/editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormCollection formCollection, int id)
        {
            Categoria categoria = new Categoria
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                categoria.Nome = formCollection["Nome"];

                Resultado resultado = new Fachada().Alterar(categoria);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(categoria);
                }
                ViewBag.Message = "Registro alterado com sucesso";
            }
            return View(new Categoria());
        }

        // GET: Livro/deletar/5
        public ActionResult Deletar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Categoria categoria = new Categoria
            {
                Id = id
            };

            var categorias = _ConsultarCategoria(categoria);

            if (categorias == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(categorias[0]);
        }

        // POST: Categoria/deletar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar(FormCollection formCollection, int id)
        {
            Categoria categoria = new Categoria
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                Resultado resultado = new Fachada().Excluir(categoria);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(categoria);
                }
                ViewBag.Message = "Registro excluido com sucesso";
            }
            return View(new Categoria());
        }

        private List<Categoria> _ConsultarCategoria(Categoria categoria)
        {
            _resultado = new Fachada().Consultar(categoria);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var categorias = new List<Categoria>();
            foreach (var item in _resultado.Entidades)
                categorias.Add((Categoria)item);

            return categorias;
        }
    }
}