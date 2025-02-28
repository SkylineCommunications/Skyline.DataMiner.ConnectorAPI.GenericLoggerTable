namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	/// <summary>
	/// Base class for all requests.
	/// </summary>
	public class Request : Message
	{
		private string id;

		/// <summary>
		/// Gets or sets the ID of the request.
		/// </summary>
		public string Id
		{
			get { return id; }
			set { id = value.ToUpper(); } // Ensure that the ID is always in uppercase because of how logger table queries are created.
		}
	}
}