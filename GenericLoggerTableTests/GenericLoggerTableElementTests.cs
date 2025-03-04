namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTableTests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp;

	using String = System.String;

	[TestClass]
	public class GenericLoggerTableElementTests
	{
		private const string existingId = "ID1";
		private const string nonExistingId = "ID2";
		private const string data = "Data";
		private const string error = "error";

		private GenericLoggerTableElement tableElement;
		private Mock<IInterAppHandler> mockInterAppHandler;

		[TestInitialize]
		public void Setup()
		{
			mockInterAppHandler = new Mock<IInterAppHandler>();
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.Action == Action.Exists))).Returns(new Response { Exists = true });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == nonExistingId && req.Action == Action.Exists))).Returns(new Response { Exists = false });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.Action == Action.Get))).Returns(new Response { Success = true, Error = String.Empty, Data = data });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == nonExistingId && req.Action == Action.Get))).Returns(new Response { Success = false, Error = error, Data = String.Empty });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == nonExistingId && req.Action == Action.Add))).Returns(new Response { Success = true, Error = String.Empty });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.AllowOverwrite == false && req.Action == Action.Add))).Returns(new Response { Success = false, Error = error });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.AllowOverwrite == true && req.Action == Action.Add))).Returns(new Response { Success = true, Error = String.Empty });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.Action == Action.Append))).Returns(new Response { Success = true, Error = String.Empty });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == nonExistingId && req.Action == Action.Append))).Returns(new Response { Success = false, Error = error });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse(It.Is<Request>(req => req.Id == existingId && req.Action == Action.Remove))).Returns(new Response { Success = true, Error = String.Empty });

			tableElement = new GenericLoggerTableElement(mockInterAppHandler.Object);
		}

		[TestMethod]
		public void EntryExists_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.EntryExists(existingId);

			// Assert
			Assert.IsTrue(result);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Action == Action.Exists)), Times.Once);
		}

		[TestMethod]
		public void EntryExists_NonExistingId_ReturnsFalse()
		{
			// Arrange & Act
			bool result = tableElement.EntryExists(nonExistingId);

			// Assert
			Assert.IsFalse(result);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == nonExistingId && r.Action == Action.Exists)), Times.Once);
		}

		[TestMethod]
		public void GetEntry_ExistingId_ReturnsData()
		{
			// Arrange & Act
			string result = tableElement.GetEntry(existingId);

			// Assert
			Assert.AreEqual(data, result);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Action == Action.Get)), Times.Once);
		}

		[TestMethod]
		public void GetEntry_NonExistingId_ReturnsEmptyString()
		{
			// Arrange & Act
			string result = tableElement.GetEntry(nonExistingId);

			// Assert
			Assert.AreEqual(String.Empty, result);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == nonExistingId && r.Action == Action.Get)), Times.Once);
		}

		[TestMethod]
		public void TryGetEntry_ExistingId_ReturnsTrueAndData()
		{
			// Arrange & Act
			bool result = tableElement.TryGetEntry(existingId, out string resultData, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
			Assert.AreEqual(data, resultData);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Action == Action.Get)), Times.Once);
		}

		[TestMethod]
		public void TryGetEntry_NonExistingId_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryGetEntry(nonExistingId, out string resultData, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));
			Assert.AreEqual(String.Empty, resultData);

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == nonExistingId && r.Action == Action.Get)), Times.Once);
		}

		[TestMethod]
		public void TryAddEntry_NonExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(nonExistingId, data, true, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == nonExistingId && r.Data == data && r.AllowOverwrite == true && r.Action == Action.Add)), Times.Once);
		}

		[TestMethod]
		public void TryAddEntry_ExistingIdAllowOverwriteTrue_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(existingId, data, true, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Data == data && r.AllowOverwrite == true && r.Action == Action.Add)), Times.Once);
		}

		[TestMethod]
		public void TryAddEntry_ExistingIdAllowOverwriteFalse_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(existingId, data, false, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Data == data && r.AllowOverwrite == false && r.Action == Action.Add)), Times.Once);
		}

		[TestMethod]
		public void TryAppendEntry_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAppendEntry(existingId, data, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Data == data && r.Action == Action.Append)), Times.Once);
		}

		[TestMethod]
		public void TryAppendEntry_NonExistingId_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryAppendEntry(nonExistingId, data, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == nonExistingId && r.Data == data && r.Action == Action.Append)), Times.Once);
		}

		[TestMethod]
		public void TryUpdateEntry_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryUpdateEntry(existingId, data, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryUpdateEntry_NonExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryUpdateEntry(nonExistingId, data, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryRemoveEntry_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryRemoveEntry(existingId, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));

			mockInterAppHandler.Verify(x => x.SendMessageWithResponse(It.Is<Request>(r => r.Id == existingId && r.Action == Action.Remove)), Times.Once);
		}
	}
}