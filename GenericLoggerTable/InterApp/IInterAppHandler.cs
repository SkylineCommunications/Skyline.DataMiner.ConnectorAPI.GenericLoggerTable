namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	internal interface IInterAppHandler
	{
		void SendMessage(Message message);

		Response SendMessageWithResponse(Request request);
	}
}