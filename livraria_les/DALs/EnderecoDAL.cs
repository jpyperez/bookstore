using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    class EnderecoDAL : AbstractDAL
    {
        public override int Salvar(IEntidade entidade)
        {
            Endereco endereco = (Endereco)entidade;

            string commandText = "INSERT INTO endereco (nome, /*tipo_residencia_id, */tipo_logradouro, logradouro, numero, bairro, " +
                "cep, cidade, estado, pais, observacoes, preferencial, dt_cadastro) VALUES (@nome, /*@tipo_residencia_id, */@tipo_logradouro, " +
                "@logradouro, @numero, @bairro, @cep, @cidade, @estado, @pais, @observacoes, @preferencial, @dt_cadastro)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", endereco.Alias },
                //{ "tipo_residencia_id", endereco.TipoResidencia.Id },
                { "tipo_logradouro", endereco.TipoLogradouro },
                { "logradouro", endereco.Logradouro },
                { "numero", endereco.Numero },
                { "bairro", endereco.Bairro },
                { "cep", endereco.CEP },
                { "cidade", endereco.Cidade },
                { "estado", endereco.Estado },
                { "pais", endereco.Pais },
                { "observacoes", endereco.Observacoes },
                { "preferencial", endereco.Preferencial },
                { "dt_cadastro", DateTime.Now }
            };

            if (conexao.ExecutaComando(commandText, parametros) == 0) // Deu erro?
                return 0;

            endereco.Id = conexao.LastInsertedId();

            commandText = "INSERT INTO cliente_endereco (cliente_id, endereco_id)" +
                    " VALUES (@cliente_id, @endereco_id)";

            parametros = new Dictionary<string, object>
            {
                { "cliente_id", endereco.ClienteId },
                { "endereco_id", endereco.Id }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Endereco endereco = (Endereco)entidade;

            string commandText = "UPDATE endereco SET nome = @nome, /*tipo_residencia_id = @tipo_residencia_id, */" +
                "tipo_logradouro = @tipo_logradouro, logradouro = @logradouro, numero = @numero, bairro = @bairro, " +
                "cep = @cep, cidade = @cidade, estado = @estado, pais = @pais, observacoes = @observacoes, " +
                "preferencial = @preferencial where id_endereco = id";

            var parametros = new Dictionary<string, object>
            {
                { "nome", endereco.Alias },
                //{ "tipo_residencia_id", endereco.TipoResidencia.Id },
                { "tipo_logradouro", endereco.TipoLogradouro },
                { "logradouro", endereco.Logradouro },
                { "numero", endereco.Numero },
                { "bairro", endereco.Bairro },
                { "cep", endereco.CEP },
                { "cidade", endereco.Cidade },
                { "estado", endereco.Estado },
                { "pais", endereco.Pais },
                { "observacoes", endereco.Observacoes },
                { "preferencial", endereco.Preferencial },
                { "id", endereco.Id }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Endereco endereco = (Endereco)entidade;
            var enderecos = new List<IEntidade>();
            bool flgId = false, flgClienteId = false;

            string strQuery = "SELECT * FROM endereco ";

            if(endereco.Id > 0)
            {
                strQuery += "WHERE id_endereco = @id_endereco";
                flgId = true;
            }
            else if (endereco.ClienteId > 0)
            {
                strQuery += "INNER JOIN cliente_endereco ON id_endereco = endereco_id where cliente_id = @cliente_id";
                flgClienteId = true;
            }

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id_endereco", endereco.Id);
            if (flgClienteId)
                parametros.Add("cliente_id", endereco.ClienteId);

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempEndereco = new Endereco
                {
                    Id = int.Parse(!string.IsNullOrEmpty(row["id_endereco"]) ? row["id_endereco"] : "0"),
                    Alias = row["nome"],
                    //TipoResidencia = new TipoResidencia { Id = int.Parse(row["tipo_residencia_id"]) },
                    TipoLogradouro = row["tipo_logradouro"],
                    Logradouro = row["logradouro"],
                    Numero = row["numero"],
                    Bairro = row["bairro"],
                    CEP = row["cep"],
                    Cidade = row["cidade"],
                    Estado = row["estado"],
                    Pais = row["pais"],
                    Observacoes = row["observacoes"],
                    Preferencial = Convert.ToInt32(row["preferencial"])
                };
                enderecos.Add(tempEndereco);
            }
            return enderecos;
        }
    }
}