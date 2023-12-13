namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages
{
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

    /// <summary>
    /// Used to overwrite the data of an existing entry.
    /// </summary>
    public class UpdateEntryRequest : Message
    {
        /// <summary>
        /// Id of the entry to update.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Data to overwrite data of existing entry.
        /// </summary>
        public string Data { get; set; }
    }
}
