namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class AppendEntryRequest : Message
    {
        public string Id { get; set; }

        public string Data { get; set; }
    }
}
