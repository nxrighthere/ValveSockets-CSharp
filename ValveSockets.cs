/*
 *  Managed C# wrapper for GameNetworkingSockets library by Valve Software
 *  Copyright (c) 2018 Stanislav Denisov
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Valve.Sockets {
	using ListenSocket = UInt32;
	using Connection = UInt32;
	using Microseconds = Int64;

	[Flags]
	public enum SendType {
		Unreliable = 0,
		NoNagle = 1 << 0,
		NoDelay = 1 << 1,
		Reliable = 1 << 3
	}

	public enum ConnectionState {
		None = 0,
		Connecting = 1,
		FindingRoute = 2,
		Connected = 3,
		ClosedByPeer = 4,
		ProblemDetectedLocally = 5
	}

	public enum ConfigurationString {
		ClientForceRelayCluster = 0,
		ClientDebugTicketAddress = 1,
		ClientForceProxyAddr = 2
	}

	public enum ConfigurationValue {
		FakeMessageLossSend = 0,
		FakeMessageLossRecv = 1,
		FakePacketLossSend = 2,
		FakePacketLossRecv = 3,
		FakePacketLagSend = 4,
		FakePacketLagRecv = 5,
		FakePacketReorderSend = 6,
		FakePacketReorderRecv = 7,
		FakePacketReorderTime = 8,
		SendBufferSize = 9,
		MaxRate = 10,
		MinRate = 11,
		NagleTime = 12,
		LogLevelAckRTT = 13,
		LogLevelPacket = 14,
		LogLevelMessage = 15,
		LogLevelPacketGaps = 16,
		LogLevelP2PRendezvous = 17,
		LogLevelRelayPings = 18,
		ClientConsecutitivePingTimeoutsFailInitial = 19,
		ClientConsecutitivePingTimeoutsFail = 20,
		ClientMinPingsBeforePingAccurate = 21,
		ClientSingleSocket = 22,
		IPAllowWithoutAuth = 23,
		TimeoutSecondsInitial = 24,
		TimeoutSecondsConnected = 25
	}

	public enum Result {
		OK = 1,
		Fail = 2,
		NoConnection = 3,
		InvalidPassword = 5,
		LoggedInElsewhere = 6,
		InvalidProtocolVer = 7,
		InvalidParam = 8,
		FileNotFound = 9,
		Busy = 10,
		InvalidState = 11,
		InvalidName = 12,
		InvalidEmail = 13,
		DuplicateName = 14,
		AccessDenied = 15,
		Timeout = 16,
		Banned = 17,
		AccountNotFound = 18,
		InvalidSteamID = 19,
		ServiceUnavailable = 20,
		NotLoggedOn = 21,
		Pending = 22,
		EncryptionFailure = 23,
		InsufficientPrivilege = 24,
		LimitExceeded = 25,
		Revoked = 26,
		Expired = 27,
		AlreadyRedeemed = 28,
		DuplicateRequest = 29,
		AlreadyOwned = 30,
		IPNotFound = 31,
		PersistFailed = 32,
		LockingFailed = 33,
		LogonSessionReplaced = 34,
		ConnectFailed = 35,
		HandshakeFailed = 36,
		IOFailure = 37,
		RemoteDisconnect = 38,
		ShoppingCartNotFound = 39,
		Blocked = 40,
		Ignored = 41,
		NoMatch = 42,
		AccountDisabled = 43,
		ServiceReadOnly = 44,
		AccountNotFeatured = 45,
		AdministratorOK = 46,
		ContentVersion = 47,
		TryAnotherCM = 48,
		PasswordRequiredToKickSession = 49,
		AlreadyLoggedInElsewhere = 50,
		Suspended = 51,
		Cancelled = 52,
		DataCorruption = 53,
		DiskFull = 54,
		RemoteCallFailed = 55,
		PasswordUnset = 56,
		ExternalAccountUnlinked = 57,
		PSNTicketInvalid = 58,
		ExternalAccountAlreadyLinked = 59,
		RemoteFileConflict = 60,
		IllegalPassword = 61,
		SameAsPreviousValue = 62,
		AccountLogonDenied = 63,
		CannotUseOldPassword = 64,
		InvalidLoginAuthCode = 65,
		AccountLogonDeniedNoMail = 66,
		HardwareNotCapableOfIPT = 67,
		IPTInitError = 68,
		ParentalControlRestricted = 69,
		FacebookQueryError = 70,
		ExpiredLoginAuthCode = 71,
		IPLoginRestrictionFailed = 72,
		AccountLockedDown = 73,
		AccountLogonDeniedVerifiedEmailRequired = 74,
		NoMatchingURL = 75,
		BadResponse = 76,
		RequirePasswordReEntry = 77,
		ValueOutOfRange = 78,
		UnexpectedError = 79,
		Disabled = 80,
		InvalidCEGSubmission = 81,
		RestrictedDevice = 82,
		RegionLocked = 83,
		RateLimitExceeded = 84,
		AccountLoginDeniedNeedTwoFactor = 85,
		ItemDeleted = 86,
		AccountLoginDeniedThrottle = 87,
		TwoFactorCodeMismatch = 88,
		TwoFactorActivationCodeMismatch = 89,
		AccountAssociatedToMultiplePartners = 90,
		NotModified = 91,
		NoMobileDevice = 92,
		TimeNotSynced = 93,
		SmsCodeFailed = 94,
		AccountLimitExceeded = 95,
		AccountActivityLimitExceeded = 96,
		PhoneActivityLimitExceeded = 97,
		RefundToWallet = 98,
		EmailSendFailure = 99,
		NotSettled = 100,
		NeedCaptcha = 101,
		GSLTDenied = 102,
		GSOwnerDenied = 103,
		InvalidItemType = 104,
		IPBanned = 105,
		GSLTExpired = 106,
		InsufficientFunds = 107,
		TooManyPending = 108,
		NoSiteLicensesFound = 109,
		WGNetworkSendExceeded = 110
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StatusInfo {
		public const int callback = Library.networkingCallbacks + 9;
		public Connection connection;
		public ConnectionInfo connectionInfo;
		public int socketState;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectionInfo {
		public ulong steamID;
		public long userData;
		public ListenSocket listenSocket;
		public uint remoteIP;
		public ushort remotePort;
		public ushort pad1;
		public uint POPRemoteID;
		public uint POPRelayID;
		public ConnectionState state;
		public int endReason;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string endDebug;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectionStatus {
		public ConnectionState state;
		public int ping;
		public float connectionQualityLocal;
		public float connectionQualityRemote;
		public float outPacketsPerSecond;
		public float outBytesPerSecond;
		public float inPacketsPerSecond;
		public float inBytesPerSecond;
		public int sendRateBytesPerSecond;
		public int pendingUnreliable;
		public int pendingReliable;
		public int sentUnackedReliable;
		public Microseconds queueTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NetworkingMessage {
		public ulong steamID;
		public long userData;
		public Microseconds timeReceived;
		public long messageNumber;
		public IntPtr release;
		public IntPtr data;
		public int length;
		public Connection connection;
		public int channel;
		public int padDummy;
	}

	public delegate void StatusCallback(StatusInfo info);
	public delegate void DebugCallback(int type, string message);

	public class NetworkingSockets {
		private IntPtr nativeSockets;
		private readonly int nativeMessageSize = Marshal.SizeOf(typeof(NetworkingMessage));

		public NetworkingSockets() {
			nativeSockets = Native.SteamNetworkingSockets();

			if (nativeSockets == IntPtr.Zero)
				throw new InvalidOperationException("Networking sockets not created");
		}

		public ListenSocket CreateListenSocket(ushort port) {
			return CreateListenSocket("0", port);
		}

		public ListenSocket CreateListenSocket(string ip, ushort port) {
			return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocket(nativeSockets, -1, ip.ParseIP(), port);
		}

		public Connection Connect(string ip, ushort port) {
			return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPv4Address(nativeSockets, ip.ParseIP(), port);
		}

		public Result AcceptConnection(Connection connection) {
			return Native.SteamAPI_ISteamNetworkingSockets_AcceptConnection(nativeSockets, connection);
		}

		public bool CloseConnection(Connection connection) {
			return CloseConnection(connection, 0, String.Empty, false);
		}

		public bool CloseConnection(Connection connection, int reason, string debug, bool enableLinger) {
			if (reason > Library.maxCloseReasonLength)
				throw new ArgumentOutOfRangeException("reason");

			if (debug.Length > Library.maxCloseMessageLength)
				throw new ArgumentOutOfRangeException("debug");

			return Native.SteamAPI_ISteamNetworkingSockets_CloseConnection(nativeSockets, connection, reason, debug, enableLinger);
		}

		public bool CloseListenSocket(ListenSocket socket) {
			return CloseListenSocket(socket, String.Empty);
		}

		public bool CloseListenSocket(ListenSocket socket, string remoteReason) {
			if (remoteReason.Length > Library.maxCloseMessageLength)
				throw new ArgumentOutOfRangeException("remoteReason");

			return Native.SteamAPI_ISteamNetworkingSockets_CloseListenSocket(nativeSockets, socket, remoteReason);
		}

		public bool SetConnectionUserData(Connection peer, long userData) {
			return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(nativeSockets, peer, userData);
		}

		public long GetConnectionUserData(Connection peer) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(nativeSockets, peer);
		}

		public void SetConnectionName(Connection peer, string name) {
			Native.SteamAPI_ISteamNetworkingSockets_SetConnectionName(nativeSockets, peer, name);
		}

		public bool GetConnectionName(Connection peer, StringBuilder name, int maxLength) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionName(nativeSockets, peer, name, maxLength);
		}

		public Result SendMessageToConnection(Connection connection, byte[] data) {
			return SendMessageToConnection(connection, data, SendType.Unreliable);
		}

		public Result SendMessageToConnection(Connection connection, byte[] data, SendType sendType) {
			return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, (uint)data.Length, sendType);
		}

		public Result FlushMessagesOnConnection(Connection connection) {
			return Native.SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(nativeSockets, connection);
		}

		public int ReceiveMessagesOnConnection(Connection connection, ref NetworkingMessage[] messages, int maxMessages) {
			IntPtr nativeMessages = IntPtr.Zero;
			int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(nativeSockets, connection, out nativeMessages, maxMessages);
			MarshalMessages(nativeMessages, ref messages, messagesCount);

			return messagesCount;
		}

		public int ReceiveMessagesOnListenSocket(ListenSocket socket, ref NetworkingMessage[] messages, int maxMessages) {
			IntPtr nativeMessages = IntPtr.Zero;
			int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnListenSocket(nativeSockets, socket, out nativeMessages, maxMessages);
			MarshalMessages(nativeMessages, ref messages, messagesCount);

			return messagesCount;
		}

		public bool GetConnectionInfo(Connection connection, ref ConnectionInfo info) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(nativeSockets, connection, ref info);
		}

		public bool GetQuickConnectionStatus(Connection connection, ConnectionStatus status) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(nativeSockets, connection, status);
		}

		public int GetDetailedConnectionStatus(Connection connection, StringBuilder status, int statusLength) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(nativeSockets, connection, status, statusLength);
		}

		public bool GetListenSocketInfo(ListenSocket socket, uint ip, ushort port) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetListenSocketInfo(nativeSockets, socket, ip, port);
		}

		public bool CreateSocketPair(Connection connectionOne, Connection connectionTwo, bool useNetworkLoopback) {
			return Native.SteamAPI_ISteamNetworkingSockets_CreateSocketPair(nativeSockets, connectionOne, connectionTwo, useNetworkLoopback);
		}

		public bool GetConnectionDebugText(Connection connection, StringBuilder debugText, int debugLength) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionDebugText(nativeSockets, connection, debugText, debugLength);
		}

		public int GetConfigurationValue(ConfigurationValue configurationValue) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConfigurationValue(nativeSockets, configurationValue);
		}

		public bool SetConfigurationValue(ConfigurationValue configurationValue, int value) {
			return Native.SteamAPI_ISteamNetworkingSockets_SetConfigurationValue(nativeSockets, configurationValue, value);
		}

		public string GetConfigurationValueName(ConfigurationValue configurationValue) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConfigurationValueName(nativeSockets, configurationValue);
		}

		public int GetConfigurationString(ConfigurationString configurationString, StringBuilder destination, int destinationLength) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConfigurationString(nativeSockets, configurationString, destination, destinationLength);
		}

		public bool SetConfigurationString(ConfigurationString configurationString, string inputString) {
			return Native.SteamAPI_ISteamNetworkingSockets_SetConfigurationString(nativeSockets, configurationString, inputString);
		}

		public string GetConfigurationStringName(ConfigurationString configurationString) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConfigurationStringName(nativeSockets, configurationString);;
		}

		public int GetConnectionConfigurationValue(Connection connection, ConfigurationValue configurationValue) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionConfigurationValue(nativeSockets, connection, configurationValue);
		}

		public bool SetConnectionConfigurationValue(Connection connection, ConfigurationValue configurationValue, int value) {
			return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionConfigurationValue(nativeSockets, connection, configurationValue, value);
		}

		public void DispatchCallback(StatusCallback callback) {
			Native.SteamAPI_ISteamNetworkingSockets_RunConnectionStatusChangedCallbacks(nativeSockets, callback);
		}

		private void MarshalMessages(IntPtr nativeMessages, ref NetworkingMessage[] messages, int messagesCount) {
			for (int i = 0; i < messagesCount; i++) {
				messages[i] = (NetworkingMessage)Marshal.PtrToStructure(new IntPtr(nativeMessages.ToInt64() + (nativeMessageSize * i)), typeof(NetworkingMessage));
			}
		}
	}

	public static class Extensions {
		public static string ParseIP(this uint ip) {
			byte[] bytes = BitConverter.GetBytes(ip);

			Array.Reverse(bytes);

			IPAddress address = new IPAddress(bytes);

			return address.ToString();
		}

		public static uint ParseIP(this string ip) {
			IPAddress address;

			if (IPAddress.TryParse(ip, out address)) {
				if (address.AddressFamily != AddressFamily.InterNetwork)
					throw new Exception("Incorrect format of an IPv4 address");
			}

			byte[] bytes = address.GetAddressBytes();

			Array.Reverse(bytes);

			return BitConverter.ToUInt32(bytes, 0);
		}

		public static void CopyTo(this NetworkingMessage message, byte[] destination) {
			if (destination == null)
				throw new ArgumentNullException("destination");

			Marshal.Copy(message.data, destination, 0, message.length);
		}

		public static void Destroy(this NetworkingMessage message) {
			if (message.release == IntPtr.Zero)
				throw new InvalidOperationException("Message not created");

			throw new NotImplementedException("Destroy function is not provided in the flat interface on the native side");
		}
	}

	public static class Library {
		public const int maxErrorMessageLength = 1024;
		public const int maxCloseMessageLength = 128;
		public const int maxCloseReasonLength = 999;
		public const int networkingCallbacks = 1200;

		public static bool Initialize(StringBuilder errorMessage) {
			if (errorMessage.Capacity != maxErrorMessageLength)
				throw new ArgumentOutOfRangeException("Capacity of the error message must be equal to " + maxErrorMessageLength);

			return Native.GameNetworkingSockets_Init(errorMessage);
		}

		public static void Deinitialize() {
			Native.GameNetworkingSockets_Kill();
		}

		public static void SetDebugCallback(int detailLevel, DebugCallback callback) {
			Native.SteamNetworkingSockets_SetDebugOutputFunction(detailLevel, callback);
		}

		public static Microseconds Time {
			get {
				return Native.SteamNetworkingSockets_GetLocalTimestamp();
			}
		}
	}

	[SuppressUnmanagedCodeSecurity]
	internal static class Native {
		private const string nativeLibrary = "GameNetworkingSockets";

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool GameNetworkingSockets_Init(StringBuilder errorMessage);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GameNetworkingSockets_Kill();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamNetworkingSockets();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Microseconds SteamNetworkingSockets_GetLocalTimestamp();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamNetworkingSockets_SetDebugOutputFunction(int detailLevel, DebugCallback callback);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ListenSocket SteamAPI_ISteamNetworkingSockets_CreateListenSocket(IntPtr instance, int virtualPort, uint ip, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Connection SteamAPI_ISteamNetworkingSockets_ConnectByIPv4Address(IntPtr isntance, uint ip, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_AcceptConnection(IntPtr instance, Connection connection);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseConnection(IntPtr instance, Connection peer, int reason, string debug, bool enableLinger);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(IntPtr instance, ListenSocket socket, string remoteReason);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(IntPtr instance, Connection peer, long userData);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern long SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(IntPtr instance, Connection peer);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ISteamNetworkingSockets_SetConnectionName(IntPtr instance, Connection peer, string name);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(IntPtr instance, Connection peer, StringBuilder name, int maxLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr instance, Connection connection, byte[] data, uint length, SendType sendType);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(IntPtr instance, Connection connection);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(IntPtr instance, Connection connection, out IntPtr messages, int maxMessages);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnListenSocket(IntPtr instance, ListenSocket socket, out IntPtr messages, int maxMessages);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(IntPtr instance, Connection connection, ref ConnectionInfo info);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(IntPtr instance, Connection connection, ConnectionStatus status);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(IntPtr instance, Connection connection, StringBuilder status, int statusLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetListenSocketInfo(IntPtr instance, ListenSocket socket, uint ip, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(IntPtr instance, Connection connectionOne, Connection connectionTwo, bool useNetworkLoopback);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionDebugText(IntPtr instance, Connection connection, StringBuilder debugText, int debugLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_GetConfigurationValue(IntPtr instance, ConfigurationValue configurationValue);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConfigurationValue(IntPtr instance, ConfigurationValue configurationValue, int value);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern string SteamAPI_ISteamNetworkingSockets_GetConfigurationValueName(IntPtr instance, ConfigurationValue configurationValue);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_GetConfigurationString(IntPtr instance, ConfigurationString configurationString, StringBuilder destination, int destinationLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConfigurationString(IntPtr instance, ConfigurationString configurationString, string inputString);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern string SteamAPI_ISteamNetworkingSockets_GetConfigurationStringName(IntPtr instance, ConfigurationString configurationString);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_GetConnectionConfigurationValue(IntPtr instance, Connection connection, ConfigurationValue configurationValue);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConnectionConfigurationValue(IntPtr instance, Connection connection, ConfigurationValue configurationValue, int value);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ISteamNetworkingSockets_RunConnectionStatusChangedCallbacks(IntPtr isntance, StatusCallback callback);
	}
}
