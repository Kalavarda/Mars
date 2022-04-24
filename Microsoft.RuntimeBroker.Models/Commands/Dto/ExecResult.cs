namespace Microsoft.RuntimeBroker.Models.Commands.Dto
{
    public class ExecResult
    {
        public uint CommandId { get; set; }

        public SuccessResultBase Result { get; set; }

        public string Error { get; set; }

        public byte[] Serialize()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(CommandId);

            if (Result != null)
            {
                writer.Write(true);

                var typeName = Result.GetType().FullName;
                writer.Write(typeName);
                
                Result.Serialize(writer);
            }
            else
            {
                writer.Write(false);
                writer.Write(Error);
            }

            writer.Flush();
            return stream.ToArray();
        }

        public static ExecResult Deserialize(byte[] data)
        {
            var execResult = new ExecResult();

            using var stream = new MemoryStream(data);
            using var reader = new BinaryReader(stream);

            execResult.CommandId = reader.ReadUInt32();

            if (reader.ReadBoolean())
            {
                var type = Type.GetType(reader.ReadString());
                execResult.Result = (SuccessResultBase)Activator.CreateInstance(type);
                
                execResult.Result.Deserialize(reader);
            }
            else
                execResult.Error = reader.ReadString();

            return execResult;
        }
    }
}
