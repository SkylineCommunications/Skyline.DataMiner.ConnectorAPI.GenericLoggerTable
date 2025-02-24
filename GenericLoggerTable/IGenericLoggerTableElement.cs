namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
	/// <summary>
	/// Defines an interface to interact with a Generic Logger Table element.
	/// </summary>
	public interface IGenericLoggerTableElement
	{
		/// <summary>
		/// Checks whether an entry exists in the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to check.</param>
		/// <returns>True if entry exists, else false.</returns>
		bool EntryExists(string id);

		/// <summary>
		/// Gets an entry from the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to retrieve.</param>
		/// <returns>Data contained in the requested entry.</returns>
		string GetEntry(string id);

		/// <summary>
		/// Tries to get an entry from the logger table.
		/// </summary>
		/// <param name="id">Id of entry to retrieve.</param>
		/// <param name="data">Data contained in the requested entry.</param>
		/// <param name="reason">Reason why the entry could not be retrieved.</param>
		/// <returns>True if data was retrieved, else false.</returns>
		bool TryGetEntry(string id, out string data, out string reason);

		/// <summary>
		/// Adds an entry to the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		void AddEntry(string id, string data, bool allowOverwrite);

		/// <summary>
		/// Tries to add an entry to the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to add.</param>
		/// <param name="data">Data to add.</param>
		/// <param name="allowOverwrite">True if existing entry can be overwritten, else false.</param>
		/// <param name="reason">Reason why the entry could not be added.</param>
		/// <returns>True if entry was added, else false.</returns>
		bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason);

		/// <summary>
		/// Appends an entry to the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to appended.</param>
		void AppendEntry(string id, string data);

		/// <summary>
		/// Tries to append an entry to an existing logger table entry.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to append.</param>
		/// <param name="reason">Reason why the data could not be appended.</param>
		/// <returns>True if entry was appended, else false.</returns>
		bool TryAppendEntry(string id, string data, out string reason);

		/// <summary>
		/// Updates an entry in the logger table.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		void UpdateEntry(string id, string data);

		/// <summary>
		/// Tries to update an entry.
		/// </summary>
		/// <param name="id">Id of the entry to update.</param>
		/// <param name="data">Data to update the existing entry with.</param>
		/// <param name="reason">Reason why the data was not be updated.</param>
		/// <returns>True if entry was updated, else false.</returns>
		bool TryUpdateEntry(string id, string data, out string reason);

		/// <summary>
		/// Removes an entry from the logger table.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		void RemoveEntry(string id);

		/// <summary>
		/// Tries to remove an entry.
		/// </summary>
		/// <param name="id">Id of entry to remove.</param>
		/// <param name="reason">Reason why the entry could not be removed.</param>
		/// <returns>True if entry was removed, else false.</returns>
		bool TryRemoveEntry(string id, out string reason);
	}
}