namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of removing an entry from the table.
    /// </summary>
    public class RemoveEntryResult : Message
    {

        /// <summary>
        /// Indicates whether the entry was removed or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the reason why the entry could not be removed.
        /// </summary>
        public string Reason { get; set; }
    }
}
