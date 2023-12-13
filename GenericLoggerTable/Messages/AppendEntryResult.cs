namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of appending data to an existing entry in the table.
    /// </summary>
    public class AppendEntryResult : Message
    {
        /// <summary>
        /// Indicates whether the data was appended or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the reason why the data could not be appended.
        /// </summary>
        public string Reason { get; set; }
    }
}
