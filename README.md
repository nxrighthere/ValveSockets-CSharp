<p align="center">
  <img src="https://i.imgur.com/CS43fv4.png" alt="alt logo">
</p>

[![GitHub release](https://img.shields.io/github/release/nxrighthere/ValveSockets-CSharp.svg?style=flat-square)](https://github.com/nxrighthere/ValveSockets-CSharp/releases)

This repository provides a managed C# abstraction of [GameNetworkingSockets](https://github.com/ValveSoftware/GameNetworkingSockets) library which is created and maintained by [Valve Software](https://www.valvesoftware.com). You will need to [build](https://github.com/ValveSoftware/GameNetworkingSockets#building) the native library with all required dependencies before you get started.

The project is [updating](https://github.com/nxrighthere/ValveSockets-CSharp/issues/8#issuecomment-616596904) in accordance with the [releases](https://github.com/ValveSoftware/GameNetworkingSockets/releases) of the native library.

Building
--------

A managed assembly can be built using any available compiling platform that supports C# 3.0 or higher.

Define `VALVESOCKETS_SPAN` to enable support for Span. Please, follow [these steps](https://github.com/nxrighthere/ValveSockets-CSharp/issues/3#issuecomment-491916163) to enable fast access to native memory blocks and improve performance.

Usage
--------
Before starting to work, the library should be initialized using `Valve.Sockets.Library.Initialize();` function.

After the work is done, deinitialize the library using `Valve.Sockets.Library.Deinitialize();` function.

### .NET environment
##### Start a new server
```c#
NetworkingSockets server = new NetworkingSockets();

uint pollGroup = server.CreatePollGroup();

StatusCallback status = (ref StatusInfo info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connecting:
			server.AcceptConnection(info.connection);
			server.SetConnectionPollGroup(pollGroup, info.connection);
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
			break;

		case ConnectionState.ClosedByPeer:
		case ConnectionState.ProblemDetectedLocally:
			server.CloseConnection(info.connection);
			Console.WriteLine("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
			break;
	}
};

utils.SetStatusCallback(status);

Address address = new Address();

address.SetAddress("::0", port);

uint listenSocket = server.CreateListenSocket(ref address);

#if VALVESOCKETS_SPAN
	MessageCallback message = (in NetworkingMessage netMessage) => {
		Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
	};
#else
	const int maxMessages = 20;

	NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif

while (!Console.KeyAvailable) {
	server.RunCallbacks();

	#if VALVESOCKETS_SPAN
		server.ReceiveMessagesOnPollGroup(pollGroup, message, 20);
	#else
		int netMessagesCount = server.ReceiveMessagesOnPollGroup(pollGroup, netMessages, maxMessages);

		if (netMessagesCount > 0) {
			for (int i = 0; i < netMessagesCount; i++) {
				ref NetworkingMessage netMessage = ref netMessages[i];

				Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);

				netMessage.Destroy();
			}
		}
	#endif

	Thread.Sleep(15);
}

server.DestroyPollGroup(pollGroup);
```

##### Start a new client
```c#
NetworkingSockets client = new NetworkingSockets();

uint connection = 0;

StatusCallback status = (ref StatusInfo info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected to server - ID: " + connection);
			break;

		case ConnectionState.ClosedByPeer:
		case ConnectionState.ProblemDetectedLocally:
			client.CloseConnection(connection);
			Console.WriteLine("Client disconnected from server");
			break;
	}
};

utils.SetStatusCallback(status);

Address address = new Address();

address.SetAddress("::1", port);

connection = client.Connect(ref address);

#if VALVESOCKETS_SPAN
	MessageCallback message = (in NetworkingMessage netMessage) => {
		Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
	};
#else
	const int maxMessages = 20;

	NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
#endif

while (!Console.KeyAvailable) {
	client.RunCallbacks();

	#if VALVESOCKETS_SPAN
		client.ReceiveMessagesOnConnection(connection, message, 20);
	#else
		int netMessagesCount = client.ReceiveMessagesOnConnection(connection, netMessages, maxMessages);

		if (netMessagesCount > 0) {
			for (int i = 0; i < netMessagesCount; i++) {
				ref NetworkingMessage netMessage = ref netMessages[i];

				Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);

				netMessage.Destroy();
			}
		}
	#endif

	Thread.Sleep(15);
}
```

##### Create and send a new message
```c#
byte[] data = new byte[64];

sockets.SendMessageToConnection(connection, data);
```

##### Copy payload from a message
```c#
byte[] buffer = new byte[1024];

netMessage.CopyTo(buffer);
```

##### Set a hook for debug information
```c#
DebugCallback debug = (type, message) => {
	Console.WriteLine("Debug - Type: " + type + ", Message: " + message);
};

NetworkingUtils utils = new NetworkingUtils();

utils.SetDebugCallback(DebugType.Everything, debug);
```

### Unity
Usage is almost the same as in the .NET environment, except that the console functions must be replaced with functions provided by Unity. If the `NetworkingSockets.RunCallbacks()` will be called in a game loop, then keep Unity run in background by enabling the appropriate option in the player settings.

API reference
--------
### Enumerations
#### SendFlags
Definitions of a flags for `NetworkingSockets.SendMessageToConnection()` function:

`SendFlags.Unreliable` unreliable, delivery of message is not guaranteed, the message may be delivered out of order.

`SendFlags.Reliable` reliable ordered, a message must be received by the target connection and resend attempts should be made until the message is delivered.

`SendFlags.NoNagle` a message will not be grouped with other messages within a timer.

`SendFlags.NoDelay` a message will not be buffered if it can't be sent relatively quickly.

#### IdentityType
Definitions of identity type for `NetworkingIdentity` structure:

`IdentityType.Invalid` unknown or invalid.

`IdentityType.SteamID` Steam identifier.

`IdentityType.IPAddress` IPv4/IPv6 address.

#### ConnectionState
Definitions of connection states for `ConnectionInfo.state` field:

`ConnectionState.None` dummy state, the connection doesn't exist or has already been closed.

`ConnectionState.Connecting` in-progress of establishing a connection initiated by `NetworkingSockets.Connect()` function.

`ConnectionState.FindingRoute` if the server accepts the connection, then this connection switch into the rendezvous state, but the end-to-end route is still have not yet established (through the relay network).

`ConnectionState.Connected` a connection request initiated by `NetworkingSockets.Connect()` function has completed.

`ConnectionState.ClosedByPeer` a connection has been closed by the peer, but not closed locally. If there are any messages in the inbound queue, they can be retrieved. Otherwise, nothing may be done with the connection except to close it using `NetworkingSockets.CloseConnection()` function. The connection still exists from an API perspective and must be closed to free up resources.

`ConnectionState.ProblemDetectedLocally` a disruption in the connection has been detected locally. Attempts to send further messages will fail. Any remaining received messages in the queue are available. The connection still exists from an API perspective and must be closed to free up resources.

#### ConfigurationScope
Definitions of configuration scopes:

`ConfigurationScope.Global`

`ConfigurationScope.SocketsInterface`

`ConfigurationScope.ListenSocket`

`ConfigurationScope.Connection`

#### ConfigurationDataType
Definitions of configuration data types:

`ConfigurationDataType.Int32`

`ConfigurationDataType.Int64`

`ConfigurationDataType.Float`

`ConfigurationDataType.String`

`ConfigurationDataType.FunctionPtr`

#### ConfigurationValue
Definitions of configuration values:

`ConfigurationValue.Invalid`

`ConfigurationValue.FakePacketLossSend`

`ConfigurationValue.FakePacketLossRecv`

`ConfigurationValue.FakePacketLagSend`

`ConfigurationValue.FakePacketLagRecv`

`ConfigurationValue.FakePacketReorderSend`

`ConfigurationValue.FakePacketReorderRecv`

`ConfigurationValue.FakePacketReorderTime`

`ConfigurationValue.FakePacketDupSend`

`ConfigurationValue.FakePacketDupRecv`

`ConfigurationValue.FakePacketDupTimeMax`

`ConfigurationValue.TimeoutInitial`

`ConfigurationValue.TimeoutConnected`

`ConfigurationValue.SendBufferSize`

`ConfigurationValue.SendRateMin`

`ConfigurationValue.SendRateMax`

`ConfigurationValue.NagleTime`

`ConfigurationValue.IPAllowWithoutAuth`

`ConfigurationValue.MTUPacketSize`

`ConfigurationValue.MTUDataSize`

`ConfigurationValue.Unencrypted`

`ConfigurationValue.EnumerateDevVars`

`ConfigurationValue.SymmetricConnect`

`ConfigurationValue.LocalVirtualPort`

`ConfigurationValue.ConnectionStatusChanged`

`ConfigurationValue.AuthStatusChanged`

`ConfigurationValue.RelayNetworkStatusChanged`

`ConfigurationValue.MessagesSessionRequest`

`ConfigurationValue.MessagesSessionFailed`

`ConfigurationValue.P2PSTUNServerList`

`ConfigurationValue.P2PTransportICEEnable`

`ConfigurationValue.P2PTransportICEPenalty`

`ConfigurationValue.P2PTransportSDRPenalty`

`ConfigurationValue.SDRClientConsecutitivePingTimeoutsFailInitial`

`ConfigurationValue.SDRClientConsecutitivePingTimeoutsFail`

`ConfigurationValue.SDRClientMinPingsBeforePingAccurate`

`ConfigurationValue.SDRClientSingleSocket`

`ConfigurationValue.SDRClientForceRelayCluster`

`ConfigurationValue.SDRClientDebugTicketAddress`

`ConfigurationValue.SDRClientForceProxyAddr`

`ConfigurationValue.SDRClientFakeClusterPing`

`ConfigurationValue.LogLevelAckRTT`

`ConfigurationValue.LogLevelPacketDecode`

`ConfigurationValue.LogLevelMessage`

`ConfigurationValue.LogLevelPacketGaps`

`ConfigurationValue.LogLevelP2PRendezvous`

`ConfigurationValue.LogLevelSDRRelayPings`

#### ConfigurationValueResult
Definitions of configuration value results:

`ConfigurationValueResult.BadValue`

`ConfigurationValueResult.BadScopeObject`

`ConfigurationValueResult.BufferTooSmall`

`ConfigurationValueResult.OK`

`ConfigurationValueResult.OKInherited`

#### DebugType
Definitions of debug types:

`DebugType.None`

`DebugType.Bug`

`DebugType.Error`

`DebugType.Important`

`DebugType.Warning`

`DebugType.Message`

`DebugType.Verbose`

`DebugType.Debug`

`DebugType.Everything`

#### Result
Definitions of operation result:

`Result.OK` success.

`Result.Fail` generic failure.

`Result.NoConnection` failed network connection.

`Result.InvalidParam` a parameter is incorrect.

`Result.InvalidState` called object was in an invalid state.

`Result.Ignored` target is ignoring a sender.

### Delegates
#### Socket callbacks
Provides per socket events.

`StatusCallback(ref StatusInfo info)` notifies when dispatch mechanism on the listen socket returns a connection state. A reference to the delegate should be preserved from being garbage collected.

#### Library callbacks
Provides per application events.

`DebugCallback(DebugType type, string message)` notifies when debug information with the desired verbosity come up. A reference to the delegate should be preserved from being garbage collected.

### Structures
#### Address
Contains marshalled data with an IP address and port number.

`Address.ip` IP address in bytes.

`Address.port` port number.

`Address.IsLocalHost` checks if identity is localhost.

`Address.GetIP()` gets an IP address in a printable form.

`Address.SetLocalHost(ushort port)` sets localhost with a specified port.

`Address.SetAddress(string ip, ushort port)` sets an IP address (IPv4/IPv6) with a specified port.

`Address.Equals(Address other)` determines equality of addresses.

#### Configuration
Contains marshalled data with configuration.

`Configuration.value` a type of the value described in the `ConfigurationValue` enumeration.

`Configuration.dataType` a type of data described in the `ConfigurationDataType` enumeration.

`Configuration.data` a union of configuration data.

#### StatusInfo
Contains marshalled data with connection state.

`StatusInfo.connection` connection ID.

`StatusInfo.connectionInfo` essentially `ConnectionInfo` structure with marshalled data.

#### ConnectionInfo
Contains marshalled data with connection info.

`ConnectionInfo.identity` identifier of an endpoint.

`ConnectionInfo.userData` user-supplied data that set using `NetworkingSockets.SetConnectionUserData()` function.

`ConnectionInfo.listenSocket` listen socket for this connection.

`ConnectionInfo.address` remote address of an endpoint.

`ConnectionInfo.state` high-level state of the connection described in the `ConnectionState` enumeration.

`ConnectionInfo.endReason` basic cause of the connection termination or problem.

`ConnectionInfo.endDebug` explanation in a human-readable form for connection termination or problem. This is intended for debugging diagnostic purposes only, not for displaying to users. It might have some details specific to the issue.

`ConnectionInfo.connectionDescription` debug description includes the connection handle, connection type, and peer information.

#### ConnectionStatus
Contains marshalled data with connection status for frequent requests.

`ConnectionStatus.state` high-level state of the connection described in the `ConnectionState` enumeration.

`ConnectionStatus.ping` current ping in milliseconds.

`ConnectionStatus.connectionQualityLocal` connection quality measured locally (percentage of packets delivered end-to-end in order).

`ConnectionStatus.connectionQualityRemote` packet delivery success rate as observed from the remote host.

`ConnectionStatus.outPacketsPerSecond` current outbound packet rates from recent history.

`ConnectionStatus.outBytesPerSecond` current outbound data rates from recent history.

`ConnectionStatus.inPacketsPerSecond` current inbound packet rates from recent history.

`ConnectionStatus.inBytesPerSecond` current inbound data rates from recent history.

`ConnectionStatus.sendRateBytesPerSecond` estimated rate at which data can be sent to a peer. It could be significantly higher than `ConnectionStatus.outBytesPerSecond`, meaning the capacity of the channel is higher than the sent data.

`ConnectionStatus.pendingUnreliable` number of bytes pending to be sent unreliably. This is data that recently requested to be sent but has not yet actually been put on the wire.

`ConnectionStatus.pendingReliable` number of bytes pending to be sent reliably. The reliable number also includes data that was previously placed on the wire but has now been scheduled for re-transmission. Thus, it's possible to observe increasing of the bytes between two checks, even if no calls were made to send reliable data between the checks. Data that is awaiting the Nagle delay will appear in these numbers.

`ConnectionStatus.sentUnackedReliable` number of bytes of reliable data that has been placed the wire, but for which not yet received an acknowledgment, and thus might have to be re-transmitted.

#### NetworkingIdentity
Contains marshalled data of networking identity.

`NetworkingIdentity.type` description of a networking identity.

`NetworkingIdentity.IsInvalid` checks if identity has the invalid type.

`NetworkingIdentity.GetSteamID()` gets Steam ID.

`NetworkingIdentity.SetSteamID(ulong steamID)` sets Steam ID.

#### NetworkingMessage
Contains marshalled data of networking message.

`NetworkingMessage.identity` identifier of a sender.

`NetworkingMessage.connectionUserData` user-supplied connection data that set using `NetworkingSockets.SetConnectionUserData()` function.

`NetworkingMessage.timeReceived` local timestamp when the message was received.

`NetworkingMessage.messageNumber` message number assigned by a sender.

`NetworkingMessage.data` payload of a message.

`NetworkingMessage.length` length of the payload.

`NetworkingMessage.connection` connection ID from which the message came from.

`NetworkingMessage.channel` channel number the message was received on.

`NetworkingMessage.flags` flags that were used to send the message.

`NetworkingMessage.CopyTo(byte[] destination)` copies payload from the message to the destination array.

`NetworkingMessage.Destroy()` destroys the message. Should be called only when the messages are obtained from sockets.

### Classes
#### NetworkingSockets
Contains a managed pointer to the sockets.

`NetworkingSockets.CreateListenSocket(ref Address address, Configuration[] configurations)` creates a socket with optional configurations and returns a socket ID that listens for incoming connections which initiated by `NetworkingSockets.Connect()` function.

`NetworkingSockets.Connect(ref Address address, Configuration[] configurations)` initiates a connection to a foreign host with optional configurations. Returns a local connection ID.

`NetworkingSockets.AcceptConnection(Connection connection)` accepts an incoming connection that has received on a listen socket. When a connection attempt is received (perhaps after a few basic handshake packets have been exchanged to prevent trivial spoofing), a connection interface object is created in the `ConnectionState.Connecting` state and a `StatusCallback()` is called. Returns a result described in the `Result` enumeration.

`NetworkingSockets.CloseConnection(Connection connection, int reason, string debug, bool enableLinger)` disconnects from the host and invalidates the connection handle. Any unread data on the connection is discarded. The reason parameter is an optional user-supplied code that will be received on the other end and recorded (when possible) in backend analytics. Debug logging might indicate an error if the reason code out of acceptable range. The debug parameter is an optional human-readable diagnostic string that will be received on the other end and recorded (when possible) in backend analytics. If the user wishes to put the socket into a lingering state, where an attempt is made to flush any remaining sent data, the linger parameter should be enabled, otherwise reliable data is not flushed. If the connection has already ended, the reason code, debug string and linger parameter is ignored. Returns true on success or false on failure.

`NetworkingSockets.CloseListenSocket(ListenSocket socket, string remoteReason)` destroys the listen socket, and all the client sockets generated by accepting connections on the listen socket. The remote reason determines what cleanup actions are performed on the client sockets being destroyed. If cleanup is requested and the user has requested the listen socket bound to a particular local port to facilitate direct IPv4 connections, then the underlying UDP socket must remain open until all clients have been cleaned up. Returns true on success or false on failure.

`NetworkingSockets.SetConnectionUserData(Connection peer, long userData)` sets a user-supplied data for the connection. Returns true on success or false on failure.

`NetworkingSockets.GetConnectionUserData(Connection peer)` returns a user-supplied data or -1 if a handle is invalid or if any data has not be set for the connection.

`NetworkingSockets.SetConnectionName(Connection peer, string name)` sets a name for the connection, used mostly for debugging.

`NetworkingSockets.GetConnectionName(Connection peer, StringBuilder name, int maxLength)` fetches connection name to the mutable string. Returns true on success or false on failure.

`NetworkingSockets.SendMessageToConnection(Connection connection, byte[] data, int length, SendFlags flags)` sends a message to the host on the connected socket. The length and send type parameters are optional. Multiple flags can be specified at once. Returns a result described in the `Result` enumeration. Pointer `IntPtr` to a native buffer can be used instead of a reference to a byte array.

`NetworkingSockets.FlushMessagesOnConnection(Connection connection)` if the Nagle is enabled (it's enabled by default) then the message will be queued up the Nagle time before being sent, to merge small messages into the same packet. Call this function to flush any queued messages and send them immediately on the next transmission time. Returns a result described in the `Result` enumeration.

`NetworkingSockets.ReceiveMessagesOnConnection(Connection connection, NetworkingMessage[] messages, int maxMessages)` fetches the next available messages from the socket for a connection. Returns a number of messages or -1 if the connection handle is invalid. The order of the messages returned in the array is relevant. Reliable messages will be received in the order they were sent. If any messages are obtained, `message.Destroy()` should be called for each of them to free up resources.

`NetworkingSockets.GetConnectionInfo(Connection connection, ref ConnectionInfo info)` gets information about the specified connection. Returns true on success or false on failure.

`NetworkingSockets.GetQuickConnectionStatus(Connection connection, ref ConnectionStatus status)` gets a brief set of connection status that can be displayed to the user in-game. Returns true on success or false on failure.

`NetworkingSockets.GetDetailedConnectionStatus(Connection connection, StringBuilder status, int statusLength)` gets detailed connection stats in a printable form. Returns 0 on success, -1 on failure, or > 0 if a capacity of the mutable string is not enough.

`NetworkingSockets.GetListenSocketAddress(ListenSocket socket, ref Address address)` gets local IP and port number of a listen socket. Returns true on success or false on failure.

`NetworkingSockets.CreateSocketPair(Connection connectionLeft, Connection connectionRight, bool useNetworkLoopback, ref NetworkingIdentity identityLeft, ref NetworkingIdentity identityRight)` creates a pair of connections that are talking to each other e.g. a loopback communication. The two connections will be immediately placed into the connected state, and no callbacks will be called. After this, if either connection is closed, the other connection will receive a callback, exactly as if they were communicating over the network. By default, internal buffers are used, completely bypassing the network, the chopping up of messages into packets, encryption, copying the payload, etc. This means that loopback packets, by default, will not simulate lag or loss. Enabled network loopback parameter will cause the socket pair to send packets through the local network loopback device on ephemeral ports. Fake lag and loss are supported in this case, and CPU time is expended to encrypt and decrypt.

`NetworkingSockets.GetIdentity()` gets an identity associated with sockets.

`NetworkingSockets.CreatePollGroup()` creates a new poll group for connections. Returns the poll group handle.

`NetworkingSockets.DestroyPollGroup(PollGroup pollGroup)` destroys a poll group. If there are any connections in the poll group, they are removed from the group and left in a state where they are not part of any poll group. Returns false if passed an invalid poll group handle.

`NetworkingSockets.SetConnectionPollGroup(PollGroup pollGroup, Connection connection)` assigns a connection to a poll group. A connection may only belong to a single poll group. Adding a connection to a poll group implicitly removes it from any other poll group. You can pass zero value to the poll group parameter to remove a connection from its current poll group. If there are received messages currently pending on the connection, an attempt is made to add them to the queue of messages for the poll group in approximately the order that would have applied if the connection was already part of the poll group at the time that the messages were received. Returns false if the connection handle is invalid or if the poll group handle is invalid.

`NetworkingSockets.ReceiveMessagesOnPollGroup()` fetches the next available messages from the socket on any connection in the poll group. Examine `NetworkingMessage.connection` to identify connection. Delivery order of messages among different connections will usually match the order that the last packet was received which completed the message. But this is not a strong guarantee, especially for packets received right as a connection is being assigned to poll group. Delivery order of messages on the same connection is well defined and the same guarantees are present. Messages are not grouped by connection, so they will not necessarily appear consecutively in the list, they may be interleaved with messages for other connections. Returns a number of messages or -1 if the poll group handle is invalid.

`NetworkingSockets.RunCallbacks()` dispatches callbacks if available.

#### NetworkingUtils

`NetworkingUtils.Dispose()` destroys the networking utils and cleanups unmanaged resources.

`NetworkingUtils.Time` returns a current local monotonic time in microseconds. It never reset while the application remains alive.

`NetworkingUtils.FirstConfigurationValue` gets the lowest numbered configuration value available in the current environment.

`NetworkingUtils.SetStatusCallback(StatusCallback callback)` sets a callback for connection status updates. Returns true on success or false on failure. 

`NetworkingUtils.SetDebugCallback(DebugType detailLevel, DebugCallback callback)` sets a callback for debug output.

`NetworkingUtils.SetConfigurationValue(ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ConfigurationDataType dataType, IntPtr value)` sets a configuration value according to `ConfigurationValue`, `ConfigurationScope`, and `ConfigurationDataType` enumerations. The value parameter should be a reference to the actual value.

`NetworkingUtils.SetConfigurationValue(Configuration configuration, ConfigurationScope configurationScope, IntPtr scopeObject)` sets a configuration using `Configuration` structure according to `ConfigurationScope` enumeration.

`NetworkingUtils.GetConfigurationValue(ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ref ConfigurationDataType dataType, ref IntPtr result, ref IntPtr resultLength)` gets a configuration value according to `ConfigurationValue`, `ConfigurationScope`, and `ConfigurationDataType` enumerations.

#### Library
Contains constant fields.

`Library.maxCloseMessageLength` the maximum length of the reason string in bytes when a connection is closed.

`Library.maxMessageSize` the maximum size of a single message that can be sent.

`Library.Initialize(ref NetworkingIdentity identity, StringBuilder errorMessage)` initializes the native library with optional identity that will be associated with sockets. Error message parameter is optional and should be used to determine error during initialization. The capacity of a mutable string for an error message must be equal to `Library.maxErrorMessageLength`.

`Library.Deinitialize()` deinitializes the native library. Should be called after the work is done.
