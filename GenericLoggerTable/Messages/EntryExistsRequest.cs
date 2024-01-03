namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to check whether a specific entry is present in the table.
    /// </summary>
    public class EntryExistsRequest : Message
    {
        /// <summary>
        /// Id of the entry to check.
        /// </summary>
        public string Id { get; set; }
    }
}
