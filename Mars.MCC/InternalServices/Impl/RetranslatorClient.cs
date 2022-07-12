using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Kalantyr.Web.Impl;
using Mars.Retranslator.DataModels;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.MCC.InternalServices.Impl
{
    internal class RetranslatorClient: HttpClientBase, IRetranslatorClient
    {
        private readonly AppKeyRequestEnricher _appKeyEnricher;

        public RetranslatorClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory, new AppKeyRequestEnricher())
        {
            _appKeyEnricher = (AppKeyRequestEnricher)RequestEnricher;
        }

        public async Task<ResultDto<IReadOnlyCollection<Instance>>> GetInstancesAsync(CancellationToken cancellationToken)
        {
            _appKeyEnricher.AppKey = Settings.Default.ApiKey;
            
            var response = await Get<ResultDto<Instance[]>>("mcc/instances", cancellationToken);
            if (response.Error != null)
                return new ResultDto<IReadOnlyCollection<Instance>> { Error = response.Error };

            return new ResultDto<IReadOnlyCollection<Instance>>
            {
                Result = response.Result
            };
        }

        public Task<ResultDto<bool>> SendCommandAsync(CommandBase command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultDto<IReadOnlyCollection<CommandRecord>>> GetCommandsAsync(uint instanceId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
        {
            _appKeyEnricher.AppKey = Settings.Default.ApiKey;

            var response = await Get<ResultDto<CommandRecord[]>>($"mcc/commands?instId={instanceId}", cancellationToken);
            if (response.Error != null)
                return new ResultDto<IReadOnlyCollection<CommandRecord>> { Error = response.Error };

            return new ResultDto<IReadOnlyCollection<CommandRecord>>
            {
                Result = response.Result
            };
        }

        public async Task<ResultDto<bool>> AddCommandsAsync(uint instanceId, CommandBase command, CancellationToken cancellationToken)
        {
            _appKeyEnricher.AppKey = Settings.Default.ApiKey;

            var body = JsonSerializer.Serialize(Convert.ToBase64String(command.Serialize()));
            var response = await Post<ResultDto<bool>>($"mcc/add?instId={instanceId}", body, cancellationToken);
            if (response.Error != null)
                return new ResultDto<bool> { Error = response.Error };

            return ResultDto<bool>.Ok;
        }
    }
}
