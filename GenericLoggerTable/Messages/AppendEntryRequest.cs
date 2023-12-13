namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to append data to an existing entry in the table.
    /// </summary>
    public class AppendEntryRequest : Message
    {
        /// <summary>
        /// Id of the entry in which the data should be appended.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Data to append.
        /// </summary>
        public string Data { get; set; }
    }
}
