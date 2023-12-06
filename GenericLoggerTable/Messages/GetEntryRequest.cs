namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class GetEntryRequest : Message
    {
        public string Id { get; set; }
    }
}
