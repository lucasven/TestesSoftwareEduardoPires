using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Core.DomainObjects
{
    class Dimensoes
    {
        public decimal Altura { get; private set; }
        public decimal Largura { get; private set; }
        public decimal Profundidade { get; private set; }

        public Dimensoes(decimal altura, decimal largura, decimal profundidade)
        {
            if (altura <= 0) throw new DomanException("A altura deve ser maior que 0");
            if (largura <= 0) throw new DomanException("A largura deve ser maior que 0");
            if (profundidade <= 0) throw new DomanException("A profundidade deve ser maior que 0");

            Altura = altura;
            Largura = largura;
            Profundidade = profundidade;
        }

        public string DescricaoFormatada()
        {
            return $"LxAxP: {Largura} x {Altura} x {Profundidade}";
        }
    }
}
