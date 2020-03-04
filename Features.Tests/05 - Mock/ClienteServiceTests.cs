using Features.Clientes;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteServiceTests
    {
        readonly ClienteTestsBogusFixture _clienteTestsBogusFixture;

        public ClienteServiceTests(ClienteTestsBogusFixture clienteTestsBogusFixture)
        {
            _clienteTestsBogusFixture = clienteTestsBogusFixture;
        }

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsBogusFixture.GerarClienteValido();
            var clienteRepo = new Mock<IClienteRepository>();
            var mediator = new Mock<IMediator>();

            var clienteService = new ClienteService(clienteRepo.Object, mediator.Object);

            // Act


            // Assert

        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {

        }


    }
}
