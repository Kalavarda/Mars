namespace Microsoft.RuntimeBroker.Models.Commands.Dto
{
    public abstract class SuccessResultBase
    {
        public abstract void Serialize(BinaryWriter writer);

        public abstract void Deserialize(BinaryReader reader);
    }
}
