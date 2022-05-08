using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.Models.Commands
{
    public abstract class CommandBase
    {
        public uint Id { get; set; }

        public CommandParametersBase CommandParameters { get; set; }

        public abstract Task<SuccessResultBase> ExecuteAsync(CancellationToken cancellationToken);

        public byte[] Serialize()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(GetType().FullName);
            writer.Write(Id);
            CommandParameters?.Serialize(writer);

            writer.Flush();
            return stream.ToArray();
        }

        public static CommandBase Deserialize(byte[] data)
        {
            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            var type = Type.GetType(reader.ReadString());
            var command = (CommandBase)Activator.CreateInstance(type);
            command.Id = reader.ReadUInt32();
            command.CommandParameters = CommandParametersBase.Deserialize(reader);

            return command;
        }
    }
}