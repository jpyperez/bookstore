using livraria_les.Aplicacao;
using livraria_les.DAL;
using livraria_les.Models;
using livraria_les.Strategies;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace livraria_les.Facade
{
    public class Fachada : IFachada
    {
        /** 
         * Mapa de DAOS, será indexado pelo nome da entidade 
         * O valor é uma instância do DAO para uma dada entidade; 
         */
        private Dictionary<string, IDAL> _dals;

        /**
         * Mapa para conter as regras de negócio de todas operações por entidade;
         * O valor é um mapa que de regras de negócio indexado pela operação
         */
        private Dictionary<string, Dictionary<string, List<IStrategy>>> _rns;

        private Resultado _resultado;


        public Fachada()
        {
            /* Intânciando o Dictionary de DAOS */
            _dals = new Dictionary<string, IDAL>();

            /* Intânciando o Dictionary de Regras de Negócio */
            _rns = new Dictionary<string, Dictionary<string, List<IStrategy>>>();

            /* Criando instâncias dos DAOs a serem utilizados*/
            LivroDAL livDAL = new LivroDAL();
            ClienteDAL cliDAL = new ClienteDAL();
            CategoriaDAL catDAL = new CategoriaDAL();
            SubcategoriaDAL scatDAL = new SubcategoriaDAL();
            CartaoDAL cardDAL = new CartaoDAL();
            PedidoDAL pedDAL = new PedidoDAL();
            EnderecoDAL endDAL = new EnderecoDAL();
            ItemPedidoDAL itemPedDAL = new ItemPedidoDAL();
            UsuarioDAL usuarioDAL = new UsuarioDAL();
            FuncionarioDAL funcionarioDAL = new FuncionarioDAL();
            AutorDAL autorDAL = new AutorDAL();
            EditoraDAL editoraDAL = new EditoraDAL();
            GrupoPrecificacaoDAL grupoPrecificacaoDAL = new GrupoPrecificacaoDAL();
            CategoriaAtivacaoDAL categoriaAtivacaoDAL = new CategoriaAtivacaoDAL();
            CategoriaInativacaoDAL categoriaInativacaoDAL = new CategoriaInativacaoDAL();
            EstoqueDAL estoqueDAL = new EstoqueDAL();
            CupomDAL cupomDAL = new CupomDAL();
            GraficoDAL graficoDAL = new GraficoDAL();

            /* Adicionando cada dal no MAP indexando pelo nome da classe */
            _dals.Add(new Livro().GetType().Name, livDAL);
            _dals.Add(new Cliente().GetType().Name, cliDAL);
            _dals.Add(new Categoria().GetType().Name, catDAL);
            _dals.Add(new Subcategoria().GetType().Name, scatDAL);
            _dals.Add(new Cartao().GetType().Name, cardDAL);
            _dals.Add(new Pedido().GetType().Name, pedDAL);
            _dals.Add(new Endereco().GetType().Name, endDAL);
            _dals.Add(new ItemPedido().GetType().Name, itemPedDAL);
            _dals.Add(new Usuario().GetType().Name, usuarioDAL);
            _dals.Add(new Funcionario().GetType().Name, funcionarioDAL);
            _dals.Add(new Autor().GetType().Name, autorDAL);
            _dals.Add(new Editora().GetType().Name, editoraDAL);
            _dals.Add(new GrupoPrecificacao().GetType().Name, grupoPrecificacaoDAL);
            _dals.Add(new CategoriaAtivacao().GetType().Name, categoriaAtivacaoDAL);
            _dals.Add(new CategoriaInativacao().GetType().Name, categoriaInativacaoDAL);
            _dals.Add(new Estoque().GetType().Name, estoqueDAL);
            _dals.Add(new Cupom().GetType().Name, cupomDAL);
            _dals.Add(new Grafico().GetType().Name, graficoDAL);

            /* Criando instâncias de regras de negócio a serem utilizados*/
            VForcaSenha vForSenha = new VForcaSenha();
            VCartaoDadosObrigatorios vCarDO = new VCartaoDadosObrigatorios();
            VClienteDadosObrigatorios vCliDO = new VClienteDadosObrigatorios();
            VEnderecoDadosObrigatorios vEndDO = new VEnderecoDadosObrigatorios();
            VEstoque vEst = new VEstoque();
            VLivroDadosObrigatorios vLivDO = new VLivroDadosObrigatorios();
            VPrecificacao vPrecificacao = new VPrecificacao();

            /* Criando RNs dos dominios */
            Dictionary<string, List<IStrategy>> rnsLivro = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCliente = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCategoria = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsSubcategoria = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCartao = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsPedido = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsEndereco = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsItemPedido = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsUsuario = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsFuncionario = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsAutor = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsEditora = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsGrupoPrecificacao = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCategoriaAtivacao = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCategoriaInativacao = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsEstoque = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsCupom = new Dictionary<string, List<IStrategy>>();
            Dictionary<string, List<IStrategy>> rnsGrafico = new Dictionary<string, List<IStrategy>>();

            /* Criando uma lista para conter as regras de negócio de livros */
            List<IStrategy> rnsSalvarLivro = new List<IStrategy>()
            {
                vLivDO
            };
            List<IStrategy> rnsConsultarLivro = new List<IStrategy>();
            List<IStrategy> rnsExcluirLivro = new List<IStrategy>();
            List<IStrategy> rnsAlterarLivro = new List<IStrategy>()
            {
                vLivDO,
                vPrecificacao
            };
            List<IStrategy> rnsVisualizarLivro = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Cliente */
            List<IStrategy> rnsSalvarCliente = new List<IStrategy>()
            {
                vForSenha,
                vCliDO
            };
            List<IStrategy> rnsConsultarCliente = new List<IStrategy>();
            List<IStrategy> rnsExcluirCliente = new List<IStrategy>();
            List<IStrategy> rnsAlterarCliente = new List<IStrategy>()
            {
                vCliDO
            };
            List<IStrategy> rnsVisualizarCliente = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Categoria */
            List<IStrategy> rnsSalvarCategoria = new List<IStrategy>();
            List<IStrategy> rnsConsultarCategoria = new List<IStrategy>();
            List<IStrategy> rnsExcluirCategoria = new List<IStrategy>();
            List<IStrategy> rnsAlterarCategoria = new List<IStrategy>();
            List<IStrategy> rnsVisualizarCategoria = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Subcategoria */
            List<IStrategy> rnsSalvarSubcategoria = new List<IStrategy>();
            List<IStrategy> rnsConsultarSubcategoria = new List<IStrategy>();
            List<IStrategy> rnsExcluirSubcategoria = new List<IStrategy>();
            List<IStrategy> rnsAlterarSubcategoria = new List<IStrategy>();
            List<IStrategy> rnsVisualizarSubcategoria = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Cartao */
            List<IStrategy> rnsSalvarCartao = new List<IStrategy>()
            {
                vCarDO
            };
            List<IStrategy> rnsConsultarCartao = new List<IStrategy>();            
            List<IStrategy> rnsExcluirCartao = new List<IStrategy>();
            List<IStrategy> rnsAlterarCartao = new List<IStrategy>()
            {
                vCarDO
            };
            List<IStrategy> rnsVisualizarCartao = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Pedido */
            List<IStrategy> rnsSalvarPedido = new List<IStrategy>();
            List<IStrategy> rnsConsultarPedido = new List<IStrategy>();
            List<IStrategy> rnsExcluirPedido = new List<IStrategy>();
            List<IStrategy> rnsAlterarPedido = new List<IStrategy>();
            List<IStrategy> rnsVisualizarPedido = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Endereco */
            List<IStrategy> rnsSalvarEndereco = new List<IStrategy>()
            {
                vEndDO
            };
            List<IStrategy> rnsConsultarEndereco = new List<IStrategy>();
            List<IStrategy> rnsExcluirEndereco = new List<IStrategy>();
            List<IStrategy> rnsAlterarEndereco = new List<IStrategy>()
            {
                vEndDO
            };
            List<IStrategy> rnsVisualizarEndereco = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de ItemPedido */
            List<IStrategy> rnsSalvarItemPedido = new List<IStrategy>()
            {
                vEst
            };
            List<IStrategy> rnsConsultarItemPedido = new List<IStrategy>();
            List<IStrategy> rnsExcluirItemPedido = new List<IStrategy>();
            List<IStrategy> rnsAlterarItemPedido = new List<IStrategy>()
            {
                vEst
            };
            List<IStrategy> rnsVisualizarItemPedido = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Usuario */
            List<IStrategy> rnsSalvarUsuario = new List<IStrategy>()
            {
                vForSenha
            };
            List<IStrategy> rnsConsultarUsuario = new List<IStrategy>();
            List<IStrategy> rnsExcluirUsuario = new List<IStrategy>();
            List<IStrategy> rnsAlterarUsuario = new List<IStrategy>()
            {
                vForSenha
            };
            List<IStrategy> rnsVisualizarUsuario = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Funcionario */
            List<IStrategy> rnsSalvarFuncionario = new List<IStrategy>();
            List<IStrategy> rnsConsultarFuncionario = new List<IStrategy>();
            List<IStrategy> rnsExcluirFuncionario = new List<IStrategy>();
            List<IStrategy> rnsAlterarFuncionario = new List<IStrategy>();
            List<IStrategy> rnsVisualizarFuncionario = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Autor */
            List<IStrategy> rnsSalvarAutor = new List<IStrategy>();
            List<IStrategy> rnsConsultarAutor = new List<IStrategy>();
            List<IStrategy> rnsExcluirAutor = new List<IStrategy>();
            List<IStrategy> rnsAlterarAutor = new List<IStrategy>();
            List<IStrategy> rnsVisualizarAutor = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Editora */
            List<IStrategy> rnsSalvarEditora = new List<IStrategy>();
            List<IStrategy> rnsConsultarEditora = new List<IStrategy>();
            List<IStrategy> rnsExcluirEditora = new List<IStrategy>();
            List<IStrategy> rnsAlterarEditora = new List<IStrategy>();
            List<IStrategy> rnsVisualizarEditora = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de GrupoPrecificacao */
            List<IStrategy> rnsSalvarGrupoPrecificacao = new List<IStrategy>();
            List<IStrategy> rnsConsultarGrupoPrecificacao = new List<IStrategy>();
            List<IStrategy> rnsExcluirGrupoPrecificacao = new List<IStrategy>();
            List<IStrategy> rnsAlterarGrupoPrecificacao = new List<IStrategy>();
            List<IStrategy> rnsVisualizarGrupoPrecificacao = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de CategoriaAtivacao */
            List<IStrategy> rnsSalvarCategoriaAtivacao = new List<IStrategy>();
            List<IStrategy> rnsConsultarCategoriaAtivacao = new List<IStrategy>();
            List<IStrategy> rnsExcluirCategoriaAtivacao = new List<IStrategy>();
            List<IStrategy> rnsAlterarCategoriaAtivacao = new List<IStrategy>();
            List<IStrategy> rnsVisualizarCategoriaAtivacao = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de CategoriaInativacao */
            List<IStrategy> rnsSalvarCategoriaInativacao = new List<IStrategy>();
            List<IStrategy> rnsConsultarCategoriaInativacao = new List<IStrategy>();
            List<IStrategy> rnsExcluirCategoriaInativacao = new List<IStrategy>();
            List<IStrategy> rnsAlterarCategoriaInativacao = new List<IStrategy>();
            List<IStrategy> rnsVisualizarCategoriaInativacao = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Estoque */
            List<IStrategy> rnsSalvarEstoque = new List<IStrategy>();
            List<IStrategy> rnsConsultarEstoque = new List<IStrategy>()
            {
                vEst
            };
            List<IStrategy> rnsExcluirEstoque = new List<IStrategy>();
            List<IStrategy> rnsAlterarEstoque = new List<IStrategy>();
            List<IStrategy> rnsVisualizarEstoque = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Cupom */
            List<IStrategy> rnsSalvarCupom = new List<IStrategy>();
            List<IStrategy> rnsConsultarCupom = new List<IStrategy>();
            List<IStrategy> rnsExcluirCupom = new List<IStrategy>();
            List<IStrategy> rnsAlterarCupom = new List<IStrategy>();
            List<IStrategy> rnsVisualizarCupom = new List<IStrategy>();

            /* Criando uma lista para conter as regras de negócio de Grafico */
            List<IStrategy> rnsSalvarGrafico = new List<IStrategy>();
            List<IStrategy> rnsConsultarGrafico = new List<IStrategy>();
            List<IStrategy> rnsExcluirGrafico = new List<IStrategy>();
            List<IStrategy> rnsAlterarGrafico = new List<IStrategy>();
            List<IStrategy> rnsVisualizarGrafico = new List<IStrategy>();

            /* Adiciona a lista de regras das operações no mapa do livro */
            rnsLivro.Add("SALVAR", rnsSalvarLivro);
            rnsLivro.Add("CONSULTAR", rnsConsultarLivro);
            rnsLivro.Add("EXCLUIR", rnsExcluirLivro);
            rnsLivro.Add("ALTERAR", rnsAlterarLivro);
            rnsLivro.Add("VISUALIZAR", rnsVisualizarLivro);

            /* Adiciona a lista de regras das operações no mapa do cliente */
            rnsCliente.Add("SALVAR", rnsSalvarCliente);
            rnsCliente.Add("CONSULTAR", rnsConsultarCliente);
            rnsCliente.Add("EXCLUIR", rnsExcluirCliente);
            rnsCliente.Add("ALTERAR", rnsAlterarCliente);
            rnsCliente.Add("VISUALIZAR", rnsVisualizarCliente);

            /* Adiciona a lista de regras das operações no mapa do categoria */
            rnsCategoria.Add("SALVAR", rnsSalvarCategoria);
            rnsCategoria.Add("CONSULTAR", rnsConsultarCategoria);
            rnsCategoria.Add("EXCLUIR", rnsExcluirCategoria);
            rnsCategoria.Add("ALTERAR", rnsAlterarCategoria);
            rnsCategoria.Add("VISUALIZAR", rnsVisualizarCategoria);

            /* Adiciona a lista de regras das operações no mapa do subcategoria */
            rnsSubcategoria.Add("SALVAR", rnsSalvarSubcategoria);
            rnsSubcategoria.Add("CONSULTAR", rnsConsultarSubcategoria);
            rnsSubcategoria.Add("EXCLUIR", rnsExcluirSubcategoria);
            rnsSubcategoria.Add("ALTERAR", rnsAlterarSubcategoria);
            rnsSubcategoria.Add("VISUALIZAR", rnsVisualizarSubcategoria);

            /* Adiciona a lista de regras das operações no mapa do cartao */
            rnsCartao.Add("SALVAR", rnsSalvarCartao);
            rnsCartao.Add("CONSULTAR", rnsConsultarCartao);
            rnsCartao.Add("EXCLUIR", rnsExcluirCartao);
            rnsCartao.Add("ALTERAR", rnsAlterarCartao);
            rnsCartao.Add("VISUALIZAR", rnsVisualizarCartao);

            /* Adiciona a lista de regras das operações no mapa do pedido */
            rnsPedido.Add("SALVAR", rnsSalvarPedido);
            rnsPedido.Add("CONSULTAR", rnsConsultarPedido);
            rnsPedido.Add("EXCLUIR", rnsExcluirPedido);
            rnsPedido.Add("ALTERAR", rnsAlterarPedido);
            rnsPedido.Add("VISUALIZAR", rnsVisualizarPedido);

            /* Adiciona a lista de regras das operações no mapa do endereco */
            rnsEndereco.Add("SALVAR", rnsSalvarEndereco);
            rnsEndereco.Add("CONSULTAR", rnsConsultarEndereco);
            rnsEndereco.Add("EXCLUIR", rnsExcluirEndereco);
            rnsEndereco.Add("ALTERAR", rnsAlterarEndereco);
            rnsEndereco.Add("VISUALIZAR", rnsVisualizarEndereco);

            /* Adiciona a lista de regras das operações no mapa do ItemPedido */
            rnsItemPedido.Add("SALVAR", rnsSalvarItemPedido);
            rnsItemPedido.Add("CONSULTAR", rnsConsultarItemPedido);
            rnsItemPedido.Add("EXCLUIR", rnsExcluirItemPedido);
            rnsItemPedido.Add("ALTERAR", rnsAlterarItemPedido);
            rnsItemPedido.Add("VISUALIZAR", rnsVisualizarItemPedido);

            /* Adiciona a lista de regras das operações no mapa do Usuario */
            rnsUsuario.Add("SALVAR", rnsSalvarUsuario);
            rnsUsuario.Add("CONSULTAR", rnsConsultarUsuario);
            rnsUsuario.Add("EXCLUIR", rnsExcluirUsuario);
            rnsUsuario.Add("ALTERAR", rnsAlterarUsuario);
            rnsUsuario.Add("VISUALIZAR", rnsVisualizarUsuario);

            /* Adiciona a lista de regras das operações no mapa do Funcionario */
            rnsFuncionario.Add("SALVAR", rnsSalvarFuncionario);
            rnsFuncionario.Add("CONSULTAR", rnsConsultarFuncionario);
            rnsFuncionario.Add("EXCLUIR", rnsExcluirFuncionario);
            rnsFuncionario.Add("ALTERAR", rnsAlterarFuncionario);
            rnsFuncionario.Add("VISUALIZAR", rnsVisualizarFuncionario);

            /* Adiciona a lista de regras das operações no mapa do Autor */
            rnsAutor.Add("SALVAR", rnsSalvarAutor);
            rnsAutor.Add("CONSULTAR", rnsConsultarAutor);
            rnsAutor.Add("EXCLUIR", rnsExcluirAutor);
            rnsAutor.Add("ALTERAR", rnsAlterarAutor);
            rnsAutor.Add("VISUALIZAR", rnsVisualizarAutor);

            /* Adiciona a lista de regras das operações no mapa do Editora */
            rnsEditora.Add("SALVAR", rnsSalvarEditora);
            rnsEditora.Add("CONSULTAR", rnsConsultarEditora);
            rnsEditora.Add("EXCLUIR", rnsExcluirEditora);
            rnsEditora.Add("ALTERAR", rnsAlterarEditora);
            rnsEditora.Add("VISUALIZAR", rnsVisualizarEditora);

            /* Adiciona a lista de regras das operações no mapa do GrupoPrecificacao */
            rnsGrupoPrecificacao.Add("SALVAR", rnsSalvarGrupoPrecificacao);
            rnsGrupoPrecificacao.Add("CONSULTAR", rnsConsultarGrupoPrecificacao);
            rnsGrupoPrecificacao.Add("EXCLUIR", rnsExcluirGrupoPrecificacao);
            rnsGrupoPrecificacao.Add("ALTERAR", rnsAlterarGrupoPrecificacao);
            rnsGrupoPrecificacao.Add("VISUALIZAR", rnsVisualizarGrupoPrecificacao);

            /* Adiciona a lista de regras das operações no mapa do CategoriaAtivacao */
            rnsCategoriaAtivacao.Add("SALVAR", rnsSalvarCategoriaAtivacao);
            rnsCategoriaAtivacao.Add("CONSULTAR", rnsConsultarCategoriaAtivacao);
            rnsCategoriaAtivacao.Add("EXCLUIR", rnsExcluirCategoriaAtivacao);
            rnsCategoriaAtivacao.Add("ALTERAR", rnsAlterarCategoriaAtivacao);
            rnsCategoriaAtivacao.Add("VISUALIZAR", rnsVisualizarCategoriaAtivacao);

            /* Adiciona a lista de regras das operações no mapa do CategoriaInativacao */
            rnsCategoriaInativacao.Add("SALVAR", rnsSalvarCategoriaInativacao);
            rnsCategoriaInativacao.Add("CONSULTAR", rnsConsultarCategoriaInativacao);
            rnsCategoriaInativacao.Add("EXCLUIR", rnsExcluirCategoriaInativacao);
            rnsCategoriaInativacao.Add("ALTERAR", rnsAlterarCategoriaInativacao);
            rnsCategoriaInativacao.Add("VISUALIZAR", rnsVisualizarCategoriaInativacao);

            /* Adiciona a lista de regras das operações no mapa do Estoque */
            rnsEstoque.Add("SALVAR", rnsSalvarEstoque);
            rnsEstoque.Add("CONSULTAR", rnsConsultarEstoque);
            rnsEstoque.Add("EXCLUIR", rnsExcluirEstoque);
            rnsEstoque.Add("ALTERAR", rnsAlterarEstoque);
            rnsEstoque.Add("VISUALIZAR", rnsVisualizarEstoque);

            /* Adiciona a lista de regras das operações no mapa do Cupom */
            rnsCupom.Add("SALVAR", rnsSalvarCupom);
            rnsCupom.Add("CONSULTAR", rnsConsultarCupom);
            rnsCupom.Add("EXCLUIR", rnsExcluirCupom);
            rnsCupom.Add("ALTERAR", rnsAlterarCupom);
            rnsCupom.Add("VISUALIZAR", rnsVisualizarCupom);

            /* Adiciona a lista de regras das operações no mapa do Grafico */
            rnsGrafico.Add("SALVAR", rnsSalvarGrafico);
            rnsGrafico.Add("CONSULTAR", rnsConsultarGrafico);
            rnsGrafico.Add("EXCLUIR", rnsExcluirGrafico);
            rnsGrafico.Add("ALTERAR", rnsAlterarGrafico);
            rnsGrafico.Add("VISUALIZAR", rnsVisualizarGrafico);

            /* Adiciona o mapa com as regras indexadas pelas operações no mapa geral indexado pelo nome da entidade */
            _rns.Add(new Livro().GetType().Name, rnsLivro);
            _rns.Add(new Cliente().GetType().Name, rnsCliente);
            _rns.Add(new Categoria().GetType().Name, rnsCategoria);
            _rns.Add(new Subcategoria().GetType().Name, rnsSubcategoria);
            _rns.Add(new Cartao().GetType().Name, rnsCartao);
            _rns.Add(new Pedido().GetType().Name, rnsPedido);
            _rns.Add(new Endereco().GetType().Name, rnsEndereco);
            _rns.Add(new ItemPedido().GetType().Name, rnsItemPedido);
            _rns.Add(new Usuario().GetType().Name, rnsUsuario);
            _rns.Add(new Funcionario().GetType().Name, rnsFuncionario);
            _rns.Add(new Autor().GetType().Name, rnsAutor);
            _rns.Add(new Editora().GetType().Name, rnsEditora);
            _rns.Add(new GrupoPrecificacao().GetType().Name, rnsGrupoPrecificacao);
            _rns.Add(new CategoriaAtivacao().GetType().Name, rnsCategoriaAtivacao);
            _rns.Add(new CategoriaInativacao().GetType().Name, rnsCategoriaInativacao);
            _rns.Add(new Estoque().GetType().Name, rnsEstoque);
            _rns.Add(new Cupom().GetType().Name, rnsCupom);
            _rns.Add(new Grafico().GetType().Name, rnsGrafico);
        }

        public Resultado Salvar(IEntidade entidade)
        {
            _resultado = new Resultado();
            string nmClasse = entidade.GetType().Name;

            string msg = ExecutarRegras(entidade, "SALVAR");

            if (msg == null)
            {
                IDAL dal = _dals[nmClasse];
                try
                {
                    dal.Salvar(entidade);
                    List<IEntidade> entidades = new List<IEntidade>
                    {
                        entidade
                    };
                    _resultado.Entidades = entidades;
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.StackTrace);
                    _resultado.Message = "Não foi possível realizar o registro!";
                }
            }
            else
            {
                _resultado.Message = msg;
            }
            return _resultado;
        }

        public Resultado Alterar(IEntidade entidade)
        {
            _resultado = new Resultado();
            string nmClasse = entidade.GetType().Name;

            string msg = ExecutarRegras(entidade, "ALTERAR");

            if (msg == null)
            {
                IDAL dal = _dals[nmClasse];
                try
                {
                    dal.Alterar(entidade);
                    List<IEntidade> entidades = new List<IEntidade>
                    {
                        entidade
                    };
                    _resultado.Entidades = entidades;
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.StackTrace);
                    _resultado.Message = "Não foi possível realizar o registro!";
                }
            }
            else
            {
                _resultado.Message = msg;
            }
            return _resultado;
        }

        public Resultado Excluir(IEntidade entidade)
        {
            _resultado = new Resultado();
            string nmClasse = entidade.GetType().Name;

            string msg = ExecutarRegras(entidade, "EXCLUIR");

            if (msg == null)
            {
                IDAL dal = _dals[nmClasse];
                try
                {
                    dal.Excluir(entidade);
                    List<IEntidade> entidades = new List<IEntidade>
                    {
                        entidade
                    };
                    _resultado.Entidades = entidades;
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.StackTrace);
                    _resultado.Message = "Não foi possível excluir o registro!";
                }
            }
            else
            {
                _resultado.Message = msg;
            }
            return _resultado;
        }

        public Resultado Consultar(IEntidade entidade)
        {
            _resultado = new Resultado();
            string nmClasse = entidade.GetType().Name;

            string msg = ExecutarRegras(entidade, "CONSULTAR");

            if (msg == null)
            {
                IDAL dal = _dals[nmClasse];
                try
                {
                    _resultado.Entidades = dal.Consultar(entidade);
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.StackTrace);
                    _resultado.Message = "Não foi possível realizar a consulta!";
                }
            }
            else
            {
                _resultado.Message = msg;
            }
            return _resultado;
        }

        public Resultado Visualizar(IEntidade entidade)
        {
            _resultado = new Resultado
            {
                Entidades = new List<IEntidade>(1)
                {
                    entidade
                }
            };
            return _resultado;
        }

        private string ExecutarRegras(IEntidade entidade, string operacao)
        {
            string nmClasse = entidade.GetType().Name;
            string msg = "";

            Dictionary<string, List<IStrategy>> regrasOperacao = _rns[nmClasse];

            if (regrasOperacao != null)
            {
                List<IStrategy> regras = regrasOperacao[operacao];

                if (regras != null)
                {
                    foreach (IStrategy s in regras)
                    {
                        string m = s.Processar(entidade);

                        if (m != null)
                        {
                            msg += m;
                            msg += "//";
                        }
                    }
                }
            }

            if (msg.Length > 0)
                return msg;
            else
                return null;
        }

    }
}