using MediatR;
using Moq;
using Moq.AutoMock;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {
        private readonly Guid _clienteId;
        private readonly Guid _produtoId;
        private readonly Pedido _pedido;
        private readonly AutoMocker _mocker;
        private readonly PedidoCommandHandler _pedidoHandler;

        public PedidoCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _pedidoHandler = _mocker.CreateInstance<PedidoCommandHandler>();

            _clienteId = Guid.NewGuid();
            _produtoId = Guid.NewGuid();

            _pedido = Pedido.PedidoFactory.NovoPedidoRascunho(_clienteId);
        }


        [Fact(DisplayName = "Adicionar Item Novo Pedido com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            //arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(Guid.NewGuid(), Guid.NewGuid(), "Produto Teste", 2, 100);

            _mocker.GetMock<IPedidoRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            //act
            var result = await _pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            //assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(c => c.Adicionar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(c => c.UnitOfWork.Commit(), Times.Once);
            //mocker.GetMock<IMediator>().Verify(c => c.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Item Novo Pedido Rascunho com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoItemPedidoAoRascunho_DeveExecutarComSucesso()
        {
            //arrange
            var pedidoItemExistente = new PedidoItem(Guid.NewGuid(), "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(_clienteId, Guid.NewGuid(), "Produto Teste", 2, 100);

            _mocker.GetMock<IPedidoRepository>().Setup(c => c.ObterPedidoRascunhoPorClienteId(_clienteId)).Returns(Task.FromResult(_pedido));
            _mocker.GetMock<IPedidoRepository>().Setup(c => c.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            //act
            var result = await _pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            //assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.AdicionarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Item Existente Pedido Rascunho com Sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_ItemExistenteAoPedidoRascunho_DeveExecutarComSucesso()
        {
            //arrange
            var pedidoItemExistente = new PedidoItem(_produtoId, "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(_clienteId, _produtoId, "Produto Xpto", 2, 100);

            _mocker.GetMock<IPedidoRepository>().Setup(c => c.ObterPedidoRascunhoPorClienteId(_clienteId)).Returns(Task.FromResult(_pedido));
            _mocker.GetMock<IPedidoRepository>().Setup(c => c.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            //act
            var result = await _pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            //assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.AtualizarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar Item Command Invalido")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_CommandInvalido_DeveRetornarFalsoELancarEventosDeNotificacao()
        {
            //arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(Guid.Empty, Guid.Empty, "", 0, 0);

            //act
            var result = await _pedidoHandler.Handle(pedidoCommand, CancellationToken.None);

            //assert
            Assert.False(result);
            _mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Exactly(5));
        }
    }
}
