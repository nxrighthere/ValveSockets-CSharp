<p align="center"> 
  <img src="https://i.imgur.com/CS43fv4.png" alt="alt logo">
</p>

[![PayPal](https://drive.google.com/uc?id=1OQrtNBVJehNVxgPf6T6yX1wIysz1ElLR)](https://www.paypal.me/nxrighthere) [![Bountysource](https://drive.google.com/uc?id=19QRobscL8Ir2RL489IbVjcw3fULfWS_Q)](https://salt.bountysource.com/checkout/amount?team=nxrighthere)

This repository provides a managed C# wrapper for [GameNetworkingSockets](https://github.com/ValveSoftware/GameNetworkingSockets) library which is created and maintained by [Valve Software](https://www.valvesoftware.com). You will need to [build](https://github.com/ValveSoftware/GameNetworkingSockets#building) the native library with all required dependencies before you get started.

Usage
--------
Before starting to work, the library should be initialized using `Valve.Sockets.Library.Initialize(StringBuilder errorMessage);` function.

When the work is done, deinitialize the library using `Valve.Sockets.Library.Deinitialize();` function.

### .NET environment
##### Start a new server:
```c#
NetworkingSockets server = new NetworkingSockets();

uint listenSocket = server.CreateListenSocket(port);

StatusCallback status = (info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connecting:
			server.AcceptConnection(info.connection);
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.remoteIP.ParseIP());
			break;

		case ConnectionState.ClosedByPeer:
			server.CloseConnection(info.connection);
			Console.WriteLine("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.remoteIP.ParseIP());
			break;
	}
};

NetworkingMessage[] netMessages = new NetworkingMessage[1];

while (!Console.KeyAvailable) {
	server.DispatchCallback(status);

	int netMessagesCount = server.ReceiveMessagesOnListenSocket(listenSocket, ref netMessages, 1);

	if (netMessagesCount > 0) {
		Console.WriteLine("Message received from - ID: " + netMessages[0].connection + ", Channel ID: " + netMessages[0].channel + ", Data length: " + netMessages[0].length);
		netMessages[0].Destroy();
	}

	Thread.Sleep(15);
}
```

##### Start a new client:
```c#
NetworkingSockets client = new NetworkingSockets();

uint connection = client.Connect(ip, port);

StatusCallback status = (info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected to server - ID: " + connection);
			break;

		case ConnectionState.ClosedByPeer:
			client.CloseConnection(connection);
			Console.WriteLine("Client disconnected from server");
			break;

		case ConnectionState.ProblemDetectedLocally:
			client.CloseConnection(connection);
			Console.WriteLine("Client unable to connect");
			break;
	}
};

NetworkingMessage[] netMessages = new NetworkingMessage[1];

while (!Console.KeyAvailable) {
	client.DispatchCallback(status);

	int netMessagesCount = client.ReceiveMessagesOnConnection(connection, ref netMessages, 1);

	if (netMessagesCount > 0) {
		Console.WriteLine("Message received from server - Channel ID: " + netMessages[0].channel + ", Data length: " + netMessages[0].length);
		netMessages[0].Destroy();
	}

	Thread.Sleep(15);
}
```

##### Create and send a new message:
```c#
byte[] data = new byte[64];

sockets.SendMessageToConnection(connection, data);
```

##### Copy payload from a message:
```c#
byte[] buffer = new byte[1024];

netMessages[0].CopyTo(buffer);
```

##### Set a custom configuration:
```c#
sockets.SetConfigurationValue(ConfigurationValue.FakePacketLagSend, 80);
sockets.SetConfigurationValue(ConfigurationValue.FakePacketLossSend, 25);
sockets.SetConfigurationValue(ConfigurationValue.FakePacketReorderSend, 25);
sockets.SetConfigurationValue(ConfigurationValue.TimeoutSecondsInitial, 30);
sockets.SetConfigurationValue(ConfigurationValue.TimeoutSecondsConnected, 60);
```

##### Set a hook for debug information:
```c#
DebugCallback debug = (type, message) => {
	Console.WriteLine("Debug - Type: " + type + ", Message: " + message);
};

Library.SetDebugCallback(10, debug);
```

### Unity
Usage is almost the same as in the .NET environment, except that the console functions must be replaced with functions provided by Unity. If the `NetworkingSockets.DispatchCallback()` will be called in a game loop, then keep Unity run in background by enabling the appropriate option in the player settings.

API reference
--------
### Enumerations
#### SendType
Definitions of a flags for `NetworkingSockets.SendMessageToConnection()` function:

`SendType.Unreliable` unreliable, delivery of message is not guaranteed.

`SendType.Reliable` reliable, a message must be received by the target connection and resend attempts should be made until the message is delivered.

`SendType.NoNagle` a message will not be grouped with other messages within a timer.

`SendType.NoDelay` a message will not be buffered if it cannot be sent relatively quickly.

#### ConnectionState
Definitions of connection states for `ConnectionInfo.state` field:

`ConnectionState.None` dummy state, the connection doesn't exist or has already been closed.

`ConnectionState.Connecting` in-progress of establishing a connection initiated by `NetworkingSockets.Connect()` function.

`ConnectionState.FindingRoute` if the server accepts the connection, then this connection switch into the rendezvous state, but the end-to-end route is still have not yet established (through the relay network).

`ConnectionState.Connected` a connection request initiated by `NetworkingSockets.Connect()` function has completed.

`ConnectionState.ClosedByPeer` a connection has been closed by the peer, but not closed locally. If there are any messages in the inbound queue, they can be retrieved. Otherwise, nothing may be done with the connection except to close it. The connection still exists from an API perspective and must be closed to free up resources.

`ConnectionState.ProblemDetectedLocally` a disruption in the connection has been detected locally. Attempts to send further messages will fail. Any remaining received messages in the queue are available. The connection still exists from an API perspective and must be closed to free up resources.

#### ConfigurationString
Definitions of configuration strings for appropriate configuration functions: 

`ConfigurationString.ClientForceRelayCluster` code of relay cluster to use. If not empty, only relays in that cluster will be used.

`ConfigurationString.ClientDebugTicketAddress` generate an unsigned ticket for debugging, using the specified gameserver address. Router must be configured to accept unsigned tickets.

`ConfigurationString.ClientForceProxyAddr` comma-separated list for debugging, to override relays from the config with this set. 

#### ConfigurationValue
Definitions of configuration values for appropriate configuration functions: 

`ConfigurationValue.FakeMessageLossSend` randomly discard unreliable messages instead of sending. Expected value 0-100.

`ConfigurationValue.FakeMessageLossRecv` randomly discard unreliable messages upon receive. Expected value 0-100.

`ConfigurationValue.FakePacketLossSend` randomly discard packets instead of sending. Expected value 0-100.

`ConfigurationValue.FakePacketLossRecv` randomly discard packets upon receive. Expected value 0-100.

`ConfigurationValue.FakePacketReorderSend` globally reorder outbound packets by N percentage. Expected value 0-100.

`ConfigurationValue.FakePacketReorderRecv` globally reorder inbound packets by N percentage. Expected value 0-100.

`ConfigurationValue.FakePacketLagSend` globally delay all outbound packets by N milliseconds before sending.

`ConfigurationValue.FakePacketLagRecv` globally delay all inbound packets by N milliseconds before processing.

`ConfigurationValue.FakePacketReorderTime` amount of delay in milliseconds, to apply packets reordering.

`ConfigurationValue.SendBufferSize` upper limit of buffered pending bytes to be sent. If this limit is reached, then `NetworkingSockets.SendMessageToConnection()` function will return `Result.LimitExceeded`. Default is 524,288 bytes.

`ConfigurationValue.MaxRate` maximum send rate clamp. This value will control the maximum allowed sending rate that congestion is allowed to reach. Default is 0 which means no limit.

`ConfigurationValue.MinRate` minimum send rate clamp. This value will control the minimum allowed sending rate that congestion is allowed to reach. Default is 0 which means no limit.

`ConfigurationValue.NagleTime` set the nagle timer in microseconds.  When `NetworkingSockets.SendMessageToConnection()` is called, if the outgoing message is less than the size of the MTU, it will be queued for a delay equal to the Nagle timer value. This is to ensure that if the application sends several small messages rapidly, they have coalesced into a single packet. See historical RFC 896. Default is 5000 microseconds.

`ConfigurationValue.LogLevelAckRTT` 

`ConfigurationValue.LogLevelPacket` 

`ConfigurationValue.LogLevelMessage` 

`ConfigurationValue.LogLevelPacketGaps` 

`ConfigurationValue.LogLevelP2PRendezvous` 

`ConfigurationValue.LogLevelRelayPings` 

`ConfigurationValue.ClientConsecutitivePingTimeoutsFailInitial` 

`ConfigurationValue.ClientConsecutitivePingTimeoutsFail` 

`ConfigurationValue.ClientMinPingsBeforePingAccurate` 

`ConfigurationValue.ClientSingleSocket` 

`ConfigurationValue.IPAllowWithoutAuth` 

`ConfigurationValue.TimeoutSecondsInitial` 

`ConfigurationValue.TimeoutSecondsConnected` 

#### Result
Definitions of operation result for appropriate functions: 

