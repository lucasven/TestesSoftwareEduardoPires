using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace Features.Tests
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteFluentAssertionsTests
    {
        private readonly ClienteTestsBogusFixture _clienteTestsBogusFixture;
        readonly ITestOutputHelper _outputHelper;

        public ClienteFluentAssertionsTests(ClienteTestsBogusFixture clienteTestsBogusFixture
            ,ITestOutputHelper testOutputHelper)
        {
            _clienteTestsBogusFixture = clienteTestsBogusFixture;
            _outputHelper = testOutputHelper;
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
            //Assert.NotEqual(0, cliente.ValidationResult.Errors.Count);
            result.Should().BeFalse();
            cliente.ValidationResult.Errors.Count.Should().BeGreaterThan(1, "deve possuir erros de validação");
            _outputHelper.WriteLine($"Foram encontrados {cliente.ValidationResult.Errors.Count} erros nessa validacao");
        }
    }
}
