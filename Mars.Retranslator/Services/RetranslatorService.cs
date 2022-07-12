using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Mars.Retranslator.Config;
using Mars.Retranslator.DataModels;
using Microsoft.Extensions.Options;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

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
            var instance = await _commandRepository.GetInstanceAsync(machineName, cancellationToken);
            var record = await _commandRepository.GetNextAsync(instance.Id, cancellationToken);

            if (record == null)
                return NoCommand;

            var command = Map(record);
            return new ResultDto<byte[]> { Result = command.Serialize() };
        }

        public async Task ConfirmResolveAsync(uint commandId, string machineName, CancellationToken cancellationToken)
        {
            var record = await _commandRepository.GetAsync(commandId, cancellationToken);
            record.Status = CommandStatus.ResolveConfirmed;
            await _commandRepository.UpdateAsync(record, cancellationToken);
        }

        public async Task<ResultDto<bool>> SubmitExecResultAsync(uint commandId, string machineName, byte[] data, CancellationToken cancellationToken)
        {
            var record = await _commandRepository.GetAsync(commandId, cancellationToken);
            record.Status = CommandStatus.Completed;
            record.ResultData = data;
            await _commandRepository.UpdateAsync(record, cancellationToken);

            Debug.WriteLine($"Команда {commandId} выполнена");
            return ResultDto<bool>.Ok;
        }

        private static CommandBase Map(CommandRecord record)
        {
            if (record == null)
                return null;

            return CommandBase.Deserialize(record.Data);
        }

        public async Task<ResultDto<IReadOnlyCollection<Instance>>> GetInstancesAsync(string mccKey, CancellationToken cancellationToken)
        {
            if (_config.MccKey != mccKey)
                return new ResultDto<IReadOnlyCollection<Instance>> { Error = Errors.AccessDenied };

            var instances = await _commandRepository.GetInstancesAsync(cancellationToken);
            return new ResultDto<IReadOnlyCollection<Instance>>
            {
                Result = instances
            };
        }

        public async Task<ResultDto<IReadOnlyCollection<CommandRecord>>> GetCommandsAsync(uint instanceId, DateTimeOffset startDate, DateTimeOffset endDate, string mccKey, CancellationToken cancellationToken)
        {
            if (_config.MccKey != mccKey)
                return new ResultDto<IReadOnlyCollection<CommandRecord>> { Error = Errors.AccessDenied };

            var records = await _commandRepository.GetRecordsAsync(instanceId, startDate, endDate, cancellationToken);
            return new ResultDto<IReadOnlyCollection<CommandRecord>> { Result = records };
        }

        public async Task<ResultDto<bool>> AddCommandsAsync(uint instanceId, byte[] data, string mccKey, CancellationToken cancellationToken)
        {
            if (_config.MccKey != mccKey)
                return new ResultDto<bool> { Error = Errors.AccessDenied };

            var command = CommandBase.Deserialize(data);

            var record = new CommandRecord
            {
                CreateStamp = DateTimeOffset.Now,
                Data = data,
                InstanceId = instanceId,
                Status = CommandStatus.New,
                Type = command.GetType().Name
            };
            await _commandRepository.AddAsync(record, cancellationToken);

            return ResultDto<bool>.Ok;
        }
    }
}
