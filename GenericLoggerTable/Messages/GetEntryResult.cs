namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using System;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of requesting an entry from the table.
    /// </summary>
    public class GetEntryResult : Message
    {
        /// <summary>
        /// Indicates whether the entry was retrieved or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the reason why the entry could not be retrieved.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Data retrieved from the table.
        /// </summary>
        public string Data { get; set; }
    }
}
