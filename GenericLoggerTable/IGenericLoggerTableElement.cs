namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
	/// <summary>
	/// Defines an interface to interact with a Generic Logger Table element.
	/// </summary>
	public interface IGenericLoggerTableElement
	{
		/// <summary>
		/// Adds an entry to the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="allowOverwrite"></param>
		void AddEntry(string id, string data, bool allowOverwrite);

		/// <summary>
		/// Appends an entry to the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		void AppendEntry(string id, string data);

		/// <summary>
		/// Checks whether an entry exists in the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		bool EntryExists(string id);

		/// <summary>
		/// Gets an entry from the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		string GetEntry(string id);

		/// <summary>
		/// Removes an entry from the logger table.
		/// </summary>
		/// <param name="id"></param>
		void RemoveEntry(string id);

		/// <summary>
		/// Tries to add an entry to the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="allowOverwrite"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason);

		/// <summary>
		/// Tries to append an entry to an existing logger table entry.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool TryAppendEntry(string id, string data, out string reason);

		/// <summary>
		/// Tries to get an entry from the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool TryGetEntry(string id, out string data, out string reason);

		/// <summary>
		/// Tries to remove an entry.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool TryRemoveEntry(string id, out string reason);

		/// <summary>
		/// Tries to update an entry.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool TryUpdateEntry(string id, string data, out string reason);

		/// <summary>
		/// Updates an entry in the logger table.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		void UpdateEntry(string id, string data);
	}
}