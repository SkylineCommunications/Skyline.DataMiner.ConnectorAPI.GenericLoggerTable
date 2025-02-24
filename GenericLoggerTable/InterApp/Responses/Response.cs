namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Responses
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	/// <summary>
	/// Base class for all responses.
	/// </summary>
	public class Response : Message
	{
		/// <summary>
		/// Gets or sets value indicating if request was successful.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Gets or sets the error message if request was not successful.
		/// </summary>
		public string Error { get; set; }
	}
}