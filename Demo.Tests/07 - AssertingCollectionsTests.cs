using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Demo.Tests
{
    public class AssertingCollectionsTests
    {
        [Fact]
        public void Funcionario_Habilidades_NaoDevePossuirHabilidadeVazias()
        {
            // Arrange & Act
            var funcionario = FuncionarioFactory.Criar("Eduardo", 10000);

            // Assert 
            Assert.All(funcionario.Habilidades, habilidade => Assert.False(string.IsNullOrWhiteSpace(habilidade)));
        }

        [Fact]
        public void Funcionario_Habilidades_JuniorDevePossuirHabilidadeBasica()
        {
            // Arrange & Act
            var funcionario = FuncionarioFactory.Criar("Eduardo", 10000);

            // Assert 
            Assert.Contains("OOP", funcionario.Habilidades);
        }

        [Fact]
        public void Funcionario_Habilidades_JuniorNaoDevePossuirHabilidadeBasica()
        {
            // Arrange & Act
            var funcionario = FuncionarioFactory.Criar("Eduardo", 2500);

            // Assert 
            Assert.DoesNotContain("Microserviços", funcionario.Habilidades);
        }

        [Fact]
        public void Funcionario_Habilidades_SeniorDevePossuirTodasHabilidades()
        {
            // Arrange & Act
            var funcionario = FuncionarioFactory.Criar("Eduardo", 10000);

            var habilidadesBasicas = new[]
            {
                "Lógica de Programação",
                "OOP",
                "Testes",
                "Microserviços"
            };

            // Assert 
            Assert.Equal(habilidadesBasicas, funcionario.Habilidades);
        }
    }
}
