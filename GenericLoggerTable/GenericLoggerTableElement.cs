namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Responses;
	using Skyline.DataMiner.Net;

	/// <summary>
	/// Represents a Generic Logger Table element in DataMiner and exposes methods to request and push data to and from its internal logger Table element.
	/// </summary>
	public class GenericLoggerTableElement : IGenericLoggerTableElement
	{
		private readonly InterAppHandler interApp;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericLoggerTableElement"/> class.
		/// </summary>
		/// <param name="connection">Connection used to communicate with the Generic Logger Table element.</param>
		/// <param name="agentId">Id of the agent on which the Generic Logger Table element is hosted.</param>
		/// <param name="elementId">Id of the Generic Logger Table element.</param>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="connection"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Throws if <paramref name="agentId"/> or <paramref name="elementId"/> are negative or zero.</exception>
		public GenericLoggerTableElement(IConnection connection, int agentId, int elementId)
		{
			this.interApp = new InterAppHandler(connection, agentId, elementId);
		}

		/// <summary>
		/// Checks whether an entry with the <paramref name="id"/> exists in the table.
		/// </summary>
		/// <param name="id">Id of the entry to check.</param>
		/// <returns>True if entry exists, else false.</returns>
		public bool EntryExists(string id)
		{
			var message = new EntryExistsRequest
			{
				Id = id
			};

			var response = interApp.SendMessageWithResponse<EntryExistsResponse>(message);

			return response.Success && response.Exists;
		}

		/// <summary>
		/// Retrieves data from the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of the entry to retrieve.</param>
		/// <returns>Data contained in the requested entry.</returns>
		public string GetEntry(string id)
		{
			var message = new GetEntryRequest
			{
				Id = id
			};

			var response = interApp.SendMessageWithResponse<GetEntryResponse>(message);

			return response.Success ? response.Data : String.Empty;
		}

		/// <summary>
		/// Attempts to retrieve data from the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of entry to retrieve.</param>
		/// <param name="data">Data contained in the requested entry.</param>
		/// <param name="reason">Reason why the entry could not be retrieved.</param>
		/// <returns>True if data was retrieved, else false.</returns>
		public bool TryGetEntry(string id, out string data, out string reason)
		{
			var message = new GetEntryRequest
			{
				Id = id
			};

			var response = interApp.SendMessageWithResponse<GetEntryResponse>(message);

			data = response.Success ? response.Data : String.Empty;

			return ProcessResponse(response, out reason);
		}

		/// <summary>
		/// Removes an entry from the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		public void RemoveEntry(string id)
		{
			var message = new RemoveEntryRequest
			{
				Id = id
			};

			interApp.SendMessage(message);
		}

		/// <summary>
		/// Attempts to remove an entry from the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="reason">Reason why the entry could not be removed.</param>
		/// <returns>True if entry was removed, else false.</returns>
		public bool TryRemoveEntry(string id, out string reason)
		{
			var message = new RemoveEntryRequest
			{
				Id = id
			};

			var response = interApp.SendMessageWithResponse<RemoveEntryResponse>(message);

			return ProcessResponse(response, out reason);
		}

		/// <summary>
		/// Adds a new entry to the table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		public void AddEntry(string id, string data, bool allowOverwrite)
		{
			var message = new AddEntryRequest
			{
				Id = id,
				Data = data,
				AllowOverwrite = allowOverwrite
			};

			interApp.SendMessage(message);
		}

		/// <summary>
		/// Attempts to add a new entry to the table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <returns>True if entry was added, else false.</returns>
		public bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason)
		{
			var message = new AddEntryRequest
			{
				Id = id,
				Data = data,
				AllowOverwrite = allowOverwrite
			};

			var response = interApp.SendMessageWithResponse<AddEntryResponse>(message);

			return ProcessResponse(response, out reason);
		}

		/// <summary>
		/// Appends the provided data to an existing entry in the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to append.</param>
		public void AppendEntry(string id, string data)
		{
			var message = new AppendEntryRequest
			{
				Id = id,
				Data = data
			};

			interApp.SendMessage(message);
		}

		/// <summary>
		/// Attempts to append the provided data to an existing entry in the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to be appended.</param>
		/// <param name="reason">Reason why the data could not be appended.</param>
		/// <returns>True if entry was appended, else false.</returns>
		public bool TryAppendEntry(string id, string data, out string reason)
		{
			var message = new AppendEntryRequest
			{
				Id = id,
				Data = data
			};

			var response = interApp.SendMessageWithResponse<AppendEntryResponse>(message);

			return ProcessResponse(response, out reason);
		}

		/// <summary>
		/// Overwrites the data of an existing entry in the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		public void UpdateEntry(string id, string data)
		{
			AddEntry(id, data, true);
		}

		/// <summary>
		/// Attempts to overwrite the data of an existing entry in the table based on <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="reason">Reason why the data was not be updated.</param>
		/// <returns>True if entry was updated, else false.</returns>
		public bool TryUpdateEntry(string id, string data, out string reason)
		{
			return TryAddEntry(id, data, true, out reason);
		}

		private bool ProcessResponse(Response response, out string reason)
		{
			reason = response.Success ? String.Empty : response.Error;

			return response.Success;
		}
	}
}