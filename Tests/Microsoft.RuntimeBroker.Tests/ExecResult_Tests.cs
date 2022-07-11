using Microsoft.RuntimeBroker.Models.Commands;
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
                Result = new GetSystemInfoResult { MachineName = "name1" }
            };
            
            var data = result.Serialize();
            var result2 = ExecResult.Deserialize(data);

            Assert.AreEqual(123, result2.CommandId);
            Assert.AreEqual("name1", ((GetSystemInfoResult)result2.Result).MachineName);
        }

        [Test]
        public void SerializeError_Test()
        {
            var result = new ExecResult
            {
                CommandId = 123,
                Error = "Test error"
            };

            var data = result.Serialize();
            var result2 = ExecResult.Deserialize(data);

            Assert.AreEqual(123, result2.CommandId);
            Assert.AreEqual("Test error", result2.Error);
        }
    }
}
