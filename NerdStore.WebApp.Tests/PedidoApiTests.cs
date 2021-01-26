using Features.Tests;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.MVC.Models;
using NerdStore.WebApp.Tests.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [TestCaseOrderer("Features.Tests.PriorityOrder", "Features.Tests")]
    [Collection(nameof(IntegrationApiTestsFixtureColletion))]
    public class PedidoApiTests
    {
        private readonly IntegrationTestsFixture<StartupApiTests> testsFixture;

        public PedidoApiTests(IntegrationTestsFixture<StartupApiTests> testsFixture)
        {
            this.testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Adicionar item em novo pedido"), TestPriority(1)]
        [Trait("Categoria", "Integração API - Pedido")]
        public async Task AdicionarItem_NovoPedido_DeveRetornarComSucesso()
        {
            //Arrange
            var itemInfo = new ItemViewModel
            {
                Id = new Guid("191ddd3e-acd4-4c3b-ae74-8e473993c5da"),
                Quantidade = 2
            };

            await testsFixture.RealizarLoginApi();
            testsFixture.Client.AtribuirToken(testsFixture.UsuarioToken);

            //Act
            var postResponse = await testsFixture.Client.PostAsJsonAsync("api/carrinho", itemInfo);

            //assert
            postResponse.EnsureSuccessStatusCode();
        }

        [Fact(DisplayName = "Remover item em pedido existente"), TestPriority(2)]
        [Trait("Categoria", "Integracao API - Pedido")]
        public async Task RemoverItem_PedidoExistente_DeveRetornarComSucesso()
        {
            //Arrange
            var produtoId = new Guid("191ddd3e-acd4-4c3b-ae74-8e473993c5da");

            await testsFixture.RealizarLoginApi();
            testsFixture.Client.AtribuirToken(testsFixture.UsuarioToken);

            //act
            var deleteResponse = await testsFixture.Client.DeleteAsync($"api/carrinho/{produtoId}");

            //assert
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}
