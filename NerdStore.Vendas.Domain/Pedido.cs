using FluentValidation.Results;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NerdStore.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;

        protected Pedido()
        {
            _pedidoItems = new Collection<PedidoItem>();
        }

        public int Codigo { get; private set; }

        public DateTime DataCadastro { get; set; }

        public Guid ClienteId { get; private set; }

        public decimal ValorTotal { get; private set; }

        private readonly Collection<PedidoItem> _pedidoItems;

        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;

        public PedidoStatus PedidoStatus { get; private set; }

        public bool VoucherUtilizado { get; set; }
        public Voucher Voucher { get; set; }
        public Guid? VoucherId { get; private set; }

        public decimal Desconto { get; set; }

        public ValidationResult AplicarVoucher(Voucher voucher)
        {
            var result = voucher.ValidarSeAplicavel();
            if (!result.IsValid) return result;

            Voucher = voucher;
            VoucherUtilizado = true;

            CalcularValorPedido();

            return result;
        }

        public void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if(Voucher.TipoDescontoVoucher == TipoDescontoVoucher.Valor)
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.PercentualDesconto.HasValue)
                {
                    desconto = (ValorTotal * Voucher.PercentualDesconto.Value) / 100;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;
        }

        public bool PedidoItemExistente(PedidoItem item)
        {
            return _pedidoItems.Any(c => c.ProdutoId == item.ProdutoId);
        }

        private void ValidarQuantidadeItensPermitida(PedidoItem item)
        {
            var quantidadeItens = item.Quantidade;
            if(PedidoItemExistente(item))
            {
                var itemExistenete = _pedidoItems.FirstOrDefault(c => c.ProdutoId == item.ProdutoId);
                quantidadeItens += itemExistenete.Quantidade;
            }

            if (quantidadeItens > MAX_UNIDADES_ITEM)
                throw new DomainException($"Máximo de {MAX_UNIDADES_ITEM} unidades por produto");
        }

        public void AdicionarItem(PedidoItem item)
        {
            ValidarQuantidadeItensPermitida(item);

            if (_pedidoItems.Any(p => p.ProdutoId == item.ProdutoId))
            {
                var itemExistente = _pedidoItems.First(c => c.ProdutoId == item.ProdutoId);

                itemExistente.AdicionarUnidades(item.Quantidade);
                item = itemExistente;

                _pedidoItems.Remove(itemExistente);
            }

            _pedidoItems.Add(item);
            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarItemPedidoInexistente(pedidoItem);
            ValidarQuantidadeItensPermitida(pedidoItem);

            var itemExistente = _pedidoItems.First(c => c.ProdutoId == pedidoItem.ProdutoId);

            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarItemPedidoInexistente(pedidoItem);

            _pedidoItems.Remove(pedidoItem);

            CalcularValorPedido();
        }

        public void ValidarItemPedidoInexistente(PedidoItem pedidoItem)
        {
            if (!PedidoItemExistente(pedidoItem))
                throw new DomainException("O item não existe no pedido");
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(c => c.CalcularValor());
            CalcularValorTotalDesconto();
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido()
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }

        public void AtualizarUnidades(PedidoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }
    }
}
