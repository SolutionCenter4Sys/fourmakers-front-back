using System.Globalization;
using Colaboracao.Initializer.Initializer.Core.Base;

var cultureInfo = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
var initalizer = new Financeiro.Application.Initializer();

var app = ApplicationConfigurator.ConfigureApplication(builder, appName, initalizer);

app.Run();