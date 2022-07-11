using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Kalantyr.Web.Impl;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.MCC.InternalServices.Impl
{
    internal class RetranslatorClient: HttpClientBase, IRetranslatorClient
    {
        private AppKeyRequestEnricher _appKeyEnricher;

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
            throw new System.NotImplementedException();
        }
    }
}
