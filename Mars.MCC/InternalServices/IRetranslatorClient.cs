using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Mars.Retranslator.DataModels;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.MCC.InternalServices
{
    public interface IRetranslatorClient
    {
        Task<ResultDto<IReadOnlyCollection<Instance>>> GetInstancesAsync(CancellationToken cancellationToken);

        Task<ResultDto<bool>> SendCommandAsync(CommandBase command, CancellationToken cancellationToken);

        Task<ResultDto<IReadOnlyCollection<CommandRecord>>> GetCommandsAsync(uint instanceId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);

        Task<ResultDto<bool>> AddCommandsAsync(uint instanceId, CommandBase command, CancellationToken cancellationToken);
    }
}
