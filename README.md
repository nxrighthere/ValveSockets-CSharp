<p align="center"> 
  <img src="https://i.imgur.com/CS43fv4.png" alt="alt logo">
</p>

[![PayPal](https://drive.google.com/uc?id=1OQrtNBVJehNVxgPf6T6yX1wIysz1ElLR)](https://www.paypal.me/nxrighthere) [![Bountysource](https://drive.google.com/uc?id=19QRobscL8Ir2RL489IbVjcw3fULfWS_Q)](https://salt.bountysource.com/checkout/amount?team=nxrighthere) [![Coinbase](https://drive.google.com/uc?id=1LckuF-IAod6xmO9yF-jhTjq1m-4f7cgF)](https://commerce.coinbase.com/checkout/03e11816-b6fc-4e14-b974-29a1d0886697) [![Discord](https://discordapp.com/api/guilds/515987760281288707/embed.png)](https://discord.gg/ceaWXVw)

This repository provides a managed C# wrapper for [GameNetworkingSockets](https://github.com/ValveSoftware/GameNetworkingSockets) library which is created and maintained by [Valve Software](https://www.valvesoftware.com). You will need to [build](https://github.com/ValveSoftware/GameNetworkingSockets#building) the native library with all required dependencies before you get started.

Usage
--------
Before starting to work, the library should be initialized using `Valve.Sockets.Library.Initialize(StringBuilder errorMessage);` function.

After the work is done, deinitialize the library using `Valve.Sockets.Library.Deinitialize();` function.

### .NET environment
##### Start a new server:
```c#
NetworkingSockets server = new NetworkingSockets();
Address address = new Address();

address.SetIPv6("::0", port);

uint listenSocket = server.CreateListenSocket(address);

StatusCallback status = (info, context) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connecting:
			server.AcceptConnection(info.connection);
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.ip.ParseIP());
			break;

		case ConnectionState.ClosedByPeer:
			server.CloseConnection(info.connection);
			Console.WriteLine("Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.ip.ParseIP());
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
Address address = new Address();

address.SetIPv6("::1", port);

uint connection = client.Connect(address);

StatusCallback status = (info, context) => {
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
