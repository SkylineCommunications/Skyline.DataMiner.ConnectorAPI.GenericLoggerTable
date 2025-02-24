namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Responses
{
	/// <summary>
	/// Base class for all responses containing data.
	/// </summary>
	public class DataResponse : Response
	{
		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		public string Data { get; set; }
	}
}