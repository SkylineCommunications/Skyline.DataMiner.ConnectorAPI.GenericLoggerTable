namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using System;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class GetEntryResult : Message
    {
        public string Id { get; set; }

        public string Data { get; set; }
    }
}
