using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Mars.Retranslator.Config;
using Microsoft.Extensions.Options;
using Microsoft.RuntimeBroker.Models.Commands;
using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Mars.Retranslator.Services
{
    public class RetranslatorService
    {
        private readonly ICommandRepository _commandRepository;
        private static readonly ResultDto<byte[]> NoCommand = new() { Result = null };

        private readonly ServiceConfig _config;

        public RetranslatorService(IOptions<ServiceConfig> config, ICommandRepository commandRepository)
        {
            _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
            _config = config.Value;
        }

        public async Task<ResultDto<byte[]>> ResolveCommandAsync(CancellationToken cancellationToken)
        {
            var command = new GetSystemInfoCommand
            {
                Id = 321,
                CommandParameters = new SystemInfoCommandParameters
                {
                    MachineName = true
                }
            };

            if (command != null)
                return new ResultDto<byte[]> { Result = command.Serialize() };
            else
                return NoCommand;
        }

        public async Task<ResultDto<bool>> ReceiveCommandAsync(uint commandId, byte[] data, CancellationToken cancellationToken)
        {
            var command = await _commandRepository.GetAsync(commandId, cancellationToken);
            var execResult = ExecResult.Deserialize(data);

            return ResultDto<bool>.Ok;
        }
    }
}
