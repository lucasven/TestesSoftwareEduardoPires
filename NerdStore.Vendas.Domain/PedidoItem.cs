﻿using NerdStore.Core.DomainObjects;
using System;

namespace NerdStore.Vendas.Domain
{
    public class PedidoItem : Entity
    {
        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public Guid PedidoId { get; set; }

        // EF Rel.
        public Pedido Pedido { get; set; }

        internal void AtualizarUnidades(int unidades)
        {
            Quantidade = unidades;
        }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
        {
            if (quantidade < Pedido.MIN_UNIDADES_ITEM)
                throw new DomainException($"Mínimo de {Pedido.MIN_UNIDADES_ITEM} unidades por produto");

            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * ValorUnitario;
        }

        internal void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }
    }
}
