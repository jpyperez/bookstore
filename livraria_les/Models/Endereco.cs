namespace livraria_les.Models
{
    public class Endereco : EntidadeDominio
    {
        public Endereco()
        {
            TipoResidencia = new TipoResidencia();
        }

        public TipoResidencia TipoResidencia { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Observacoes { get; set; }
        public string Alias { get; set; }
        public int Preferencial { get; set; }
        public int ClienteId { get; set; }
    }
}