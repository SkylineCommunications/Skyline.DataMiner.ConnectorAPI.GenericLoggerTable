namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Responses
{
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests;

	/// <summary>
	/// Response to <see cref="EntryExistsRequest"/>.
	/// </summary>
	public class EntryExistsResponse : Response
	{
		/// <summary>
		/// Gets or sets a value indicating whether the entry exists.
		/// </summary>
		public bool Exists { get; set; }
	}
}