namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Net;

	/// <summary>
	/// Handles the communication between two processes using the InterApp Framework.
	/// </summary>
	public class InterAppHandler : IInterAppHandler
	{
		/// <summary>
		/// List of known types.
		/// </summary>
		public static IReadOnlyList<Type> KnownTypes { get; } = new List<Type> { typeof(Request), typeof(Response) };

		private readonly IConnection connection;
		private readonly int agentId;
		private readonly int elementId;

		/// <summary>
		/// Initializes a new instance of the <see cref="InterAppHandler"/> class.
		/// </summary>
		/// <param name="connection">Raw SLNet connection.</param>
		/// <param name="agentId">Id of the agent on which the element is hosted.</param>
		/// <param name="elementId">Id of the element.</param>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="connection"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Throws if <paramref name="agentId"/> or <paramref name="elementId"/> are negative or zero.</exception>
		internal InterAppHandler(IConnection connection, int agentId, int elementId)
		{
			this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
			this.agentId = agentId <= 0 ? throw new ArgumentOutOfRangeException(nameof(agentId), "Agent ID cannot be negative") : agentId;
			this.elementId = elementId <= 0 ? throw new ArgumentOutOfRangeException(nameof(elementId), "Element ID cannot be negative") : elementId;
		}

		/// <summary>
		/// Sends a request without waiting for a response.
		/// </summary>
		/// <param name="request">Request to send.</param>
		public void SendMessage(Request request)
		{
			var interAppCall = InterAppCallFactory.CreateNew();

			interAppCall.Messages.Add(request);

			interAppCall.Send(connection, agentId, elementId, 9000000, KnownTypes);
		}

		/// <summary>
		/// Sends a request and waits for a response.
		/// </summary>
		/// <param name="request">Request to send.</param>
		/// <returns>Returns the response.</returns>
		public Response SendMessageWithResponse(Request request)
		{
			var interAppCall = InterAppCallFactory.CreateNew();

			interAppCall.Messages.Add(request);

			var responses = interAppCall.Send(connection, agentId, elementId, 9000000, TimeSpan.FromMinutes(1), KnownTypes);

			return (Response)responses.SingleOrDefault();
		}
	}
}