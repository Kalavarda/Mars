using Microsoft.Extensions.Options;
using Microsoft.RuntimeBroker;
using Microsoft.RuntimeBroker.Config;
using Microsoft.RuntimeBroker.InternalServices;
using Microsoft.RuntimeBroker.InternalServices.Impl;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(Configure)
    .Build();

await host.RunAsync();

void Configure(HostBuilderContext context, IServiceCollection serviceCollection)
{
    serviceCollection.Configure<ServiceConfig>(context.Configuration.GetSection("Service"));

    serviceCollection.AddSingleton<ICommandRepository, CommandRepository>();
    serviceCollection.AddSingleton<ICommandExecutor, CommandExecutor>();
    serviceCollection.AddSingleton<ICommandReceiver, CommandReceiver>();
    serviceCollection.AddSingleton<IResultSender, ResultSender>();
    serviceCollection.AddHttpClient<CommandHttpClient>((sp, client) =>
    {
        client.BaseAddress = new Uri(sp.GetService<IOptions<ServiceConfig>>().Value.RetranslatorUrl);
    });
    serviceCollection.AddHostedService<Worker>();
}
