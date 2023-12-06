using System;

namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    public class AddEntryRequest
    {
        public string Id { get; set; }

        public string Data { get; set; }

        public bool AllowOverwrite { get; set; }
    }
}
