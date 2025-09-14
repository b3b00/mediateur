

using external;
using interfaces;
using mediateur;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mediateurtest;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = Host.CreateApplicationBuilder(args);

        var t = typeof(Plan9Greeter);
        
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
        greeter.SayBye("Olivier");

        var myRequestHandler = app.Services.GetService<IMyRequestHandler>();
        Console.WriteLine("3+4=" + myRequestHandler.Sum(3, 4));
    }
}
