using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Mars.Retranslator.Config;
using Mars.Retranslator.DataModels;
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

        public async Task<ResultDto<byte[]>> ResolveCommandAsync(string machineName, CancellationToken cancellationToken)
        {
            var record = await _commandRepository.GetNextAsync(machineName, cancellationToken);

            if (record == null)
                return NoCommand;

            var command = Map(record);
            return new ResultDto<byte[]> { Result = command.Serialize() };
        }

        public async Task<ResultDto<bool>> SubmitExecResultAsync(uint commandId, string machineName, byte[] data, CancellationToken cancellationToken)
        {
            var command = await _commandRepository.GetAsync(commandId, cancellationToken);
            var execResult = ExecResult.Deserialize(data);

            return ResultDto<bool>.Ok;
        }

        public async Task ConfirmResolveAsync(uint commandId, string machineName, CancellationToken cancellationToken)
        {
            var record = await _commandRepository.GetAsync(commandId, cancellationToken);
            record.Status = CommandStatus.ResolveConfirmed;
            await _commandRepository.StoreAsync(record);
            Debug.WriteLine($"Команда {commandId} выполнена");
        }

        private static CommandBase Map(CommandRecord record)
        {
            if (record == null)
                return null;

            return CommandBase.Deserialize(record.Data);
        }
    }
}
