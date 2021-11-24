using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Data;

namespace livraria_les.Util
{
    public class Conexao : IDisposable
    {
        private MySqlConnection _conexao;
        private MySqlCommand _cmdComando;

        public Conexao()
        {
            var conexaoString = ConfigurationManager.ConnectionStrings[2].ConnectionString;
            _conexao = new MySqlConnection(conexaoString);
        }

        public int ExecutaComando(string comandoSQL, Dictionary<string, object> parametros)
        {
            var resultado = 0;
            if (string.IsNullOrEmpty(comandoSQL))
            {
                throw new ArgumentException("O comandoSQL não pode ser nulo ou vazio");
            }
            try
            {
                AbrirConexao();
                var cmdComando = CriarComando(comandoSQL, parametros);
                resultado = cmdComando.ExecuteNonQuery();
            }
            finally
            {
                FecharConexao();
            }

            return resultado;
        }

        public List<Dictionary<string, string>> ExecutaComandoComRetorno(string comandoSQL, Dictionary<string, object> parametros = null)
        {
            List<Dictionary<string, string>> linhas = null;

            if (string.IsNullOrEmpty(comandoSQL))
            {
                throw new ArgumentException("O comandoSQL não pode ser nulo ou vazio");
            }
            try
            {
                AbrirConexao();
                _cmdComando = CriarComando(comandoSQL, parametros);
                using (var reader = _cmdComando.ExecuteReader())
                {
                    linhas = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var linha = new Dictionary<string, string>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var nomeDaColuna = reader.GetName(i);
                            var valorDaColuna = reader.IsDBNull(i) ? null : reader.GetString(i);
                            linha.Add(nomeDaColuna, valorDaColuna);
                        }
                        linhas.Add(linha);
                    }
                }
            }
            finally
            {
                FecharConexao();
            }

            return linhas;
        }

        public int LastInsertedId()
        {
            return Convert.ToInt32(_cmdComando.LastInsertedId);
        }

        private MySqlCommand CriarComando(string comandoSQL, Dictionary<string, object> parametros)
        {
            _cmdComando = _conexao.CreateCommand();
            _cmdComando.CommandText = comandoSQL;
            AdicionarParamatros(_cmdComando, parametros);
            return _cmdComando;
        }

        private static void AdicionarParamatros(MySqlCommand cmdComando, Dictionary<string, object> parametros)
        {
            if (parametros == null)
                return;

            foreach (var item in parametros)
            {
                var parametro = cmdComando.CreateParameter();
                parametro.ParameterName = item.Key;
                parametro.Value = item.Value ?? DBNull.Value;
                cmdComando.Parameters.Add(parametro);
            }
        }

        private void AbrirConexao()
        {
            if (_conexao.State == ConnectionState.Open) return;

            _conexao.Open();
        }

        private void FecharConexao()
        {
            if (_conexao.State == ConnectionState.Open)
                _conexao.Close();
        }

        public void Dispose()
        {
            if (_conexao == null) return;

            _conexao.Dispose();
            _conexao = null;
        }
    }
}