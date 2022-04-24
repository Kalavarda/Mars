namespace Microsoft.RuntimeBroker.Models.Commands.Dto
{
    public class GetMachineNameResult: SuccessResultBase
    {
        public string MachineName { get; set; }
        
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(MachineName);
        }

        public override void Deserialize(BinaryReader reader)
        {
            MachineName = reader.ReadString();
        }
    }
}
