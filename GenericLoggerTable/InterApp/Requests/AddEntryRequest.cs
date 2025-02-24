namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests
{
	/// <summary>
	/// Request to add the entry.
	/// </summary>
	public class AddEntryRequest : DataRequest
	{
		/// <summary>
		/// Gets or sets a value indicating whether to allow overwriting an existing entry.
		/// </summary>
		public bool AllowOverwrite { get; set; }
	}
}