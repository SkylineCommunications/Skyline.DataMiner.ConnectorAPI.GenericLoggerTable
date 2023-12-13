namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to remove an entry from the table.
    /// </summary>
    public class RemoveEntryRequest : Message
    {
        /// <summary>
        /// Id of the entry to remove from the table.
        /// </summary>
        public string Id { get; set; }
    }
}
