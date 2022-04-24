using Microsoft.RuntimeBroker;
using Microsoft.RuntimeBroker.InternalServices;
using Microsoft.RuntimeBroker.InternalServices.Impl;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ICommandQueue, CommandQueue>();
        services.AddSingleton<IResultQueue, ResultQueue>();
        services.AddSingleton<ICommandExecutor, CommandExecutor>();
        services.AddSingleton<ICommandReceiver, CommandReceiver>();
        services.AddSingleton<IResultSender, ResultSender>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
