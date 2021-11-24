using System;
using System.ComponentModel;

namespace livraria_les.Models
{
    public class Cartao : FormaPagamento
    {
        [DisplayName("Número")]
        public string Numero { get; set; }
        [DisplayName("Nome Impresso")]
        public string NomeImpresso { get; set; }
        public string Bandeira { get; set; }
        [DisplayName("Código de Segurança")]
        public int CodSeguranca { get; set; }
        public int Preferencial { get; set; } // 0 - Não preferencial / 1 - Preferencial
        [DisplayName("Data de Vencimento")]
        public DateTime DtVencimento { get; set; }
        public int ClienteId { get; set; }
    }
}