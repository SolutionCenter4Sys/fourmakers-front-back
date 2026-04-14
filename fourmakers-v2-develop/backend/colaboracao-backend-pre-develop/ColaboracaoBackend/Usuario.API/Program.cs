using Colaboracao.Initializer.Initializer.Core.Base;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
var initalizer = new Usuario.Application.Initializer();

var app = ApplicationConfigurator.ConfigureApplication(builder, appName, initalizer);

app.Run();