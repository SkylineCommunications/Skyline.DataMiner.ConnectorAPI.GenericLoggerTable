namespace GenericLoggerTableTests
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Requests;
	using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.InterApp.Responses;

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
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<EntryExistsResponse>(It.Is<EntryExistsRequest>(req => req.Id == existingId))).Returns(new EntryExistsResponse { Exists = true });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<EntryExistsResponse>(It.Is<EntryExistsRequest>(req => req.Id == nonExistingId))).Returns(new EntryExistsResponse { Exists = false });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<GetEntryResponse>(It.Is<GetEntryRequest>(req => req.Id == existingId))).Returns(new GetEntryResponse { Success = true, Error = String.Empty, Data = data });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<GetEntryResponse>(It.Is<GetEntryRequest>(req => req.Id == nonExistingId))).Returns(new GetEntryResponse { Success = false, Error = error, Data = String.Empty });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<AddEntryResponse>(It.Is<AddEntryRequest>(req => req.Id == nonExistingId))).Returns(new AddEntryResponse { Success = true, Error = String.Empty });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<AddEntryResponse>(It.Is<AddEntryRequest>(req => req.Id == existingId && req.AllowOverwrite == false))).Returns(new AddEntryResponse { Success = false, Error = error });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<AddEntryResponse>(It.Is<AddEntryRequest>(req => req.Id == existingId && req.AllowOverwrite == true))).Returns(new AddEntryResponse { Success = true, Error = String.Empty });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<AppendEntryResponse>(It.Is<AppendEntryRequest>(req => req.Id == existingId))).Returns(new AppendEntryResponse { Success = true, Error = String.Empty });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<AppendEntryResponse>(It.Is<AppendEntryRequest>(req => req.Id == nonExistingId))).Returns(new AppendEntryResponse { Success = false, Error = error });

			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<RemoveEntryResponse>(It.Is<RemoveEntryRequest>(req => req.Id == existingId))).Returns(new RemoveEntryResponse { Success = true, Error = String.Empty });
			mockInterAppHandler.Setup(x => x.SendMessageWithResponse<RemoveEntryResponse>(It.Is<RemoveEntryRequest>(req => req.Id == nonExistingId))).Returns(new RemoveEntryResponse { Success = false, Error = error });

			tableElement = new GenericLoggerTableElement(mockInterAppHandler.Object);
		}

		[TestMethod]
		public void EntryExists_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.EntryExists(existingId);

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void EntryExists_NonExistingId_ReturnsFalse()
		{
			// Arrange & Act
			bool result = tableElement.EntryExists(nonExistingId);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void GetEntry_ExistingId_ReturnsData()
		{
			// Arrange & Act
			string result = tableElement.GetEntry(existingId);

			// Assert
			Assert.AreEqual(data, result);
		}

		[TestMethod]
		public void GetEntry_NonExistingId_ReturnsEmptyString()
		{
			// Arrange & Act
			string result = tableElement.GetEntry(nonExistingId);

			// Assert
			Assert.AreEqual(String.Empty, result);
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
		}

		[TestMethod]
		public void TryAddEntry_NonExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(nonExistingId, data, true, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryAddEntry_ExistingIdAllowOverwriteTrue_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(existingId, data, true, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryAddEntry_ExistingIdAllowOverwriteFalse_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryAddEntry(existingId, data, false, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryAppendEntry_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryAppendEntry(existingId, data, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryAppendEntry_NonExistingId_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryAppendEntry(nonExistingId, data, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));
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
		public void TryRemoveEntry_ExistingId_ReturnsTrue()
		{
			// Arrange & Act
			bool result = tableElement.TryRemoveEntry(existingId, out string reason);

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(String.IsNullOrEmpty(reason));
		}

		[TestMethod]
		public void TryRemoveEntry_NonExistingId_ReturnsFalseAndError()
		{
			// Arrange & Act
			bool result = tableElement.TryRemoveEntry(nonExistingId, out string reason);

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(String.IsNullOrEmpty(reason));
		}
	}
}