namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of adding an entry to the table.
    /// </summary>
    public class AddEntryResult : Message
    {
        /// <summary>
        /// Indicates whether the entry was added or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the reason why the entry could not be added.
        /// </summary>
        public string Reason { get; set; }
    }
}
