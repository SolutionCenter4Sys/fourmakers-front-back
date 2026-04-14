using Colaboracao.Initializer.Initializer.Core.Base;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
var initializer = new Marketing.Application.Initializer();

var app = ApplicationConfigurator.ConfigureApplication(builder, appName, initializer);

app.Run();
