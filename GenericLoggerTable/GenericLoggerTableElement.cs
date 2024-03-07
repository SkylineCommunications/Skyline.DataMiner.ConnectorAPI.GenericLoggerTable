namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents a Generic Logger Table element in DataMiner and exposes methods to request and push data to and from its internal logger Table element.
    /// </summary>
    public class GenericLoggerTableElement
    {
        /// <summary>
        /// Name of the Generic Logger Table protocol.
        /// </summary>
        public const string GenericLoggerTable_ProtocolName = "Generic Logger Table";

        private readonly IConnection connection;
        private readonly int agentId;
        private readonly int elementId;
        private readonly string tableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericLoggerTableElement"/> class.
        /// </summary>
        /// <param name="connection">Connection used to communicate with the Generic Logger Table element.</param>
        /// <param name="agentId">ID of the agent on which the Generic Logger Table element is hosted.</param>
        /// <param name="elementId">ID of the Generic Logger Table element.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided connection or the element is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when provided element or agent id is negative.</exception>
        public GenericLoggerTableElement(IConnection connection, int agentId, int elementId)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.agentId = agentId < 0 ? throw new ArgumentOutOfRangeException(nameof(agentId), "Agent ID cannot be negative") : agentId;
            this.elementId = elementId < 0 ? throw new ArgumentOutOfRangeException(nameof(elementId), "Element ID cannot be negative") : elementId;
            this.tableName = GetTableName();
        }

        /// <summary>
        /// Checks whether an entry with the <paramref name="id"/> exists in database.
        /// </summary>
        /// <param name="id">Id of the entry to check.</param>
        /// <returns>True if entry exists, else false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if we're unable to check if the entry exists.</exception>
        public bool EntryExists(string id)
        {
            var query = $"SELECT id FROM {tableName} WHERE id='{QueryEscaper.Escape(id)}' LIMIT 1 /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            if (!String.IsNullOrEmpty(response.Error))
            {
                throw new InvalidOperationException($"Unable to check if entry with id {id} exists due to {response.Error}");
            }

            return response.Values.Sa.Any();
        }

        /// <summary>
        /// Retrieves data from database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the entry to retrieve.</param>
        /// <returns>Data contained in the requested entry.</returns>
        /// <exception cref="InvalidOperationException">Thrown if we're unable to retrieve the requested data.</exception>
        public string GetEntry(string id)
        {
            var query = $"SELECT dt FROM {tableName} WHERE id='{QueryEscaper.Escape(id)}' LIMIT 1 /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            if (!String.IsNullOrEmpty(response.Error))
            {
                throw new InvalidOperationException($"Unable to get entry with id {id} due to {response.Error}");
            }

            if (!response.Values.Sa.Any())
            {
                throw new InvalidOperationException($"Entry with id {id} doesn't exist in the table");
            }

            string data = QueryEscaper.Unescape(response.Values.Sa.FirstOrDefault());

            return data;
        }

        /// <summary>
        /// Attempts to retrieve data from database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of entry to retrieve.</param>
        /// <param name="data">Data contained in the requested entry.</param>
        /// <param name="reason">Reason if data could not be retrieved.</param>
        /// <returns>True if data was retrieved, else false.</returns>
        public bool TryGetEntry(string id, out string data, out string reason)
        {
            var query = $"SELECT dt FROM {tableName} WHERE id='{QueryEscaper.Escape(id)}' LIMIT 1 /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            bool entryExists = response.Values.Sa.Any();
            data = QueryEscaper.Unescape(response.Values.Sa.FirstOrDefault());
            reason = entryExists ? response.Error : "Entry doesn't exist in the table";
            return entryExists && String.IsNullOrWhiteSpace(response.Error);
        }

        /// <summary>
        /// Removes an entry from database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of entry to remove.</param>
        public void RemoveEntry(string id)
        {
            string query = $"DELETE FROM {tableName} WHERE id = '{QueryEscaper.Escape(id)}' /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            connection.HandleSingleResponseMessage(message);
        }

        /// <summary>
        /// Attempts to remove an entry from database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of entry to remove.</param>
        /// <param name="reason">Reason why the entry could not be removed.</param>
        /// <returns>True if entry was removed, else false.</returns>
        public bool TryRemoveEntry(string id, out string reason)
        {
            string query = $"DELETE FROM {tableName} WHERE id = '{QueryEscaper.Escape(id)}' /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            reason = response.Error;
            return String.IsNullOrWhiteSpace(reason);
        }

        /// <summary>
        /// Adds a new entry to database.
        /// </summary>
        /// <param name="id">Id of the entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
        public void AddEntry(string id, string data, bool allowOverwrite)
        {
            if (!allowOverwrite && EntryExists(id)) return;

            string timestamp = DateTime.UtcNow.ToString("G", CultureInfo.CreateSpecificCulture("fr-CA"));
            string query;
            if (allowOverwrite)
            {
                query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{QueryEscaper.Escape(id)}', '{QueryEscaper.Escape(data)}', '{timestamp}') /*unlimited query*/";
            }
            else
            {
                query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{QueryEscaper.Escape(id)}', '{QueryEscaper.Escape(data)}', '{timestamp}') IF NOT EXISTS /*unlimited query*/";
            }

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            connection.HandleSingleResponseMessage(message);
        }

        /// <summary>
        /// Attempts to add a new entry to database.
        /// </summary>
        /// <param name="id">Id of the entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
        /// <param name="reason">Reason why the entry could not be added.</param>
        /// <returns>True if entry was added, else false.</returns>
        public bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason)
        {
            if (!allowOverwrite && EntryExists(id))
            {
                reason = $"Unable to add entry because an entry with ID {QueryEscaper.Escape(id)} already exists and allow overwrite is disabled";
                return false;
            }

            string timestamp = DateTime.UtcNow.ToString("G", CultureInfo.CreateSpecificCulture("fr-CA"));

            string query;
            if (allowOverwrite)
            {
                query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{QueryEscaper.Escape(id)}', '{QueryEscaper.Escape(data)}', '{timestamp}') /*unlimited query*/";
            }
            else
            {
                query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{QueryEscaper.Escape(id)}', '{QueryEscaper.Escape(data)}', '{timestamp}') IF NOT EXISTS /*unlimited query*/";
            }

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            reason = response.Error;
            return String.IsNullOrWhiteSpace(reason);
        }

        /// <summary>
        /// Appends the provided data to an existing entry in database based on <paramref name="id"/>.
        /// This operation throws an exception if there's no existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to be appended.</param>
        public void AppendEntry(string id, string data)
        {
            var oldData = GetEntry(id);
            oldData += data;

            UpdateEntry(id, oldData);
        }

        /// <summary>
        /// Attempts to append the provided data to an existing entry in database based on <paramref name="id"/>.
        /// This operation fails if there's no existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to be appended.</param>
        /// <param name="reason">Reason why the data could not be appended.</param>
        /// <returns>True if entry was appended, else false.</returns>
        public bool TryAppendEntry(string id, string data, out string reason)
        {
            if (!TryGetEntry(id, out string oldData, out reason))
            {
                return false;
            }

            oldData += data;

            if (!TryAddEntry(id, oldData, true, out reason))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overwrites the data of an existing entry in database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to update the existing entry with.</param>
        public void UpdateEntry(string id, string data)
        {
            string query = $"UPDATE {tableName} SET dt = '{QueryEscaper.Escape(data)}' WHERE id = '{QueryEscaper.Escape(id)}' IF EXISTS /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            connection.HandleSingleResponseMessage(message);
        }

        /// <summary>
        /// Attempts to overwrite the data of an existing entry in database based on <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to update the existing entry with.</param>
        /// <param name="reason">Reason why the data was not be updated.</param>
        /// <returns>True if entry was updated, else false.</returns>
        public bool TryUpdateEntry(string id, string data, out string reason)
        {
            string query = $"UPDATE {tableName} SET dt = '{QueryEscaper.Escape(data)}' WHERE id = '{QueryEscaper.Escape(id)}' IF EXISTS /*unlimited query*/";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)connection.HandleSingleResponseMessage(message);

            reason = response.Error;
            return String.IsNullOrWhiteSpace(reason);
        }

        private string GetTableName()
        {
            var prefix = String.Empty;

            var getDataBaseInfoMessage = new GetInfoMessage(InfoType.Database);
            var dataBaseInfoResponseMessage = (GetDataBaseInfoResponseMessage)connection.HandleSingleResponseMessage(getDataBaseInfoMessage);
            if (dataBaseInfoResponseMessage?.LocalDatabaseInfo?.DatabaseType == DBMSType.CassandraCluster)
            {
                var db = dataBaseInfoResponseMessage.LocalDatabaseInfo.DB;
                prefix = String.Format("{0}_elementdata_{1}_{2}_1000.", db, agentId, elementId);
            }

            string name = String.Format("{0}elementdata_{1}_{2}_1000", prefix, agentId, elementId);

            return name;
        }
    }
}
