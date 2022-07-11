using Kalantyr.Web;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandReceiver: ICommandReceiver
    {
        private readonly ICommandRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestEnricher _requestEnricher;

        public CommandReceiver(ICommandRepository repository, IHttpClientFactory httpClientFactory, IRequestEnricher requestEnricher)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _requestEnricher = requestEnricher ?? throw new ArgumentNullException(nameof(requestEnricher));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var commandHttpClient = new CommandHttpClient(_httpClientFactory, _requestEnricher);

            var command = await commandHttpClient.ResolveCommandAsync(cancellationToken);
            if (command == null)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            await _repository.AddAsync(command, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            await commandHttpClient.ConfirmResolveAsync(command.Id, cancellationToken);
        }
    }
}
