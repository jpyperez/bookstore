using livraria_les.Aplicacao;
using livraria_les.Facade;
using livraria_les.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace livraria_les.Controllers
{
    public class LivroController : Controller
    {
        private Resultado _resultado;

        // GET: Livro
        public ActionResult Index()
        {
            return View(_ConsultarLivro(new Livro()));
        }

        // POST: Livro
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            var livro = new Livro
            {
                Titulo = formCollection["Titulo"],
                Ano = formCollection["Ano"],
                Edicao = formCollection["Edicao"],
                ISBN = formCollection["ISBN"],
                BuscaAtivo = formCollection["Ativo"]
            };

            return View(_ConsultarLivro(livro));
        }

        // GET: Livro/Cadastrar
        public ActionResult Cadastrar()
        {
            if (_PreencherDDLCategoria() == 0 || _PreencherDDLSubcategoria(0) == 0 || _PreencherDDLEditora(0) == 0 ||
                _PreencherDDLAutor(0) == 0 || _PreencherDDLGrupoPrecificacao(0) == 0)
            {
                ViewBag.Erro = "Erro ao consultar DDL";
                return null;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(FormCollection formCollection)
        {
            Livro livro = new Livro();
            bool flgErro = false;

            if (ModelState.IsValid)
            {
                livro.Titulo = formCollection["Titulo"];
                livro.Ano = formCollection["Ano"];
                livro.Edicao = formCollection["Edicao"];
                livro.ISBN = formCollection["ISBN"];
                livro.CodBarras = formCollection["CodBarras"];
                livro.NumPag = formCollection["NumPag"];
                livro.Sinopse = formCollection["Sinopse"];
                livro.Dimensoes = formCollection["Dimensoes"];
                livro.Peso = string.IsNullOrEmpty(formCollection["Peso"]) ? 0 : Convert.ToDouble(formCollection["Peso"]);
                livro.ValorCompra = string.IsNullOrEmpty(formCollection["ValorCompra"]) ? 0 : Convert.ToDouble(formCollection["ValorCompra"]);
                livro.Subcategoria.Id = string.IsNullOrEmpty(formCollection["SubCategoriaDDL"]) ? 0 : Convert.ToInt32(formCollection["SubCategoriaDDL"]);
                livro.Autor.Id = string.IsNullOrEmpty(formCollection["AutorDDL"]) ? 0 : Convert.ToInt32(formCollection["AutorDDL"]);
                livro.Editora.Id = string.IsNullOrEmpty(formCollection["EditoraDDL"]) ? 0 : Convert.ToInt32(formCollection["EditoraDDL"]);
                livro.GrupoPrecificacao.Id = string.IsNullOrEmpty(formCollection["GrupoPrecificacaoDDL"]) ? 0 : Convert.ToInt32(formCollection["GrupoPrecificacaoDDL"]);
                livro.Estoque = string.IsNullOrEmpty(formCollection["Estoque"]) ? 0 : Convert.ToInt32(formCollection["Estoque"]);
                var categoriasId = formCollection["CategoriasDDL"] == null ? new string[] { } : formCollection["CategoriasDDL"].Split(',');
                foreach (var categoriaId in categoriasId)
                    livro.Categorias.Add(new Categoria { Id = Convert.ToInt32(categoriaId) });

                _resultado = new Fachada().Salvar(livro);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    flgErro = true;
                }

                if (_PreencherDDLCategoria() == 0 || _PreencherDDLSubcategoria(livro.Subcategoria.Id) == 0 || _PreencherDDLEditora(livro.Editora.Id) == 0 ||
                _PreencherDDLAutor(livro.Autor.Id) == 0 || _PreencherDDLGrupoPrecificacao(livro.GrupoPrecificacao.Id) == 0)
                {
                    ViewBag.Message = "Erro ao consultar DDL";
                    return View(new Livro());
                }

                if(flgErro)
                    return View(livro);


                ViewBag.Message = "Registro Salvo com sucesso";
            }

            return View(new Livro());
        }

        // Get Livro/AtivarLivro/5
        public ActionResult AtivarLivro(int id)
        {
            Livro livro = new Livro
            {
                Id = id
            };

            var livros = _ConsultarLivro(livro);

            if (livros == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            _PreencherDDLCategoriaAtivacao(0);

            return View(livros[0]);
        }

        // Get Livro/AtivarLivro/5
        [HttpPost]
        public ActionResult AtivarLivro(FormCollection formCollection)
        {
            var livro = new Livro
            {
                Id = Convert.ToInt32(formCollection["Id"])
            };

            livro = _ConsultarLivro(livro)[0];
            livro.Ativo = "1";
            livro.CategoriaAtivacao.Id = Convert.ToInt32(formCollection["CategoriaAtivacaoDDL"]);
            livro.Justificativa = formCollection["Justificativa"];

            if (livro == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            _resultado = new Fachada().Alterar(livro);

            if (!string.IsNullOrEmpty(_resultado.Message))
            {
                _PreencherDDLCategoriaAtivacao(0);
                ViewBag.Erro = _resultado.Message;
                // apresenta erro
                return View(livro);
            }
            ViewBag.Message = "Registro Salvo com sucesso";

            return RedirectToAction("Index", "Livro");
        }

        // Get Livro/Detalhes/5
        public ActionResult Detalhes(int id)
        {
            Livro livro = new Livro
            {
                Id = id
            };

            var livros = _ConsultarLivro(livro);

            if (livros == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(livros[0]);
        }

        // GET: Livro/editar/5
        public ActionResult Editar(int id)
        {
            Livro livro = new Livro
            {
                Id = id
            };

            var livros = _ConsultarLivro(livro)[0];
            livros.ValorNovaVenda = livros.ValorVenda;

            if (livros == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            if (_PreencherDDLCategoria() == 0 || _PreencherDDLSubcategoria(livros.Subcategoria.Id) == 0 || _PreencherDDLEditora(livros.Editora.Id) == 0 ||
                _PreencherDDLAutor(livros.Autor.Id) == 0 || _PreencherDDLGrupoPrecificacao(livros.GrupoPrecificacao.Id) == 0)
            {
                ViewBag.Message = "Erro ao consultar DDL";
                return View(new Livro());
            }

            return View(livros);
        }

        // POST: Livro/editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(FormCollection formCollection, int id)
        {

            Livro livro = new Livro
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                livro.Titulo = formCollection["Titulo"];
                livro.Ano = formCollection["Ano"];
                livro.Edicao = formCollection["Edicao"];
                livro.ISBN = formCollection["ISBN"];
                livro.CodBarras = formCollection["CodBarras"];
                livro.NumPag = formCollection["NumPag"];
                livro.Sinopse = formCollection["Sinopse"];
                livro.Dimensoes = formCollection["Dimensoes"];
                livro.Ativo = formCollection["Ativo"];
                livro.Peso = Convert.ToDouble(formCollection["Peso"]);
                livro.ValorNovaVenda = Convert.ToDouble(formCollection["ValorNovaVenda"]);
                livro.ValorVenda = Convert.ToDouble(formCollection["ValorVenda"]);
                livro.Subcategoria.Id = Convert.ToInt32(formCollection["SubCategoriaDDL"]);
                livro.Autor.Id = Convert.ToInt32(formCollection["AutorDDL"]);
                livro.Editora.Id = Convert.ToInt32(formCollection["EditoraDDL"]);
                livro.GrupoPrecificacao.Id = Convert.ToInt32(formCollection["GrupoPrecificacaoDDL"]);
                livro.Estoque = Convert.ToInt32(formCollection["Estoque"]);
                var categoriasId = formCollection["CategoriasDDL"] == null ? new string[] { } : formCollection["CategoriasDDL"].Split(',');
                foreach (var categoriaId in categoriasId)
                {
                    livro.Categorias.Add(new Categoria
                    {
                        Id = Convert.ToInt32(categoriaId)
                    });
                }

                livro.GrupoPrecificacao = _ConsultarGrupoPrecificacao(livro.GrupoPrecificacao)[0];

                if (_PreencherDDLCategoria() == 0 || _PreencherDDLSubcategoria(livro.Subcategoria.Id) == 0 || _PreencherDDLEditora(livro.Editora.Id) == 0 ||
                _PreencherDDLAutor(livro.Autor.Id) == 0 || _PreencherDDLGrupoPrecificacao(livro.GrupoPrecificacao.Id) == 0)
                {
                    ViewBag.Message = "Erro ao consultar DDL";
                    return View(new Livro());
                }

                _resultado = new Fachada().Alterar(livro);

                if (!string.IsNullOrEmpty(_resultado.Message))
                {
                    ViewBag.Erro = _resultado.Message;
                    // apresenta erro
                    return View(livro);
                }
                ViewBag.Message = "Registro alterado com sucesso";
            }
            return View(new Livro());
        }

        // GET: Livro/deletar/5
        public ActionResult Deletar(int id)
        {
            FormCollection formCollection = new FormCollection();
            Resultado result = new Resultado();
            Fachada fachada = new Fachada();
            Livro livro = new Livro
            {
                Id = id
            };

            var livros = _ConsultarLivro(livro);

            if (livros == null)
            {
                ViewBag.Erro = _resultado.Message;
                return HttpNotFound();
            }

            return View(livros[0]);
        }

        // POST: Livro/deletar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deletar(FormCollection formCollection, int id)
        {
            Livro livro = new Livro
            {
                Id = id
            };

            if (ModelState.IsValid)
            {
                Resultado resultado = new Fachada().Excluir(livro);

                if (!string.IsNullOrEmpty(resultado.Message))
                {
                    ViewBag.Erro = resultado.Message;
                    // apresenta erro
                    return View(livro);
                }
                ViewBag.Message = "Registro excluido com sucesso";
            }
            return View(new Livro());
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
            foreach (var item in _resultado.Entidades)
                livros.Add((Livro)item);

            return livros;
        }

        private List<GrupoPrecificacao> _ConsultarGrupoPrecificacao(GrupoPrecificacao grupoPrecificacao)
        {
            _resultado = new Fachada().Consultar(grupoPrecificacao);

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return null;
            }

            var gruposPrecificacao = new List<GrupoPrecificacao>();
            foreach (var item in _resultado.Entidades)
                gruposPrecificacao.Add((GrupoPrecificacao)item);

            return gruposPrecificacao;
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

        private int _PreencherDDLSubcategoria(int idSelected)
        {
            ViewBag.SubcategoriaDDL = "";
            _resultado = new Fachada().Consultar(new Subcategoria());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var subcategorias = new List<Subcategoria>();
            foreach (Subcategoria item in _resultado.Entidades)
                subcategorias.Add(item);

            if(idSelected > 0)
                ViewBag.SubcategoriaDDL = new SelectList(subcategorias, "Id", "Nome", idSelected);
            else
                ViewBag.SubcategoriaDDL = new SelectList(subcategorias, "Id", "Nome");

            return 1;
        }

        private int _PreencherDDLAutor(int idSelected)
        {
            ViewBag.AutorDDL = "";
            _resultado = new Fachada().Consultar(new Autor());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var autores = new List<Autor>();
            foreach (Autor item in _resultado.Entidades)
                autores.Add(item);

            if (idSelected > 0)
                ViewBag.AutorDDL = new SelectList(autores, "Id", "Nome", idSelected);
            else
                ViewBag.AutorDDL = new SelectList(autores, "Id", "Nome");

            return 1;
        }

        private int _PreencherDDLEditora(int idSelected)
        {
            ViewBag.EditoraDDL = "";
            _resultado = new Fachada().Consultar(new Editora());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var editoras = new List<Editora>();
            foreach (Editora item in _resultado.Entidades)
                editoras.Add(item);

            if (idSelected > 0)
                ViewBag.EditoraDDL = new SelectList(editoras, "Id", "Nome", idSelected);
            else
                ViewBag.EditoraDDL = new SelectList(editoras, "Id", "Nome");

            return 1;
        }

        private int _PreencherDDLGrupoPrecificacao(int idSelected)
        {
            ViewBag.GrupoPrecificacaoDDL = "";
            _resultado = new Fachada().Consultar(new GrupoPrecificacao());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var gruposPrecificacao = new List<GrupoPrecificacao>();
            foreach (GrupoPrecificacao item in _resultado.Entidades)
                gruposPrecificacao.Add(item);

            if (idSelected > 0)
                ViewBag.GrupoPrecificacaoDDL = new SelectList(gruposPrecificacao, "Id", "Nome", idSelected);
            else
                ViewBag.GrupoPrecificacaoDDL = new SelectList(gruposPrecificacao, "Id", "Nome");

            return 1;
        }

        private int _PreencherDDLCategoriaAtivacao(int idSelected)
        {
            ViewBag.CategoriaAtivacaoDDL = "";
            _resultado = new Fachada().Consultar(new CategoriaAtivacao());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var categoriasAtivacao = new List<CategoriaAtivacao>();
            foreach (CategoriaAtivacao item in _resultado.Entidades)
                categoriasAtivacao.Add(item);

            if (idSelected > 0)
                ViewBag.CategoriaAtivacaoDDL = new SelectList(categoriasAtivacao, "Id", "Nome", idSelected);
            else
                ViewBag.CategoriaAtivacaoDDL = new SelectList(categoriasAtivacao, "Id", "Nome");

            return 1;
        }

        private int _PreencherDDLCategoriaInativacao(int idSelected)
        {
            ViewBag.CategoriaInativacaoDDL = "";
            _resultado = new Fachada().Consultar(new CategoriaInativacao());

            if (!string.IsNullOrEmpty(_resultado.Message)) // Encontrou erro?
            {
                ViewBag.Message = _resultado.Message;
                return 0;
            }

            var categoriasInativacao = new List<CategoriaInativacao>();
            foreach (CategoriaInativacao item in _resultado.Entidades)
                categoriasInativacao.Add(item);

            if (idSelected > 0)
                ViewBag.CategoriaInativacaoDDL = new SelectList(categoriasInativacao, "Id", "Nome", idSelected);
            else
                ViewBag.CategoriaInativacaoDDL = new SelectList(categoriasInativacao, "Id", "Nome");

            return 1;
        }
    }
}