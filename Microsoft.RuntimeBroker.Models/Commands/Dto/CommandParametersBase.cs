namespace Microsoft.RuntimeBroker.Models.Commands.Dto
{
    public abstract class CommandParametersBase
    {
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(GetType().FullName);
            SerializeImpl(writer);
        }

        public static CommandParametersBase Deserialize(BinaryReader reader)
        {
            var type = Type.GetType(reader.ReadString());
            var parameters = (CommandParametersBase)Activator.CreateInstance(type);
            parameters.DeserializeImpl(reader);
            return parameters;
        }

        protected virtual void SerializeImpl(BinaryWriter writer)
        {

        }

        protected virtual void DeserializeImpl(BinaryReader reader)
        {

        }
    }
}
