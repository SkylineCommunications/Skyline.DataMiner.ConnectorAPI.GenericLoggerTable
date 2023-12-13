namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to add an entry to the table.
    /// </summary>
    public class AddEntryRequest : Message
    {
        /// <summary>
        /// Id of the entry to add.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Data of the entry.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Indicates whether the request is allowed to overwrite the data of an existing entry with the same Id.
        /// </summary>
        public bool AllowOverwrite { get; set; }
    }
}
