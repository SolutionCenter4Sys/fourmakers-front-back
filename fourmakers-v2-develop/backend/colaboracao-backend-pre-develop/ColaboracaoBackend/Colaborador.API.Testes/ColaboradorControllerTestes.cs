namespace Colaborador.API.Testes
{
    public class ColaboradorControllerTestes
    {
        //    [Theory]
        //    [InlineData("38477992827", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    [InlineData("03772351131", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    public async System.Threading.Tasks.Task BuscarNomeColaboradorTeste(string cpf, string tokenUsuario)
        //    {
        //        IServiceCollection services = new ServiceCollection();

        //        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //        services.AddSingleton<IConfiguration>(configuration);
        //        var target = new Startup(configuration);
        //        target.ConfigureServices(services);
        //        services.AddTransient<ColaboradorController>();
        //        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        //        var context = new DefaultHttpContext();
        //        var claims = new[] {
        //                new Claim("Token", tokenUsuario)
        //            };
        //        var identity = new ClaimsIdentity(claims, "ClaimsName");
        //        context.User = new ClaimsPrincipal(identity);
        //        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        //        services.AddSingleton<IHttpContextAccessor>(mockHttpContextAccessor.Object);
        //        var serviceProvider = services.BuildServiceProvider();
        //        var controller = serviceProvider.GetService<ColaboradorController>();

        //        var retornov1 = controller.BuscarNomeColaborador(cpf);

        //        var _apiClient = new ApliClient.Infra.Impl.ApiClient(new Mock<ILogger<ApliClient.Infra.Impl.ApiClient>>().Object);
        //        byte[] textoAsBytes = Encoding.ASCII.GetBytes(tokenUsuario);
        //        tokenUsuario = System.Convert.ToBase64String(textoAsBytes);
        //        var headers = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>
        //                (
        //                    "Authorization",
        //                    String.Format("Basic {0}", tokenUsuario)
        //                )
        //            };
        //        var responseMessage = await _apiClient.GetAsync<NomeColaboradorResult>("https://colab-api.app.foursys.com/ColaboradorHML/api/Colaborador/BuscarNomeColaborador?cpfColaborador=" + cpf, headers);
        //        var retornov2 = responseMessage.Resposta;

        //        Assert.Equal(retornov2.Colaborador.NomeCompleto, retornov1.Colaborador.NomeCompleto);
        //    }

        //    [Theory]
        //    [InlineData("38477992827", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    [InlineData("03772351131", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    public async System.Threading.Tasks.Task BuscarColaboradorTeste(string cpf, string tokenUsuario)
        //    {
        //        IServiceCollection services = new ServiceCollection();

        //        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //        services.AddSingleton<IConfiguration>(configuration);
        //        var target = new Startup(configuration);
        //        target.ConfigureServices(services);
        //        services.AddTransient<ColaboradorController>();
        //        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        //        var context = new DefaultHttpContext();
        //        var claims = new[] {
        //                new Claim("Token", tokenUsuario)
        //            };
        //        var identity = new ClaimsIdentity(claims, "ClaimsName");
        //        context.User = new ClaimsPrincipal(identity);
        //        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        //        services.AddSingleton<IHttpContextAccessor>(mockHttpContextAccessor.Object);
        //        var serviceProvider = services.BuildServiceProvider();
        //        var controller = serviceProvider.GetService<ColaboradorController>();

        //        var retornov1 = controller.BuscarColaborador(cpf, "Jav", 0, 10);

        //        var jsonRetornoV1 = JsonSerializer.Serialize(retornov1);

        //        var _apiClient = new ApliClient.Infra.Impl.ApiClient(new Mock<ILogger<ApliClient.Infra.Impl.ApiClient>>().Object);
        //        byte[] textoAsBytes = Encoding.ASCII.GetBytes(tokenUsuario);
        //        tokenUsuario = System.Convert.ToBase64String(textoAsBytes);
        //        var headers = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>
        //                (
        //                    "Authorization",
        //                    String.Format("Basic {0}", tokenUsuario)
        //                )
        //            };
        //        var responseMessage = await _apiClient.GetAsync<BuscaColaboradorResult>("https://colab-api.app.foursys.com/ColaboradorHML/api/Colaborador/BuscarColaborador?cpf=" + cpf + "&cursor=0&limite=10&busca=Jav", headers);
        //        var retornov2 = responseMessage.Resposta;

        //        var jsonRetornoV2 = JsonSerializer.Serialize(retornov2);

        //        Assert.Equal(jsonRetornoV2, jsonRetornoV1);
        //    }

        //    [Theory]
        //    [InlineData("38477992827", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    [InlineData("03772351131", "BSaQToJxqA+ZfkED5diJjr8Mje80kSpntQj6Ec62mDnFae4XXVulooNRt0Ro58rd")]
        //    public async System.Threading.Tasks.Task BuscarCandidatoTeste(string cpf, string tokenUsuario)
        //    {
        //        IServiceCollection services = new ServiceCollection();

        //        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //        services.AddSingleton<IConfiguration>(configuration);
        //        var target = new Startup(configuration);
        //        target.ConfigureServices(services);
        //        services.AddTransient<ColaboradorController>();
        //        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        //        var context = new DefaultHttpContext();
        //        var claims = new[] {
        //                new Claim("Token", tokenUsuario)
        //            };
        //        var identity = new ClaimsIdentity(claims, "ClaimsName");
        //        context.User = new ClaimsPrincipal(identity);
        //        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        //        services.AddSingleton<IHttpContextAccessor>(mockHttpContextAccessor.Object);
        //        var serviceProvider = services.BuildServiceProvider();
        //        var controller = serviceProvider.GetService<ColaboradorController>();

        //        var retornov1 = controller.BuscarCandidato(cpf, "Jav", 0, 10);

        //        var _apiClient = new ApliClient.Infra.Impl.ApiClient(new Mock<ILogger<ApliClient.Infra.Impl.ApiClient>>().Object);
        //        byte[] textoAsBytes = Encoding.ASCII.GetBytes(tokenUsuario);
        //        tokenUsuario = System.Convert.ToBase64String(textoAsBytes);
        //        var headers = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>
        //                (
        //                    "Authorization",
        //                    String.Format("Basic {0}", tokenUsuario)
        //                )
        //            };
        //        var responseMessage = await _apiClient.GetAsync<BuscaColaboradorResult>("https://colab-api.app.foursys.com/ColaboradorHML/api/Colaborador/BuscarCandidato?cpf=" + cpf + "&cursor=0&limite=10&busca=Jav", headers);
        //        var retornov2 = responseMessage.Resposta;

        //        //Assert.Equal(retornov2, retornov1);
        //    }
    }
}