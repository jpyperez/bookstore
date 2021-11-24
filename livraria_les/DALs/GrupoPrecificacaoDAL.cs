using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class GrupoPrecificacaoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            GrupoPrecificacao grupoPrecificacao = (GrupoPrecificacao)entidade;

            string commandText = "INSERT INTO grupo_precificacao (nome, porcentagem)" +
                " VALUES (@nome, @porcentagem)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", grupoPrecificacao.Nome },
                { "porcentagem", grupoPrecificacao.Porcentagem },
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            GrupoPrecificacao grupoPrecificacao = (GrupoPrecificacao)entidade;

            var commandText = "UPDATE grupo_precificacao SET nome = @nome, porcentagem = @porcentagem where id_grupoPrecificacao = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", grupoPrecificacao.Id},
                { "nome", grupoPrecificacao.Nome },
                { "porcentagem", grupoPrecificacao.Porcentagem }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            GrupoPrecificacao grupoPrecificacao = (GrupoPrecificacao)entidade;
            var grupoPrecificacaos = new List<IEntidade>();
            bool flgId = false;

            string strQuery = "SELECT * FROM grupo_precificacao ";
            if (grupoPrecificacao.Id > 0)
            {
                strQuery += "WHERE id_grupo_precificacao = @id";
                flgId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", grupoPrecificacao.Id);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempGrupoPrecificacao = new GrupoPrecificacao
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_grupo_precificacao"]) ? row["id_grupo_precificacao"] : "0"),
                    Nome = row["nome"],
                    Porcentagem = Convert.ToInt32(row["porcentagem"])
                };
                grupoPrecificacaos.Add(tempGrupoPrecificacao);
            }

            return grupoPrecificacaos;
        }
    }
}