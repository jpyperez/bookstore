using livraria_les.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace livraria_les.DAL
{
    public class ClienteDAL : AbstractDAL
    {
        public ClienteDAL()
        {
            table = "cliente";
            idTable = "id_cliente";
        }

        public override int Salvar(IEntidade entidade)
        {
            UsuarioDAL usuDAL = new UsuarioDAL();
            TelefoneDAL telDAL = new TelefoneDAL();
            Cliente cliente = (Cliente)entidade;

            if (telDAL.Salvar(cliente.Telefone) == 0) // Deu erro?
                return 0;

            if (usuDAL.Salvar(cliente.Usuario) == 0) // Deu erro?
                return 0;

            string commandText = "INSERT INTO cliente (nome, genero, dt_nascimento, cpf, telefone_id, dt_cadastro, usuario_id)" +
                " VALUES (@nome, @genero, @dt_nascimento, @cpf, @telefone_id, @dt_cadastro, @usuario_id)";

            var parametros = new Dictionary<string, object>
            {
                { "nome", cliente.Nome },
                { "genero", cliente.Genero },
                { "dt_nascimento", cliente.DtNascimento },
                { "cpf", cliente.CPF },
                { "telefone_id", cliente.Telefone.Id },
                { "usuario_id", cliente.Usuario.Id },
                { "dt_cadastro", DateTime.Now }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public override int Alterar(IEntidade entidade)
        {
            Cliente cliente = (Cliente)entidade;

            var clienteHistorico = Consultar(new Cliente { Id = cliente.Id })[0];
            if (SalvarHistorico(clienteHistorico) == 0) // Deu erro?
                return 0;

            var commandText = "UPDATE cliente SET nome = @nome, genero = @genero, dt_nascimento = @dt_nascimento, " +
                "cpf = cpf, dt_alteracao = @dt_alteracao " +
                "where id_cliente = @id";

            var parametros = new Dictionary<string, object>
            {
                { "Id", cliente.Id},
                { "nome", cliente.Nome },
                { "genero", cliente.Genero },
                { "dt_nascimento", cliente.DtNascimento },
                { "cpf", cliente.CPF },
                { "dt_alteracao", DateTime.Now }
            };

            return conexao.ExecutaComando(commandText, parametros);
         }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Cliente cliente = (Cliente)entidade;
            var clientes = new List<IEntidade>();
            bool flgId = false, flgBuscaAtivo = false, flgNome = false,
                flgGenero = false, flgCPF = false, flgRanking = false;
            CartaoDAL cardDAL = new CartaoDAL();
            UsuarioDAL usuDAL = new UsuarioDAL();
            TelefoneDAL telDAL = new TelefoneDAL();
            CupomDAL cupomDAL = new CupomDAL();
            EnderecoDAL enderecoDAL = new EnderecoDAL();

            string strQuery = "SELECT * FROM cliente WHERE ";
            if (cliente.Id > 0)
            {
                strQuery += "id_cliente like @id AND  ";
                flgId = true;
            }
            if (!string.IsNullOrEmpty(cliente.Nome))
            {
                strQuery += "nome like @nome AND  ";
                flgNome = true;
            }
            if (!string.IsNullOrEmpty(cliente.Genero))
            {
                strQuery += "genero like @genero AND  ";
                flgGenero = true;
            }
            if (!string.IsNullOrEmpty(cliente.CPF))
            {
                strQuery += "cpf like @cpf AND  ";
                flgCPF = true;
            }
            if (!string.IsNullOrEmpty(cliente.Ranking))
            {
                strQuery += "ranking like @ranking AND  ";
                flgRanking = true;
            }
            if (!string.IsNullOrEmpty(cliente.BuscaAtivo))
            {
                strQuery += "ativo = @ativo AND  ";
                flgBuscaAtivo = true;
            }

            strQuery = strQuery.Substring(0, strQuery.Length - 6);

            var parametros = new Dictionary<string, object>();

            if (flgId)
                parametros.Add("id", cliente.Id);
            if (flgBuscaAtivo)
                parametros.Add("ativo", cliente.BuscaAtivo);
            if (flgNome)
                parametros.Add("nome", "%" + cliente.Nome+ "%");
            if (flgGenero)
                parametros.Add("genero", "%" + cliente.Genero + "%");
            if (flgCPF)
                parametros.Add("cpf", "%" + cliente.CPF + "%");
            if (flgRanking)
                parametros.Add("ranking", "%" + cliente.Ranking + "%");

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var tempCliente = new Cliente
                {
                    Id = Convert.ToInt32(!string.IsNullOrEmpty(row["id_cliente"]) ? row["id_cliente"] : "0"),
                    Nome = row["nome"],
                    Genero = row["genero"],
                    DtNascimento = DateTime.Parse(row["dt_nascimento"]),
                    CPF = row["cpf"],
                    Ativo = row["ativo"],
                    Telefone = telDAL.Consultar(new Telefone { Id = Convert.ToInt32(row["telefone_id"]) }).ConvertAll(x => (Telefone)x)[0],
                    Cartoes = cardDAL.Consultar(new Cartao { ClienteId = Convert.ToInt32(row["id_cliente"]) }).ConvertAll(x => (Cartao)x),
                    Cupons = cupomDAL.Consultar(new Cupom { ClienteId = Convert.ToInt32(row["id_cliente"]) }).ConvertAll(x => (Cupom)x),
                    Enderecos = enderecoDAL.Consultar(new Endereco { ClienteId = Convert.ToInt32(row["id_cliente"]) }).ConvertAll(x => (Endereco)x),
                    Usuario = usuDAL.Consultar(new Usuario { Id = Convert.ToInt32(row["usuario_id"]) }).ConvertAll(x => (Usuario)x)[0]
                };
                tempCliente.Historicos = ConsultarHistorico(new Cliente { Id = Convert.ToInt32(row["id_cliente"]) }).ConvertAll(x => (Cliente)x);
                clientes.Add(tempCliente);
            }

            return clientes;
        }

        public int SalvarHistorico(IEntidade entidade)
        {
            var cliente = (Cliente)entidade;

            string commandText = "INSERT INTO historico (id_class, class_name, json_dados, dt_alteracao)" +
                " VALUES (@id_class, @class_name, @json_dados, @dt_alteracao)";

            var parametros = new Dictionary<string, object>
            {
                { "id_class", cliente.Id },
                { "class_name", cliente.GetType().Name },
                { "json_dados", JsonConvert.SerializeObject(cliente) },
                { "dt_alteracao", DateTime.Now }
            };

            return conexao.ExecutaComando(commandText, parametros);
        }

        public List<IEntidade> ConsultarHistorico(IEntidade entidade)
        {
            var cliente = (Cliente)entidade;
            var clientes = new List<IEntidade>();

            var strQuery = "SELECT * FROM historico where id_class = @id and class_name = @class_name order by dt_alteracao desc";

            var parametros = new Dictionary<string, object> {
                { "id", cliente.Id },
                { "class_name", cliente.GetType().Name }
            };

            var rows = conexao.ExecutaComandoComRetorno(strQuery, parametros);
            foreach (var row in rows)
            {
                var clienteHistorico = JsonConvert.DeserializeObject<Cliente>(row["json_dados"]);

                clientes.Add(clienteHistorico);
            }

            return clientes;
        }
    }
}