﻿namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    public class AddEntryResult : Message
    {
        public bool Success { get; set; }

        public string Reason { get; set; }
    }
}
