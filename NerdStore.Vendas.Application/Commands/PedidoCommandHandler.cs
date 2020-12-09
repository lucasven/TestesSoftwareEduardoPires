using MediatR;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using NerdStore.Core.DomainObjects;
using NerdStore.Core.Messages;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IPedidoRepository pedidoRepository;
        public IMediator Mediator { get; }

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediator mediator)
        {
            this.pedidoRepository = pedidoRepository;
            Mediator = mediator;
        }


        public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if(!ValidarComando(message)) return false;

            var pedido = await pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);
            if (pedido == null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
                pedido.AdicionarItem(pedidoItem);
                pedidoRepository.Adicionar(pedido);
            }
            else
            {
                var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);
                pedido.AdicionarItem(pedidoItem);

                if(pedidoItemExistente)
                {
                    pedidoRepository.AtualizarItem(pedido.PedidoItems.FirstOrDefault(c => c.ProdutoId == pedidoItem.ProdutoId));
                }
                else
                {
                    pedidoRepository.AdicionarItem(pedidoItem);
                }

                pedidoRepository.Atualizar(pedido);
            }



            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(pedido.ClienteId, pedido.Id, message.ProdutoId, message.Nome, message.ValorUnitario, message.Quantidade));
            
            return await pedidoRepository.UnitOfWork.Commit();
        }

        public bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                Mediator.Publish(new DomainNotification(message.MessageType, error.ErrorMessage));
            }

            return false;
        }
    }
}
