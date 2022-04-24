using Microsoft.RuntimeBroker.Models.Commands;
using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class ResultQueue: IResultQueue
    {
        private readonly ICollection<ExecResult> _results = new List<ExecResult>();

        public Task AddAsync(CommandBase command, SuccessResultBase execResult, Exception error)
        {
            _results.Add(new ExecResult
            {
                CommandId = command.Id,
                Result = execResult
            });
            return Task.CompletedTask;
        }

        public Task<ExecResult> GetAsync(CancellationToken cancellationToken)
        {
            if (_results.Count == 0)
                return Task.FromResult<ExecResult>(null);

            return Task.FromResult(_results.First());
        }

        public Task RemoveAsync(ExecResult result, CancellationToken cancellationToken)
        {
            _results.Remove(result);
            return Task.CompletedTask;
        }
    }
}
