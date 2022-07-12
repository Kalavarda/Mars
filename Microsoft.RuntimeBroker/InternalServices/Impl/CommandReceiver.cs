namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandReceiver: ICommandReceiver
    {
        private readonly ICommandRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;

        public CommandReceiver(ICommandRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var commandHttpClient = new CommandHttpClient(_httpClientFactory);

            var commands = await commandHttpClient.ResolveCommandsAsync(cancellationToken);
            foreach (var command in commands)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _repository.AddAsync(command, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await commandHttpClient.ConfirmResolveAsync(command.Id, cancellationToken);
            }
        }
    }
}
