namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
{
	/// <summary>
	/// Enumerates the possible actions.
	/// </summary>
	public enum Action
	{
		/// <summary>
		/// Check if the entry exists.
		/// </summary>
		Exists,

		/// <summary>
		/// Get the entry.
		/// </summary>
		Get,

		/// <summary>
		/// Add the entry.
		/// </summary>
		Add,

		/// <summary>
		/// Append to existing entry.
		/// </summary>
		Append,

		/// <summary>
		/// Remove the entry.
		/// </summary>
		Remove,
	}
}