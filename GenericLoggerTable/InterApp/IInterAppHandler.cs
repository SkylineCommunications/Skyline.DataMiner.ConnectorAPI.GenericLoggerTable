namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp
{
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	internal interface IInterAppHandler
	{
		void SendMessage(Message message);

		T SendMessageWithResponse<T>(Message message) where T : Message;
	}
}