namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class RemoveEntryRequest : Message
    {
        public string Id { get; set; }
    }
}
