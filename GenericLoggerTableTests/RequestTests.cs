namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTableTests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests;

	[TestClass]
	public class RequestTests
	{
		[TestMethod]
		public void Id_SetValue_ValueIsUpperCase()
		{
			// Arrange
			var request = new Request();

			// Act
			request.Id = "id";

			// Assert
			Assert.AreEqual("ID", request.Id);
		}

		[TestMethod]
		public void Id_GetValue_ReturnsValue()
		{
			// Arrange
			var request = new Request();

			// Act
			request.Id = "id";
			
			// Assert
			Assert.AreEqual("ID", request.Id);
		}
	}
}