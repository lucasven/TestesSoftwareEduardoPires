using Features.Clientes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteCollection))]
    public class ClienteTesteInvalido
    {
        private readonly ClienteTestsFixture _clienteTestsFixture;

        public ClienteTesteInvalido(ClienteTestsFixture clienteTestsFixture)
        {
            _clienteTestsFixture = clienteTestsFixture;
        }

        [Fact(DisplayName = "Novo Cliente Invalido")]
        [Trait("Categoria", "Cliente Fixture Testes")]
        public void Cliente_NovoCliente_DeveEstarInvalido()
        {
            //Arrange
            var cliente = _clienteTestsFixture.GerarClienteInvalido();

            //Act
            var result = cliente.EhValido();

            //Assert
            Assert.False(result);
            Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);
        }
    }
}
