namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class AddEntryRequest : Message
    {
        public string Id { get; set; }

        public string Data { get; set; }

        public bool AllowOverwrite { get; set; }
    }
}
