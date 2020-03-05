using Features.Clientes;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace Features.Tests
{
    [Collection(nameof(ClienteAutoMockerCollection))]
    public class ClienteServiceFluentAssertionTests
    {
        readonly ClienteTestsAutoMockerFixture _clienteTestsAutoMockerFixture;

        public ClienteServiceFluentAssertionTests(ClienteTestsAutoMockerFixture clienteTestsBogusFixture)
        {
            _clienteTestsAutoMockerFixture = clienteTestsBogusFixture;
            _clienteService = _clienteTestsAutoMockerFixture.ObterClienteService();
        }

        ClienteService _clienteService;

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsAutoMockerFixture.GerarClienteValido();

            // Act
            _clienteService.Adicionar(cliente);

            // Assert
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Once);
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None));
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            // Arrange
            var cliente = _clienteTestsAutoMockerFixture.GerarClienteInvalido();

            // Act
            _clienteService.Adicionar(cliente);

            // Assert
            //Assert.False(cliente.EhValido());
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.Adicionar(cliente), Times.Never);
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IMediator>().Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);

            // Assert
            cliente.EhValido().Should().BeFalse("Possui incosistencias");
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Fluent Assertions Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            // Arrange
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Setup(c => c.ObterTodos())
                .Returns(_clienteTestsAutoMockerFixture.GerarClientesVariados());

            //Act
            var clientes = _clienteService.ObterTodosAtivos();

            //Assert
            _clienteTestsAutoMockerFixture.Mocker.GetMock<IClienteRepository>().Verify(r => r.ObterTodos(), Times.Once);
            clientes.Any().Should().BeTrue();
            clientes.Should().HaveCountGreaterOrEqualTo(1).And.OnlyHaveUniqueItems();
            clientes.Should().NotContain(c => !c.Ativo);

            _clienteService.ExecutionTimeOf(c => c.ObterTodosAtivos())
                .Should()
                .BeLessOrEqualTo(50.Milliseconds(), "Porque deve ser executado muitas vezes");
            
            //Assert.False(clientes.Count(c => !c.Ativo) > 0);
        }
    }
}
