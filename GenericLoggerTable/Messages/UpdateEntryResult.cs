namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of updating an entry in the table.
    /// </summary>
    public class UpdateEntryResult : Message
    {
        /// <summary>
        /// Indicates whether the entry was updated or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the reason why the entry could not be updated.
        /// </summary>
        public string Reason { get; set; }
    }
}
