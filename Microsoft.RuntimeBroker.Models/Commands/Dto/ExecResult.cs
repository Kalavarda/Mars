namespace Microsoft.RuntimeBroker.Models.Commands.Dto
{
    public class ExecResult
    {
        public uint CommandId { get; set; }

        public SuccessResultBase Result { get; set; }

        public string Error { get; set; }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public static ExecResult Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
