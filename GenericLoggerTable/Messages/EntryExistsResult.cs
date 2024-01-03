namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Result of checking whether an entry is present in the table.
    /// </summary>
    public class EntryExistsResult : Message
    {
        /// <summary>
        /// Indicates whether the specific entry is present in the table or not.
        /// </summary>
        public bool Exists { get; set; }
    }
}
