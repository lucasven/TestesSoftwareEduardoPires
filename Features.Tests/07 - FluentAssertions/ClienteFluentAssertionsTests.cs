using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;

namespace Features.Tests
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteFluentAssertionsTests
    {
        private readonly ClienteTestsBogusFixture _clienteTestsBogusFixture;

        public ClienteFluentAssertionsTests(ClienteTestsBogusFixture clienteTestsBogusFixture)
        {
            _clienteTestsBogusFixture = clienteTestsBogusFixture;
        }

        [Fact(DisplayName = "Novo Cliente Válido")]
        [Trait("Categoria", "Cliente Fluent Assertions Testes")]
        public void Cliente_NovoCliente_DeveEstarValido()
        {
            //Arrange
            var cliente = _clienteTestsBogusFixture.GerarClienteValido();

            //Act
            var result = cliente.EhValido();

            //Assert
            //Assert.True(result);
            //Assert.Equal(0, cliente.ValidationResult.Errors.Count);

            //Assert
            result.Should().BeTrue();
            cliente.ValidationResult.Errors.Count.Should().Be(0);
        }

        [Fact(DisplayName = "Novo Cliente Inválido")]
        [Trait("Categoria", "Cliente Fluent Assertions Testes")]
        public void Cliente_NovoCliente_DeveEstarInvalido()
        {
            //Arrange
            var cliente = _clienteTestsBogusFixture.GerarClienteInvalido();

            //Act
            var result = cliente.EhValido();

            //Assert
            result.Should().BeFalse();
            cliente.ValidationResult.Errors.Count.Should().BeGreaterThan(1, "deve possuir erros de validação");
            Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);
        }
    }
}
