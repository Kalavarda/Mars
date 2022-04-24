using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class ResultSender: IResultSender
    {
        private readonly IResultQueue _resultQueue;

        public ResultSender(IResultQueue resultQueue)
        {
            _resultQueue = resultQueue ?? throw new ArgumentNullException(nameof(resultQueue));
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
            throw new NotImplementedException();
        }
    }
}
