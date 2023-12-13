namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Executors
{
    using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;

    internal class GetEntryResultExecutor : MessageExecutor<GetEntryResult>
    {
        public GetEntryResultExecutor(GetEntryResult message) : base(message)
        {
        }

        public override Message CreateReturnMessage()
        {
            return null;
        }

        public override void DataGets(object dataSource)
        {
        }

        public override void DataSets(object dataDestination)
        {
        }

        public override void Modify()
        {

        }

        public override void Parse()
        {

        }

        public override bool Validate()
        {
            return true;
        }
    }
}
