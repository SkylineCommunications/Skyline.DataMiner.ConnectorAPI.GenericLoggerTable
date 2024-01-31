namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
    using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.DataMiner.Core.InterAppCalls.Common.Shared;
    using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	/// <summary>
	/// Represents a Generic Logger Table element in DataMiner and exposes methods to request and push data to and from its internal logger table.
	/// </summary>
    public class GenericLoggerTableElement
    {
        /// <summary>
        /// Name of the Generic Logger Table protocol.
        /// </summary>
        public const string GenericLoggerTable_ProtocolName = "Generic Logger Table";

        /// <summary>
        /// ID of the parameter in the Generic Logger Table protocol that's used to receive incoming InterApp messages.
        /// </summary>
        public const int InterAppReceive_ParameterId = 9000000;

        /// <summary>
        /// ID of the parameter in the Generic Logger Table protocol that's used to return outgoing InterApp messages.
        /// </summary>
        public const int InterAppReturn_ParameterId = 9000001;

        private readonly IConnection connection;
        private readonly int agentId;
        private readonly int elementId;
        private readonly IDms dms;
        private readonly string tableName;

        private static readonly List<Type> knownTypes = new List<Type>
        {
            typeof(IInterAppCall),
            typeof(AddEntryRequest),
            typeof(AppendEntryRequest),
            typeof(GetEntryRequest),
            typeof(RemoveEntryRequest),
            typeof(UpdateEntryRequest),
            typeof(AddEntryResult),
            typeof(AppendEntryResult),
            typeof(GetEntryResult),
            typeof(RemoveEntryResult),
            typeof(UpdateEntryResult),
            typeof(EntryExistsRequest),
            typeof(EntryExistsResult)
        };

        /// <summary>
        /// List of known types. Used during InterApp communication.
        /// </summary>
        public static IEnumerable<Type> KnownTypes => knownTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericLoggerTableElement"/> class.
        /// </summary>
        /// <param name="connection">Connection used to communicate with the Generic Logger Table element.</param>
        /// <param name="agentId">ID of the agent on which the Generic Logger Table element is hosted.</param>
        /// <param name="elementId">ID of the Generic Logger Table element.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided connection or the element is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when provided element or agent id is negative.</exception>
        public GenericLoggerTableElement(Connection connection, int agentId, int elementId)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.agentId = agentId < 0 ? throw new ArgumentOutOfRangeException(nameof(agentId), "Agent ID cannot be negative") : agentId;
            this.elementId = elementId < 0 ? throw new ArgumentOutOfRangeException(nameof(elementId), "Element ID cannot be negative") : elementId;
            this.dms = connection.GetDms();
            this.tableName = GetTableName();
        }

        /// <summary>
        /// Maximum amount of time in which every request to the Generic Logger Table should be handled.
        /// Default: 5 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

		/// <summary>
		/// Checks whether an entry with the given Id exists in database.
		/// </summary>
		/// <param name="id">Id of the entry to check.</param>
		/// <param name="sendRequest">True if check should be handled by Generic Logger Table driver.</param>
		/// <returns>True if entry exists, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to check if the entry exists.</exception>
		public bool EntryExists(string id, bool sendRequest = true)
		{
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

			if (sendRequest) return EntryExistsExternal(id);
			else return EntryExistsInternal(id);
		}

		/// <summary>
		/// Checks whether an entry with the given Id exists in the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to check.</param>
		/// <returns>True if entry exists, else false.</returns>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to check if the entry exists.</exception>
		private bool EntryExistsExternal(string id)
        {
            var request = new EntryExistsRequest
            {
                Id = id,
            };

            if (!TrySendMessage<EntryExistsResult>(request, true, out string reason, out var response))
            {
                throw new InvalidOperationException($"Unable to check if entry with id {id} exists due to {reason}");
            }

            return response.Exists;
        }

		/// <summary>
		/// Checks whether an entry with the given Id exists in database.
		/// </summary>
		/// <param name="id">Id of the entry to check.</param>
		/// <returns>True if entry exists, else false.</returns>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to check if the entry exists.</exception>
		private bool EntryExistsInternal(string id)
		{
            var query = $"SELECT id FROM {tableName} WHERE id='{id}' LIMIT 1";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
            var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
            var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

            if (!String.IsNullOrEmpty(response.Error))
			{
				throw new InvalidOperationException($"Unable to check if entry with id {id} exists due to {response.Error}");
			}

			return response.Values.Sa.Any();
        }

		/// <summary>
		/// Retrieves data from database.
		/// </summary>
		/// <param name="id">Id of the entry to retrieve.</param>
		/// <param name="sendRequest">True if retrieval should be handled by Generic Logger Table driver.</param>
		/// <returns>Data contained in the requested entry.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to retrieve the requested data.</exception>
		public string GetEntry(string id, bool sendRequest = true)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (sendRequest) return GetEntryExternal(id);
            else return GetEntryInternal(id);
		}

		/// <summary>
		/// Retrieves data from the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to retrieve.</param>
		/// <returns>Data contained in the requested entry.</returns>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to retrieve the requested data.</exception>
		private string GetEntryExternal(string id)
        {
            var request = new GetEntryRequest
            {
                Id = id,
            };

            if (!TrySendMessage<GetEntryResult>(request, true, out string reason, out var response))
            {
                throw new InvalidOperationException($"Unable to get entry with id {id} due to {reason}");
            }

            if (!response.Success) throw new InvalidOperationException($"Unable to get entry with id {id} due to {response.Reason}");

            return response.Data;
        }

		/// <summary>
		/// Retrieves data from database.
		/// </summary>
		/// <param name="id">Id of the entry to retrieve.</param>
		/// <returns>Data contained in the requested entry.</returns>
		/// <exception cref="InvalidOperationException">Thrown if we're unable to retrieve the requested data.</exception>
		private string GetEntryInternal(string id)
        {
			var query = $"SELECT dt FROM {tableName} WHERE id='{id}' LIMIT 1";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

            if (!String.IsNullOrEmpty(response.Error))
            {
				throw new InvalidOperationException($"Unable to check if entry with id {id} exists due to {response.Error}");
			}

			if (!response.Values.Sa.Any())
            {
				throw new InvalidOperationException($"Entry with id {id} doesn't exist in the table");
			}

			string data = response.Values.Sa.FirstOrDefault();

            return data;
		}

		/// <summary>
		/// Attempts to retrieve data from database.
		/// </summary>
		/// <param name="id">Id of entry to retrieve.</param>
		/// <param name="data">Data contained in the requested entry.</param>
		/// <param name="reason">Reason if data could not be retrieved.</param>
		/// <param name="sendRequest">True if retrieval should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if data was retrieved, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
		public bool TryGetEntry(string id, out string data, out string reason, bool sendRequest = true)
        {
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (sendRequest) return TryGetEntryExternal(id, out data, out reason);
            else return TryGetEntryInternal(id, out data, out reason);
		}

		/// <summary>
		/// Attempts to retrieve data from the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of entry to retrieve.</param>
		/// <param name="data">Data contained in the requested entry.</param>
		/// <param name="reason">Reason if data could not be retrieved.</param>
		/// <returns>True if data was retrieved, else false.</returns>
		private bool TryGetEntryExternal(string id, out string data, out string reason)
        {
            var request = new GetEntryRequest
            {
                Id = id,
            };

            if (!TrySendMessage<GetEntryResult>(request, true, out reason, out var response))
            {
                data = null;
                return false;
            }

            data = response.Data;
            reason = response.Reason;
            return response.Success;
        }

		/// <summary>
		/// Attempts to retrieve data from database.
		/// </summary>
		/// <param name="id">Id of entry to retrieve.</param>
		/// <param name="data">Data contained in the requested entry.</param>
		/// <param name="reason">Reason if data could not be retrieved.</param>
		/// <returns>True if data was retrieved, else false.</returns>
		private bool TryGetEntryInternal(string id, out string data, out string reason)
        {
			var query = $"SELECT dt FROM {tableName} WHERE id='{id}' LIMIT 1";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

			bool entryExists = response.Values.Sa.Any();
			data = response.Values.Sa.FirstOrDefault();
			reason = entryExists ? response.Error : "Entry doesn't exist in the table";
			return entryExists && String.IsNullOrWhiteSpace(response.Error);
		}

		/// <summary>
		/// Removes an entry from database.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="sendRequest">True if removal should be handled by Generic Logger Table driver.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
		public void RemoveEntry(string id, bool sendRequest = true)
        {
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (sendRequest) RemoveEntryExternal(id);
            else RemoveEntryInternal(id);
		}

		/// <summary>
		/// Removes an entry from the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		private void RemoveEntryExternal(string id)
        {
            var request = new RemoveEntryRequest
            {
                Id = id
            };

            TrySendMessage<RemoveEntryResult>(request, false, out _, out _);
        }

		/// <summary>
		/// Removes an entry from database.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		private void RemoveEntryInternal(string id)
        {
			string query = $"DELETE FROM {tableName} WHERE id = '{id}'";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var _ = dms.Communication.SendSingleResponseMessage(message);
		}

		/// <summary>
		/// Attempts to remove an entry from database.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="reason">Reason why the entry could not be removed.</param>
		/// <param name="sendRequest">True if removal should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if entry was removed, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
		public bool TryRemoveEntry(string id, out string reason, bool sendRequest = true)
        {
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (sendRequest) return TryRemoveEntryExternal(id, out reason);
            else return TryRemoveEntryInternal(id, out reason);
		}

		/// <summary>
		/// Attempts to remove an entry from the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="reason">Reason why the entry could not be removed.</param>
		/// <returns>True if entry was removed, else false.</returns>
		private bool TryRemoveEntryExternal(string id, out string reason)
        {
            var request = new RemoveEntryRequest
            {
                Id = id
            };

            if (!TrySendMessage<RemoveEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

		/// <summary>
		/// Attempts to remove an entry from database.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="reason">Reason why the entry could not be removed.</param>
		/// <returns>True if entry was removed, else false.</returns>
		private bool TryRemoveEntryInternal(string id, out string reason)
        {
			string query = $"DELETE FROM {tableName} WHERE id = '{id}'";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

			reason = response.Error;
			return String.IsNullOrWhiteSpace(reason);
		}

		/// <summary>
		/// Adds a new entry to database.
		/// </summary>
		/// <param name="id">Id of entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="sendRequest">True if add should be handled by Generic Logger Table driver.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public void AddEntry(string id, string data, bool allowOverwrite = false, bool sendRequest = true)
        {
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

            if (sendRequest) AddEntryExternal(id, data, allowOverwrite);
            else AddEntryInternal(id, data, allowOverwrite);
        }

		/// <summary>
		/// Adds a new entry to the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		private void AddEntryExternal(string id, string data, bool allowOverwrite)
        {
            var request = new AddEntryRequest
            {
                Id = id,
                Data = data,
                AllowOverwrite = allowOverwrite
            };

            TrySendMessage<AddEntryResult>(request, false, out _, out _);
        }

		/// <summary>
		/// Adds a new entry to database.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		private void AddEntryInternal(string id, string data, bool allowOverwrite)
        {
            if (!allowOverwrite && EntryExists(id, false)) return;

			string timestamp = DateTime.UtcNow.ToString("G", CultureInfo.CreateSpecificCulture("fr-CA"));

			string query;
			if (allowOverwrite)
			{
				query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{id}', '{data}', '{timestamp}')";
			}
			else
			{
				query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{id}', '{data}', '{timestamp}') IF NOT EXISTS";
			}

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var _ = dms.Communication.SendSingleResponseMessage(message);
		}

		/// <summary>
		/// Attempts to add a new entry to database.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="sendRequest">True if add should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if entry was added, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public bool TryAddEntry(string id, string data, out string reason, bool allowOverwrite = false, bool sendRequest = true)
        {
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

            if (sendRequest) return TryAddEntryExternal(id, data, allowOverwrite, out reason);
            else return TryAddEntryInternal(id, data, allowOverwrite, out reason);
        }

		/// <summary>
		/// Attempts to add a new entry to database.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <param name="sendRequest">True if add should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if entry was added, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason, bool sendRequest = true)
        {
            return TryAddEntry(id, data, out reason, allowOverwrite, sendRequest);
		}

		/// <summary>
		/// Attempts to add a new entry to the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <returns>True if entry was added, else false.</returns>
		private bool TryAddEntryExternal(string id, string data, bool allowOverwrite, out string reason)
        {
            var request = new AddEntryRequest
            {
                Id = id,
                Data = data,
                AllowOverwrite = allowOverwrite
            };

            if (!TrySendMessage<AddEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
			return response.Success;
		}

		/// <summary>
		/// Attempts to add a new entry to database.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data of entry to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <returns>True if entry was added, else false.</returns>
		private bool TryAddEntryInternal(string id, string data, bool allowOverwrite, out string reason)
        {
			if (!allowOverwrite && EntryExists(id))
			{
				reason = $"Unable to add entry because an entry with ID {id} already exists and allow overwrite is disabled";
				return false;
			}

			string timestamp = DateTime.UtcNow.ToString("G", CultureInfo.CreateSpecificCulture("fr-CA"));

			string query;
			if (allowOverwrite)
			{
				query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{id}', '{data}', '{timestamp}')";
			}
			else
			{
				query = $"INSERT INTO {tableName} (id, dt, ts) VALUES ('{id}', '{data}', '{timestamp}') IF NOT EXISTS";
			}

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

			reason = response.Error;
			return String.IsNullOrWhiteSpace(reason);
		}

		/// <summary>
		/// Appends the provided data to an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		/// <param name="sendRequest">True if append should be handled by Generic Logger Table driver.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public void AppendEntry(string id, string data, bool sendRequest = true)
		{
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (sendRequest) AppendEntryExternal(id, data);
			else AppendEntryInternal(id, data);
		}

		/// <summary>
		/// Appends the provided data to an existing entry in the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		private void AppendEntryExternal(string id, string data)
        {
            var request = new AppendEntryRequest
            {
                Id = id,
                Data = data
            };

            TrySendMessage<AppendEntryResult>(request, false, out _, out _);
        }

		/// <summary>
		/// Appends the provided data to an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		private void AppendEntryInternal(string id, string data)
		{
			if (!TryGetEntry(id, out string oldData, out _, false))
			{
				oldData = String.Empty;
			}

			oldData += data;

			TryAddEntry(id, oldData, out _, true, false);
		}

		/// <summary>
		/// Attempts to append the provided data to an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		/// <param name="reason">Reason why the data could not be appended.</param>
		/// <param name="sendRequest">True if append should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if entry was appended, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public bool TryAppendEntry(string id, string data, out string reason, bool sendRequest = true)
		{
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (sendRequest) return TryAppendEntryExternal(id, data, out reason);
			else return TryAppendEntryInternal(id, data, out reason);
		}

		/// <summary>
		/// Attempts to append the provided data to an existing entry in the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		/// <param name="reason">Reason why the data could not be appended.</param>
		/// <returns>True if entry was appended, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		private bool TryAppendEntryExternal(string id, string data, out string reason)
        {
            var request = new AppendEntryRequest
            {
                Id = id,
                Data = data
            };

            if (!TrySendMessage<AppendEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

		/// <summary>
		/// Attempts to append the provided data to an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		/// <param name="reason">Reason why the data could not be appended.</param>
		/// <returns>True if entry was appended, else false.</returns>
		private bool TryAppendEntryInternal(string id, string data, out string reason)
		{
			if (!TryGetEntry(id, out string oldData, out _, false))
			{
				oldData = String.Empty;
			}

			oldData += data;

			if (!TryAddEntry(id, oldData, out reason, true, false))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Overwrites the data of an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="sendRequest">True if update should be handled by Generic Logger Table driver.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public void UpdateEntry(string id, string data, bool sendRequest = true)
		{
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (sendRequest) UpdateEntryExternal(id, data);
			else UpdateEntryInternal(id, data);
		}

		/// <summary>
		/// Overwrites the data of an existing entry in the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		private void UpdateEntryExternal(string id, string data)
        {
            var request = new UpdateEntryRequest
            {
                Id = id,
                Data = data
            };

            TrySendMessage<UpdateEntryResult>(request, false, out _, out _);
        }

		/// <summary>
		/// Overwrites the data of an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		private void UpdateEntryInternal(string id, string data)
		{
			string query = $"UPDATE {tableName} SET dt = '{data}' WHERE id = '{id}' IF EXISTS";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var _ = dms.Communication.SendSingleResponseMessage(message);
		}

		/// <summary>
		/// Attempts to overwrite the data of an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="reason">Reason why the data was not be updated.</param>
		/// <param name="sendRequest">True if update should be attempted by Generic Logger Table driver.</param>
		/// <returns>True if entry was updated, else false.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
		public bool TryUpdateEntry(string id, string data, out string reason, bool sendRequest = true)
		{
			if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
			if (data == null) throw new ArgumentNullException(nameof(data));

			if (sendRequest) return TryUpdateEntryExternal(id, data, out reason);
			else return TryUpdateEntryInternal(id, data, out reason);
		}

		/// <summary>
		/// Attempts to overwrite the data of an existing entry in the Generic Logger Table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="reason">Reason why the data was not be updated.</param>
		/// <returns>True if entry was updated, else false.</returns>
		private bool TryUpdateEntryExternal(string id, string data, out string reason)
        {
            var request = new UpdateEntryRequest
            {
                Id = id,
                Data = data
            };

            if (!TrySendMessage<UpdateEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

		/// <summary>
		/// Attempts to overwrite the data of an existing entry in database.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="reason">Reason why the data was not be updated.</param>
		/// <returns>True if entry was updated, else false.</returns>
		private bool TryUpdateEntryInternal(string id, string data, out string reason)
		{
			string query = $"UPDATE {tableName} SET dt = '{data}' WHERE id = '{id}' IF EXISTS";

#pragma warning disable CS0612 // No other functionality available (used by DataMiner Cube as well)
			var message = new ExecuteDatabaseQueryMessage(query, agentId);
#pragma warning restore CS0612
			var response = (ExecuteDatabaseQueryResponseMessage)dms.Communication.SendSingleResponseMessage(message);

			reason = response.Error;
			return String.IsNullOrWhiteSpace(reason);
		}

		private bool TrySendMessage<T>(Message message, bool requiresResponse, out string reason, out T responseMessage) where T : Message
        {
            reason = String.Empty;
            responseMessage = default(T);

            var commands = InterAppCallFactory.CreateNew();
            commands.Messages.Add(message);

            try
            {
                if (requiresResponse)
                {
                    commands.ReturnAddress = new ReturnAddress(agentId, elementId, InterAppReturn_ParameterId);

                    var response = commands.Send(connection, agentId, elementId, InterAppReceive_ParameterId, Timeout, knownTypes).First();
                    if (!(response is T castResponse))
                    {
                        reason = $"Received response is not of type {typeof(T)}";
                        return false;
                    }

                    responseMessage = castResponse;
                }
                else
                {
                    commands.Send(connection, agentId, elementId, InterAppReceive_ParameterId, knownTypes);
                }
            }
            catch (Exception e)
            {
                reason = e.ToString();
                return false;
            }

            return true;
        }

		private string GetTableName()
		{
			var prefix = String.Empty;

			var getDataBaseInfoMessage = new GetInfoMessage(InfoType.Database);
			var dataBaseInfoResponseMessage = (GetDataBaseInfoResponseMessage)dms.Communication.SendSingleResponseMessage(getDataBaseInfoMessage);
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
