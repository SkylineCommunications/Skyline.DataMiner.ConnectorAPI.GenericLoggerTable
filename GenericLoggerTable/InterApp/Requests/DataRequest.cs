namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests
{
	/// <summary>
	/// Base class for all requests containing data.
	/// </summary>
	public class DataRequest : Request
	{
		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data { get; set; }
	}
}