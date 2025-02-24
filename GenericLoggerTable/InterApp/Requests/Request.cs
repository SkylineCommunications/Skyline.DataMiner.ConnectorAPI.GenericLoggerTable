namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	/// <summary>
	/// Base class for all requests.
	/// </summary>
	public class Request : Message
	{
		/// <summary>
		/// Gets or sets the ID of the request.
		/// </summary>
		public string Id { get; set; }
	}
}