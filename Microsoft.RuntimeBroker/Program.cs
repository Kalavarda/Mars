using Kalantyr.Web;
using Microsoft.RuntimeBroker;
using Microsoft.RuntimeBroker.InternalServices;
using Microsoft.RuntimeBroker.InternalServices.Impl;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(Configure)
    .Build();

await host.RunAsync();

void Configure(IServiceCollection serviceCollection)
{
    serviceCollection.AddSingleton<IRequestEnricher, CommandRequestEnricher>();
    serviceCollection.AddSingleton<ICommandRepository, CommandRepository>();
    serviceCollection.AddSingleton<ICommandExecutor, CommandExecutor>();
    serviceCollection.AddSingleton<ICommandReceiver, CommandReceiver>();
    serviceCollection.AddSingleton<IResultSender, ResultSender>();
    serviceCollection.AddHttpClient<CommandHttpClient>(q => q.BaseAddress = new Uri("http://localhost:63426"));
    serviceCollection.AddHostedService<Worker>();
}
