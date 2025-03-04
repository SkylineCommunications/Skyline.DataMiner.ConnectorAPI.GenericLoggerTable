namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	/// <summary>
	/// Base class for all requests.
	/// </summary>
	public class Request : Message
	{
		/// <summary>
		/// Gets or sets the action to be performed.
		/// </summary>
		public Action Action { get; set; }

		/// <summary>
		/// Gets or sets the ID of the request.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <remarks>Used by <see cref="Action.Add"/> and <see cref="Action.Append"/>.</remarks>
		public string Data { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to allow overwriting an existing entry.
		/// </summary>
		/// <remarks>Used by <see cref="Action.Add"/>.</remarks>
		public bool AllowOverwrite { get; set; }
	}
}