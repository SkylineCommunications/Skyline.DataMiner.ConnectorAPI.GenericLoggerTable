namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
	public interface IGenericLoggerTableElement
	{
		void AddEntry(string id, string data, bool allowOverwrite);
		void AppendEntry(string id, string data);
		bool EntryExists(string id);
		string GetEntry(string id);
		void RemoveEntry(string id);
		bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason);
		bool TryAppendEntry(string id, string data, out string reason);
		bool TryGetEntry(string id, out string data, out string reason);
		bool TryRemoveEntry(string id, out string reason);
		bool TryUpdateEntry(string id, string data, out string reason);
		void UpdateEntry(string id, string data);
	}
}