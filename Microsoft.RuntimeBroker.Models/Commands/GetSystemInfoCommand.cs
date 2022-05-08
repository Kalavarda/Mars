using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.Models.Commands
{
    public class GetSystemInfoCommand: CommandBase
    {
        public SystemInfoCommandParameters Parameters => (SystemInfoCommandParameters)CommandParameters;

        public override Task<SuccessResultBase> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new GetMachineNameResult();
            if (Parameters.MachineName)
                result.MachineName = Environment.MachineName;
            return Task.FromResult<SuccessResultBase>(result);
        }
    }

    public class SystemInfoCommandParameters : CommandParametersBase
    {
        public bool MachineName { get; set; }

        protected override void SerializeImpl(BinaryWriter writer)
        {
            base.SerializeImpl(writer);
            writer.Write(MachineName);
        }

        protected override void DeserializeImpl(BinaryReader reader)
        {
            base.DeserializeImpl(reader);
            MachineName = reader.ReadBoolean();
        }
    }
}
