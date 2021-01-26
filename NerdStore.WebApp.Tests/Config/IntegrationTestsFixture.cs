using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.WebApp.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationWebTestsFixtureColletion))]
    public class IntegrationWebTestsFixtureColletion : ICollectionFixture<IntegrationTestsFixture<StartupWebTests>> { }

    [CollectionDefinition(nameof(IntegrationApiTestsFixtureColletion))]
    public class IntegrationApiTestsFixtureColletion : ICollectionFixture<IntegrationTestsFixture<StartupApiTests>> { }

    public class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        public string AntiForgeryFieldName = "__RequestVerificationToken";

        public string UsuarioEmail;
        public string UsuarioSenha;

        public string UsuarioToken;

        public readonly LojaAppFactory<TStartup> Factory;
        public HttpClient Client;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
                HandleCookies = true,
                BaseAddress = new Uri("http://localhost"),
                MaxAutomaticRedirections = 7
            };

            Factory = new LojaAppFactory<TStartup>();
            Client = Factory.CreateClient();
        }

        public void GerarUserSenha()
        {
            var faker = new Faker("pt_BR");
            UsuarioEmail = faker.Internet.Email().ToLower();
            UsuarioSenha = faker.Internet.Password(8, false, "", "@1Ab_");
        }

        public async Task RealizarLoginApi()
        {
            var itemInfo = new LoginViewModel
            {
                Email = "teste3@teste.com",
                Senha = "Teste@123"
            };

            //recriando o client para evitar configuracoes de web
            Client = Factory.CreateClient();

            var postResponse = await Client.PostAsJsonAsync("api/login", itemInfo);
            postResponse.EnsureSuccessStatusCode();

            UsuarioToken = await postResponse.Content.ReadAsStringAsync();
        }

        public async Task RealizarLoginWeb()
        {
            var initialResponse = await Client.GetAsync("Identity/Account/Login");
            initialResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = ObterAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {AntiForgeryFieldName, antiForgeryToken },
                {"Input.Email", "teste3@teste.com" },
                {"Input.Password", "Teste@123" },
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            //Act
            var postResponse = await Client.SendAsync(postRequest);
        }

        public string ObterAntiForgeryToken(string htmlBody)
        {
            var requestVerificationToken = Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if(requestVerificationToken.Success)
            {
                return requestVerificationToken.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti Forgery token '{AntiForgeryFieldName}' não encontrado no HTML", nameof(htmlBody));
        }

        public void Dispose()
        {
        }
    }
}
