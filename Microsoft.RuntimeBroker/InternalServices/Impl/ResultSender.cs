using Kalantyr.Web;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class ResultSender: IResultSender
    {
        private readonly IResultQueue _resultQueue;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestEnricher _requestEnricher;

        public ResultSender(IResultQueue resultQueue, IHttpClientFactory httpClientFactory, IRequestEnricher requestEnricher)
        {
            _resultQueue = resultQueue ?? throw new ArgumentNullException(nameof(resultQueue));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _requestEnricher = requestEnricher ?? throw new ArgumentNullException(nameof(requestEnricher));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var result = await _resultQueue.GetAsync(cancellationToken);
            if (result == null)
                return;

            cancellationToken.ThrowIfCancellationRequested();

            var data = result.Serialize();
            try
            {
                await SendAsync(result.CommandId, data, cancellationToken);
                await _resultQueue.RemoveAsync(result, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task SendAsync(uint commandId, byte[] data, CancellationToken cancellationToken)
        {
            var commandHttpClient = new CommandHttpClient(_httpClientFactory, _requestEnricher);
            var res = await commandHttpClient.SendCommandAsync(commandId, data, cancellationToken);
            if (res.Error != null)
                throw new Exception(res.Error.Code);
        }
    }
}
