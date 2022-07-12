namespace Mars.Retranslator.DataModels
{
    public class CommandRecord
    {
        public uint Id { get; set; }

        public uint InstanceId { get; set; }

        public string Type { get; set; }

        public CommandStatus Status { get; set; }
        
        public byte[] Data { get; set; }
        
        public byte[] ResultData { get; set; }

        public DateTimeOffset CreateStamp { get; set; }
    }

    public enum CommandStatus
    {
        New,
        ResolveConfirmed,
        Completed
    }
}