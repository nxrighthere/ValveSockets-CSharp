<p align="center"> 
  <img src="https://i.imgur.com/zwt2YyJ.png" alt="alt logo">
</p>

[![PayPal](https://drive.google.com/uc?id=1OQrtNBVJehNVxgPf6T6yX1wIysz1ElLR)](https://www.paypal.me/nxrighthere) [![Bountysource](https://drive.google.com/uc?id=19QRobscL8Ir2RL489IbVjcw3fULfWS_Q)](https://salt.bountysource.com/checkout/amount?team=nxrighthere)

This repository provides a managed C# wrapper for GameNetworkingSockets library which is created and maintained by Valve Software. You will need to build the native library with all required dependencies before you get started.

Usage
--------
Before starting to work, the library should be initialized using `Valve.Sockets.Library.Initialize(ref StringBuilder errorMessage);` function.

When the work is done, deinitialize the library using `Valve.Sockets.Library.Deinitialize();` function.

##### Start a new server:
```c#
NetworkingSockets server = new NetworkingSockets();

uint listenSocket = server.CreateListenSocket(port);

StatusCallback callback = (info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connecting:
			server.AcceptConnection(info.connection);

			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected - ID: " + info.connection + ", IP: " + info.remoteIP);

			break;

		case ConnectionState.ClosedByPeer:
			Console.WriteLine("Client disconnected - ID: " + info.connection + ", IP: " + info.remoteIP);

			break;
	}
};

while (!Console.KeyAvailable) {
	server.DispatchCallback(callback);

	if (server.ReceiveMessagesOnListenSocket(listenSocket, out NetworkingMessage netMessage, 1) > 0) {
		Console.WriteLine("Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
		netMessage.Destroy();
	}

	Thread.Sleep(15);
}
```

##### Start a new client:
```c#
NetworkingSockets client = new NetworkingSockets();

uint connection = client.Connect(ip, port);

StatusCallback callback = (info) => {
	switch (info.connectionInfo.state) {
		case ConnectionState.None:
			break;

		case ConnectionState.Connected:
			Console.WriteLine("Client connected - ID: " + connection);

			break;

		case ConnectionState.ClosedByPeer:
			Console.WriteLine("Client disconnected from server");

			break;

		case ConnectionState.ProblemDetectedLocally:
			Console.WriteLine("Client unable to connect");

			break;
	}
};

while (!Console.KeyAvailable) {
	server.DispatchCallback(callback);

	if (server.ReceiveMessagesOnConnection(connection, out NetworkingMessage netMessage, 1) > 0) {
		Console.WriteLine("Message received from server - Channel ID: " + netMessage.channel + ", Data length: " + netMessage.length);
		netMessage.Destroy();
	}

	Thread.Sleep(15);
}
```

##### Create and send a new message:
```c#
byte[] data = new byte[64];

sockets.SendMessageToConnection(connection, data);
```

