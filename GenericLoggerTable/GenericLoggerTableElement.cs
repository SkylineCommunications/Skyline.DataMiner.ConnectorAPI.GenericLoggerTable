namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
    using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Executors;
    using Skyline.DataMiner.ConnectorAPI.GenericLoggerTable.Messages;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.DataMiner.Core.InterAppCalls.Common.Shared;
    using Skyline.DataMiner.Net;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a Generic Logger Table element in DataMiner and exposes methods to request and push data to and from its internal logger table.
    /// </summary>
    public class GenericLoggerTableElement
    {
        /// <summary>
        /// Name of the Generic Logger Table protocol.
        /// </summary>
        public const string GenericLoggerTable_ProtocolName = "Generic Logger Table";

        /// <summary>
        /// ID of the parameter in the Generic Logger Table protocol that's used to receive incoming InterApp messages.
        /// </summary>
        public const int InterAppReceive_ParameterId = 9000000;

        /// <summary>
        /// ID of the parameter in the Generic Logger Table protocol that's used to return outgoing InterApp messages.
        /// </summary>
        public const int InterAppReturn_ParameterId = 9000001;

        private readonly IConnection connection;
        private readonly int agentId;
        private readonly int elementId;

        private readonly List<Type> knownTypes = new List<Type>
        {
            typeof(IInterAppCall),
            typeof(AddEntryRequest),
            typeof(AppendEntryRequest),
            typeof(GetEntryRequest),
            typeof(RemoveEntryRequest),
            typeof(UpdateEntryRequest),
            typeof(AddEntryResult),
            typeof(AppendEntryResult),
            typeof(GetEntryResult),
            typeof(RemoveEntryResult),
            typeof(UpdateEntryResult),
        };

        private readonly Dictionary<Type, Type> executorMap = new Dictionary<Type, Type>
        {
            { typeof(GetEntryResult), typeof(GetEntryResultExecutor) },
            { typeof(AppendEntryResult), typeof(AppendEntryResultExecutor) },
            { typeof(AddEntryResult), typeof(AddEntryResultExecutor) },
            { typeof(RemoveEntryResult), typeof(RemoveEntryResultExecutor) },
            { typeof(UpdateEntryResult), typeof(UpdateEntryResultExecutor) },
        };

        /// <summary>
        /// Creates a new instance of a GenericLoggerTable.
        /// </summary>
        /// <param name="connection">Connection used to communicate with the Generic Logger Table element.</param>
        /// <param name="agentId">The DataMiner agent id of the Generic Logger Table element.</param>
        /// <param name="elementId">The element id of the Generic Logger Table element.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided connection or the element is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided element is not using the Generic Logger Table protocol.</exception>
        public GenericLoggerTableElement(IConnection connection, int agentId, int elementId)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Maximum amount of time in which every request to the Generic Logger Table should be handled.
        /// Default: 5 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Retrieves data from the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of the entry to retrieve.</param>
        /// <returns>Data contained in the requested entry.</returns>
        /// <exception cref="InvalidOperationException">Thrown if we're unable to retrieve the requested data.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
        public string GetEntry(string id)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var request = new GetEntryRequest
            {
                Id = id,
            };

            if (!TrySendMessage<GetEntryResult>(request, true, out string reason, out var response))
            {
                throw new InvalidOperationException($"Unable to get entry with id {id} due to {reason}");
            }

            if (!response.Success) throw new InvalidOperationException($"Unable to get entry with id {id} due to {response.Reason}");

            return response.Data;
        }

        /// <summary>
        /// Retrieves data from the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of entry to retrieve.</param>
        /// <param name="data">Data contained in the requested entry.</param>
        /// <param name="reason">Reason if data could not be retrieved.</param>
        /// <returns>Value indicating if data could be retrieved or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
        public bool TryGetEntry(string id, out string data, out string reason)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var request = new GetEntryRequest
            {
                Id = id,
            };

            if (!TrySendMessage<GetEntryResult>(request, true, out reason, out var response))
            {
                data = null;
                return false;
            }

            data = response.Data;
            reason = response.Reason;
            return response.Success;
        }

        /// <summary>
        /// Removes an entry from the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of entry to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
        public void RemoveEntry(string id)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var request = new RemoveEntryRequest
            {
                Id = id
            };

            TrySendMessage<RemoveEntryResult>(request, false, out _, out _);
        }

        /// <summary>
        /// Removes an entry from the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of entry to remove.</param>
        /// <param name="reason">Reason why the entry could not be removed.</param>
        /// <returns>Value indicating if the entry was removed or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string.</exception>
        public bool TryRemoveEntry(string id, out string reason)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var request = new RemoveEntryRequest
            {
                Id = id
            };

            if (!TrySendMessage<RemoveEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

        /// <summary>
        /// Adds a new entry to the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public void AddEntry(string id, string data)
        {
            AddEntry(id, data, false);
        }

        /// <summary>
        /// Adds a new entry to the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of the entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <param name="allowOverwrite">Indicates if the data of existing entries can be overwritten.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public void AddEntry(string id, string data, bool allowOverwrite)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new AddEntryRequest
            {
                Id = id,
                Data = data,
                AllowOverwrite = allowOverwrite
            };

            TrySendMessage<AddEntryResult>(request, false, out _, out _);
        }

        /// <summary>
        /// Add a new entry to the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of the entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <param name="reason">Reason why the entry could not be added.</param>
        /// <returns>Value indicating if the entry was added or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public bool TryAddEntry(string id, string data, out string reason)
        {
            return TryAddEntry(id, data, false, out reason);
        }

        /// <summary>
        /// Add a new entry to the Generic Logger Table.
        /// </summary>
        /// <param name="id">Id of the entry to add.</param>
        /// <param name="data">Data of entry to add.</param>
        /// <param name="allowOverwrite">Indicates if the data of existing entries can be overwritten.</param>
        /// <param name="reason">Reason why the entry could not be added.</param>
        /// <returns>Value indicating if the entry was added or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public bool TryAddEntry(string id, string data, bool allowOverwrite, out string reason)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new AddEntryRequest
            {
                Id = id,
                Data = data,
                AllowOverwrite = allowOverwrite
            };

            if (!TrySendMessage<AddEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

        /// <summary>
        /// Appends the provided data to the data of an existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to be appended.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public void AppendEntry(string id, string data)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new AppendEntryRequest
            {
                Id = id,
                Data = data
            };

            TrySendMessage<AppendEntryResult>(request, false, out _, out _);
        }

        /// <summary>
        /// Appends the provided data to the data of an existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to be appended.</param>
        /// <param name="reason">Reason why the data could not be appended.</param>
        /// <returns>Value indicating if the data was appended or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public bool TryAppendEntry(string id, string data, out string reason)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new AppendEntryRequest
            {
                Id = id,
                Data = data
            };

            if (!TrySendMessage<AppendEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

        /// <summary>
        /// Overwrites the data of an existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to update the existing entry with.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public void UpdateEntry(string id, string data)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new UpdateEntryRequest
            {
                Id = id,
                Data = data
            };

            TrySendMessage<UpdateEntryResult>(request, false, out _, out _);
        }

        /// <summary>
        /// Overwrites the data of an existing entry.
        /// </summary>
        /// <param name="id">Id of the entry to update.</param>
        /// <param name="data">Data to update the existing entry with.</param>
        /// <param name="reason">Reason why the data not be updated.</param>
        /// <returns>Value indicating whether the data was updated or not.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided id is null or an empty string or if the data is null.</exception>
        public bool TryUpdateEntry(string id, string data, out string reason)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (data == null) throw new ArgumentNullException(nameof(data));

            var request = new UpdateEntryRequest
            {
                Id = id,
                Data = data
            };

            if (!TrySendMessage<UpdateEntryResult>(request, true, out reason, out var response))
            {
                return false;
            }

            reason = response.Reason;
            return response.Success;
        }

        private bool TrySendMessage<T>(Message message, bool requiresResponse, out string reason, out T responseMessage) where T : Message
        {
            reason = String.Empty;
            responseMessage = default(T);

            var commands = InterAppCallFactory.CreateNew();
            commands.ReturnAddress = new ReturnAddress(agentId, elementId, InterAppReturn_ParameterId);
            commands.Messages.Add(message);

            try
            {
                if (requiresResponse)
                {
                    var response = commands.Send(connection, agentId, elementId, InterAppReceive_ParameterId, Timeout, knownTypes).First();
                    if (!response.TryExecute(null, null, executorMap, out _))
                    {
                        reason = $"Unable to execute response";
                        return false;
                    }

                    if (!(response is T castResponse))
                    {
                        reason = $"Received response is not of type {typeof(T)}";
                        return false;
                    }

                    responseMessage = castResponse;
                }
                else
                {
                    commands.Send(connection, agentId, elementId, InterAppReceive_ParameterId, knownTypes);
                }
            }
            catch (Exception e)
            {
                reason = e.ToString();
                return false;
            }

            return true;
        }
    }
}
