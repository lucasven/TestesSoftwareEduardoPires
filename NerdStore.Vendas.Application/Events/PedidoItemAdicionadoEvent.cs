﻿using MediatR;
using NerdStore.Core.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Vendas.Application.Events
{
    public class PedidoItemAdicionadoEvent : Event
    {
        public Guid ClienteId { get; set; }
        public Guid ProdutoId { get; set; }
        public Guid PedidoId { get; set; }
        public string ProdutoNome { get; set; }
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }

        public PedidoItemAdicionadoEvent(Guid clienteId, Guid pedidoId, Guid produtoId, string produtoNome, decimal valorUnitario, int quantidade)
        {
            ClienteId = clienteId;
            PedidoId = pedidoId;
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            ValorUnitario = valorUnitario;
            Quantidade = quantidade;
        }
    }
}
