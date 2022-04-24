using Microsoft.RuntimeBroker.Models.Commands.Dto;
using NUnit.Framework;

namespace Microsoft.RuntimeBroker.Tests
{
    public class ExecResult_Tests
    {
        [Test]
        public void SerializeDeserialize_Test()
        {
            var result = new ExecResult
            {
                CommandId = 123,
                Result = new GetMachineNameResult { MachineName = "name1" }
            };
            var data = result.Serialize();
            var result2 = ExecResult.Deserialize(data);
            Assert.AreEqual("name1", ((GetMachineNameResult)result2.Result).MachineName);
        }
    }
}
