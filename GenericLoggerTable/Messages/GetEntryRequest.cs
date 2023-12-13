namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to retrieve an entry from the table.
    /// </summary>
    public class GetEntryRequest : Message
    {
        /// <summary>
        /// Id of the entry to retrieve.
        /// </summary>
        public string Id { get; set; }
    }
}
