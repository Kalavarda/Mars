using Kalantyr.Web;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandReceiver: ICommandReceiver
    {
        private readonly ICommandQueue _queue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestEnricher _requestEnricher;

        public CommandReceiver(ICommandQueue queue, IHttpClientFactory httpClientFactory, IRequestEnricher requestEnricher)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _requestEnricher = requestEnricher ?? throw new ArgumentNullException(nameof(requestEnricher));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var commandHttpClient = new CommandHttpClient(_httpClientFactory, _requestEnricher);
            var command = await commandHttpClient.ResolveCommandAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await _queue.AddAsync(command, cancellationToken);
        }
    }
}
