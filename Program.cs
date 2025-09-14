using mediateur;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Enregistrement d’un service
// builder.Services.AddTransient<IGreeter, BasicGreeter>();
// builder.Services.AddTransient<IGreeter, ObsequiousGreeter>();
builder.Services.RegisterNotificationHandlers<IGreeter>();
builder.Services.RegisterRequestHandlers<IMyRequestHandler>();
var app = builder.Build();

// Récupération et utilisation du service
//var greeter = app.Services.GetRequiredService<IGreeter>();
var greeter = app.Services.GetService<IGreeter>();
greeter.Greet("Olivier");

var myRequestHandler = app.Services.GetService<IMyRequestHandler>();
Console.WriteLine("3+4="+myRequestHandler.Sum(3, 4));

