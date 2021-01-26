using Features.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApp.MVC;
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
    [Collection(nameof(IntegrationWebTestsFixtureColletion))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> testsFixture;
        public UsuarioTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            this.testsFixture = testsFixture;
        }
        

        [Fact(DisplayName = "Realizar Cadastro com sucesso"), TestPriority(1)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Arrange
            var initialResponse = await testsFixture.Client.GetAsync("/Identity/Account/Register");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            testsFixture.GerarUserSenha();

            var formData = new Dictionary<string, string>
            {
                {testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", testsFixture.UsuarioEmail },
                {"Input.Password", testsFixture.UsuarioSenha },
                {"Input.ConfirmPassword", testsFixture.UsuarioSenha },
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var postResponse = await testsFixture.Client.SendAsync(postRequest);

            //Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {testsFixture.UsuarioEmail}!", responseString);
        }


        [Fact(DisplayName = "Realizar Cadastro com senha fraca"), TestPriority(3)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarCadastroComSenhaFraca_DeveRetornarMensagemDeErro()
        {
            // Arrange
            var initialResponse = await testsFixture.Client.GetAsync("/Identity/Account/Register");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            testsFixture.GerarUserSenha();
            const string senhaFraca = "123456";

            var formData = new Dictionary<string, string>
            {
                {testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", testsFixture.UsuarioEmail },
                {"Input.Password", senhaFraca },
                {"Input.ConfirmPassword", senhaFraca },
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var postResponse = await testsFixture.Client.SendAsync(postRequest);

            //Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains("Passwords must have at least one non alphanumeric character.", responseString);
            Assert.Contains("Passwords must have at least one lowercase (&#x27;a&#x27;-&#x27;z&#x27;).", responseString);
            Assert.Contains("Passwords must have at least one uppercase (&#x27;A&#x27;-&#x27;Z&#x27;).", responseString);
        }

        [Fact(DisplayName = "Realizar Login com sucesso"), TestPriority(2)]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async Task Usuario_RealizarLogin_DeveExecutarComSucesso()
        {
            // Arrange
            var initialResponse = await testsFixture.Client.GetAsync("/Identity/Account/Login");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = testsFixture.ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {testsFixture.AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", testsFixture.UsuarioEmail },
                {"Input.Password", testsFixture.UsuarioSenha },
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var postResponse = await testsFixture.Client.SendAsync(postRequest);

            //Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();


            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {testsFixture.UsuarioEmail}", responseString);
        }
    }
}
