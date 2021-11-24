using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class Cliente : Pessoa
    {
        public Cliente()
        {
            Enderecos = new List<Endereco>();
            Cartoes = new List<Cartao>();
            Telefone = new Telefone();
            Cupons = new List<Cupom>();
            BuscaCartao = false;
            Historicos = new List<Cliente>();
        }

        [DisplayName("Cliente Id")]
        public override int Id { get; set; }
        public string Genero { get; set; }
        [DisplayName("Data de Nascimento")]
        public DateTime DtNascimento { get; set; }
        public string CPF { get; set; }
        public List<Endereco> Enderecos { get; set; }
        public List<Cartao> Cartoes { get; set; }
        public List<Cupom> Cupons { get; set; }
        public Telefone Telefone { get; set; }
        public bool BuscaCartao { get; set; }
        public string Ranking { get; set; }
        public List<Cliente> Historicos { get; set; }
    }
}