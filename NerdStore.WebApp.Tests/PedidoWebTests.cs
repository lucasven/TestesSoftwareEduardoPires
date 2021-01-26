using AngleSharp.Html.Parser;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureColletion))]
    public class PedidoWebTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> testsFixture;

        public PedidoWebTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            this.testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Adicionar Item em novo pedido")]
        [Trait("Categoria", "Integração Web - Pedido")]
        public async Task AdicionarItem_NovoPedido_DeveAtualizarValorTotal()
        {
            // Arrange
            var produtoId = new Guid("191ddd3e-acd4-4c3b-ae74-8e473993c5da");
            const int quantidade = 2;

            var initialResponse = await testsFixture.Client.GetAsync($"/produto-detalhe/{produtoId}");
            initialResponse.EnsureSuccessStatusCode();

            var formData = new Dictionary<string, string>
            {
                {"Id", produtoId.ToString()},
                {"quantidade", quantidade.ToString()}
            };

            await testsFixture.RealizarLoginWeb();

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/meu-carrinho")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await testsFixture.Client.SendAsync(postRequest);

            // Assert
            postResponse.EnsureSuccessStatusCode();

            var html = new HtmlParser()
                .ParseDocumentAsync(await postResponse.Content.ReadAsStringAsync())
                .Result
                .All;

            var formQuantidade = html?.FirstOrDefault(c => c.Id == "quantidade")?.GetAttribute("value")?.ApenasNumeros();
            var valorUnitario = html?.FirstOrDefault(c => c.Id == "valorUnitario")?.TextContent.Split(".")[0]?.ApenasNumeros();
            var valorTotal = html?.FirstOrDefault(c => c.Id == "valorTotal")?.TextContent.Split(".")[0]?.ApenasNumeros();

            Assert.Equal(valorTotal, valorUnitario * formQuantidade);
        }
    }
}
