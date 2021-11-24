using livraria_les.Models;
using System;
using System.Collections.Generic;

namespace livraria_les.Aplicacao
{
    public class Grafico : EntidadeAplicacao
    {
        public Grafico()
        {
            Categorias = new List<Categoria>();
        }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<Categoria> Categorias { get; set; }
        public string Status { get; set; }
    }
}