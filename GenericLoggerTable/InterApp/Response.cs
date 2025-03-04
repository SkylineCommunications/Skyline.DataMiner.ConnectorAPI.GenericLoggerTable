namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
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

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <remarks>Used by <see cref="Action.Get"/>.</remarks>
		public string Data { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the entry exists.
		/// </summary>
		/// <remarks>Used by <see cref="Action.Exists"/>.</remarks>
		public bool Exists { get; set; }
	}
}