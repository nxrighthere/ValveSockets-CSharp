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
		NoDelay = 1 << 2,
		Reliable = 1 << 3
	}

	public enum IdentityType {
		Invalid = 0,
		IPAddress = 1,
		GenericString = 2,
		GenericBytes = 3,
		SteamID = 16
	}

	public enum ConnectionState {
		None = 0,
		Connecting = 1,
		FindingRoute = 2,
		Connected = 3,
		ClosedByPeer = 4,
		ProblemDetectedLocally = 5
	}

	public enum ConfigurationScope {
		Global = 1,
		SocketsInterface = 2,
		ListenSocket = 3,
		Connection = 4
	}

	public enum ConfigurationDataType {
		Int32 = 1,
		Int64 = 2,
		Float = 3,
		String = 4,
		FunctionPtr = 5
	}

	public enum ConfigurationValue {
		Invalid = 0,
		FakePacketLossSend = 2,
		FakePacketLossRecv = 3,
		FakePacketLagSend = 4,
		FakePacketLagRecv = 5,
		FakePacketReorderSend = 6,
		FakePacketReorderRecv = 7,
		FakePacketReorderTime = 8,
		FakePacketDupSend = 26,
		FakePacketDupRecv = 27,
		FakePacketDupTimeMax = 28,
		TimeoutInitial = 24,
		TimeoutConnected = 25,
		SendBufferSize = 9,
		SendRateMin = 10,
		SendRateMax = 11,
		NagleTime = 12,
		IPAllowWithoutAuth = 23,
		SDRClientConsecutitivePingTimeoutsFailInitial = 19,
		SDRClientConsecutitivePingTimeoutsFail = 20,
		SDRClientMinPingsBeforePingAccurate = 21,
		SDRClientSingleSocket = 22,
		SDRClientForceRelayCluster = 29,
		SDRClientDebugTicketAddress = 30,
		SDRClientForceProxyAddr = 31,
		LogLevelAckRTT = 13,
		LogLevelPacketDecode = 14,
		LogLevelMessage = 15,
		LogLevelPacketGaps = 16,
		LogLevelP2PRendezvous = 17,
		LogLevelSDRRelayPings = 18
	}

	public enum ConfigurationValueResult {
		BadValue = -1,
		BadScopeObject = -2,
		BufferTooSmall = -3,
		OK = 1,
		OKInherited = 2
	}

	public enum DebugType {
		None = 0,
		Bug = 1,
		Error = 2,
		Important = 3,
		Warning = 4,
		Message = 5,
		Verbose = 6,
		Debug = 7,
		Everything = 8
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
	public struct Address {
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ip;
		public ushort port;

		public bool IsLocalHost {
			get {
				return Native.SteamAPI_SteamNetworkingIPAddr_IsLocalHost(this);
			}
		}

		public string GetIP() {
			return ip.ParseIP();
		}

		public void SetLocalHost(ushort port) {
			Native.SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref this, port);
		}

		public void SetAddress(string ip, ushort port) {
			if (!ip.Contains(":"))
				Native.SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref this, ip.ParseIPv4(), port);
			else
				Native.SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref this, ip.ParseIPv6(), port);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct StatusInfo {
		private const int callback = Library.socketsCallbacks + 1;
		public Connection connection;
		public ConnectionInfo connectionInfo;
		private int socketState;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectionInfo {
		public NetworkingIdentity identity;
		public long userData;
		public ListenSocket listenSocket;
		public Address address;
		private ushort pad;
		private uint popRemote;
		private uint popRelay;
		public ConnectionState state;
		public int endReason;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string endDebug;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string connectionDescription;
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

	[StructLayout(LayoutKind.Explicit, Size = 136)]
	public struct NetworkingIdentity {
		[FieldOffset(0)]
		public IdentityType type;

		public bool IsInvalid {
			get {
				return Native.SteamAPI_SteamNetworkingIdentity_IsInvalid(this);
			}
		}

		public ulong GetSteamID() {
			return Native.SteamAPI_SteamNetworkingIdentity_GetSteamID64(this);
		}

		public void SetSteamID(ulong steamID) {
			Native.SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref this, steamID);
		}

		public bool EqualsTo(NetworkingIdentity identity) {
			return Native.SteamAPI_SteamNetworkingIdentity_EqualTo(this, identity);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NetworkingMessage {
		public IntPtr data;
		public int length;
		public Connection connection;
		public NetworkingIdentity identity;
		public long userData;
		public Microseconds timeReceived;
		public long messageNumber;
		internal IntPtr release;
		public int channel;
		private int pad;

		public void CopyTo(byte[] destination) {
			if (destination == null)
				throw new ArgumentNullException("destination");

			Marshal.Copy(data, destination, 0, length);
		}

		public void Destroy() {
			if (release == IntPtr.Zero)
				throw new InvalidOperationException("Message not created");

			Native.SteamAPI_SteamNetworkingMessage_t_Release(release);
		}
	}

	public delegate void StatusCallback(StatusInfo info, IntPtr context);
	public delegate void DebugCallback(DebugType type, string message);

	internal static class ArrayPool {
		[ThreadStatic]
		private static IntPtr[] pointerBuffer;

		public static IntPtr[] GetPointerBuffer() {
			if (pointerBuffer == null)
				pointerBuffer = new IntPtr[Library.maxMessagesPerBatch];

			return pointerBuffer;
		}
	}

	public class NetworkingSockets {
		private IntPtr nativeSockets;
		private readonly int nativeMessageSize = Marshal.SizeOf(typeof(NetworkingMessage));

		public NetworkingSockets() {
			nativeSockets = Native.SteamNetworkingSockets();

			if (nativeSockets == IntPtr.Zero)
				throw new InvalidOperationException("Networking sockets not created");
		}

		public ListenSocket CreateListenSocket(Address address) {
			return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(nativeSockets, address);
		}

		public Connection Connect(Address address) {
			return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(nativeSockets, address);
		}

		public Result AcceptConnection(Connection connection) {
			return Native.SteamAPI_ISteamNetworkingSockets_AcceptConnection(nativeSockets, connection);
		}

		public bool CloseConnection(Connection connection) {
			return CloseConnection(connection, 0, String.Empty, false);
		}

		public bool CloseConnection(Connection connection, int reason, string debug, bool enableLinger) {
			if (reason > Library.maxCloseReasonValue)
				throw new ArgumentOutOfRangeException("reason");

			if (debug.Length > Library.maxCloseMessageLength)
				throw new ArgumentOutOfRangeException("debug");

			return Native.SteamAPI_ISteamNetworkingSockets_CloseConnection(nativeSockets, connection, reason, debug, enableLinger);
		}

		public bool CloseListenSocket(ListenSocket socket) {
			return Native.SteamAPI_ISteamNetworkingSockets_CloseListenSocket(nativeSockets, socket);
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
			return SendMessageToConnection(connection, data, data.Length, SendType.Unreliable);
		}

		public Result SendMessageToConnection(Connection connection, byte[] data, SendType flags) {
			return SendMessageToConnection(connection, data, data.Length, SendType.Unreliable);
		}

		public Result SendMessageToConnection(Connection connection, byte[] data, int length, SendType flags) {
			return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, (uint)length, flags);
		}

		public Result FlushMessagesOnConnection(Connection connection) {
			return Native.SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(nativeSockets, connection);
		}

		public int ReceiveMessagesOnConnection(Connection connection, NetworkingMessage[] messages, int maxMessages) {
			if (maxMessages > Library.maxMessagesPerBatch)
				throw new ArgumentOutOfRangeException("maxMessages");

			IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
			int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(nativeSockets, connection, nativeMessages, maxMessages);

			MarshalMessages(nativeMessages, messages, messagesCount);

			return messagesCount;
		}

		public int ReceiveMessagesOnListenSocket(ListenSocket socket, NetworkingMessage[] messages, int maxMessages) {
			if (maxMessages > Library.maxMessagesPerBatch)
				throw new ArgumentOutOfRangeException("maxMessages");

			IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
			int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnListenSocket(nativeSockets, socket, nativeMessages, maxMessages);

			MarshalMessages(nativeMessages, messages, messagesCount);

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

		public bool GetListenSocketAddress(ListenSocket socket, ref Address address) {
			return Native.SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(nativeSockets, socket, ref address);
		}

		public bool CreateSocketPair(Connection connectionOne, Connection connectionTwo, bool useNetworkLoopback, NetworkingIdentity identityOne, NetworkingIdentity identityTwo) {
			return Native.SteamAPI_ISteamNetworkingSockets_CreateSocketPair(nativeSockets, connectionOne, connectionTwo, useNetworkLoopback, identityOne, identityTwo);
		}

		public void DispatchCallback(StatusCallback callback) {
			DispatchCallback(callback, IntPtr.Zero);
		}

		public void DispatchCallback(StatusCallback callback, IntPtr context) {
			Native.SteamAPI_ISteamNetworkingSockets_RunConnectionStatusChangedCallbacks(nativeSockets, callback, context);
		}

		private void MarshalMessages(IntPtr[] nativeMessages, NetworkingMessage[] messages, int messagesCount) {
			for (int i = 0; i < messagesCount; i++) {
				messages[i] = (NetworkingMessage)Marshal.PtrToStructure(nativeMessages[i], typeof(NetworkingMessage));
				messages[i].release = nativeMessages[i];
			}
		}
	}

	public class NetworkingUtils {
		private IntPtr nativeUtils;

		public NetworkingUtils() {
			nativeUtils = Native.SteamNetworkingUtils();

			if (nativeUtils == IntPtr.Zero)
				throw new InvalidOperationException("Networking utils not created");
		}

		public Microseconds Time {
			get {
				return Native.SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(nativeUtils);
			}
		}

		public ConfigurationValue FirstConfigurationValue {
			get {
				return Native.SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(nativeUtils);
			}
		}

		public void SetDebugCallback(DebugType detailLevel, DebugCallback callback) {
			Native.SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(nativeUtils, detailLevel, callback);
		}

		public bool SetConfiguratioValue(ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ConfigurationDataType dataType, IntPtr value) {
			return Native.SteamAPI_ISteamNetworkingUtils_SetConfigValue(nativeUtils, configurationValue, configurationScope, scopeObject, dataType, value);
		}

		public ConfigurationValueResult GetConfigurationValue(ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ConfigurationDataType dataType, IntPtr result, IntPtr resultLength) {
			return Native.SteamAPI_ISteamNetworkingUtils_GetConfigValue(nativeUtils, configurationValue, configurationScope, scopeObject, dataType, result, resultLength);
		}
	}

	public static class Library {
		public const int maxCloseMessageLength = 128;
		public const int maxCloseReasonValue = 999;
		public const int maxErrorMessageLength = 1024;
		public const int maxMessagesPerBatch = 256;
		public const int maxMessageSize = 512 * 1024;
		public const int socketsCallbacks = 1220;

		public static bool Initialize() {
			return Initialize(null);
		}

		public static bool Initialize(StringBuilder errorMessage) {
			if (errorMessage != null && errorMessage.Capacity != maxErrorMessageLength)
				throw new ArgumentOutOfRangeException("Capacity of the error message must be equal to " + maxErrorMessageLength);

			return Native.GameNetworkingSockets_Init(IntPtr.Zero, errorMessage);
		}

		public static void Deinitialize() {
			Native.GameNetworkingSockets_Kill();
		}
	}

	public static class Extensions {
		public static uint ParseIPv4(this string ip) {
			IPAddress address = default(IPAddress);

			if (IPAddress.TryParse(ip, out address)) {
				if (address.AddressFamily != AddressFamily.InterNetwork)
					throw new Exception("Incorrect format of an IPv4 address");
			}

			byte[] bytes = address.GetAddressBytes();

			Array.Reverse(bytes);

			return BitConverter.ToUInt32(bytes, 0);
		}

		public static byte[] ParseIPv6(this string ip) {
			IPAddress address = default(IPAddress);

			if (IPAddress.TryParse(ip, out address)) {
				if (address.AddressFamily != AddressFamily.InterNetworkV6)
					throw new Exception("Incorrect format of an IPv6 address");
			}

			return address.GetAddressBytes();
		}

		public static string ParseIP(this byte[] ip) {
			IPAddress address = new IPAddress(ip);
			string converted = address.ToString();

			if (converted.Length > 7 && converted.Remove(7) == "::ffff:") {
				Address ipv4 = default(Address);

				ipv4.ip = ip;

				byte[] bytes = BitConverter.GetBytes(Native.SteamAPI_SteamNetworkingIPAddr_GetIPv4(ipv4));

				Array.Reverse(bytes);

				address = new IPAddress(bytes);
			}

			return address.ToString();
		}
	}

	[SuppressUnmanagedCodeSecurity]
	internal static class Native {
		private const string nativeLibrary = "GameNetworkingSockets";

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool GameNetworkingSockets_Init(IntPtr identity, StringBuilder errorMessage);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GameNetworkingSockets_Kill();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamNetworkingSockets();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamNetworkingUtils();

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ListenSocket SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr sockets, Address address);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Connection SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr sockets, Address address);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_AcceptConnection(IntPtr sockets, Connection connection);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseConnection(IntPtr sockets, Connection peer, int reason, string debug, bool enableLinger);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(IntPtr sockets, ListenSocket socket);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(IntPtr sockets, Connection peer, long userData);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern long SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(IntPtr sockets, Connection peer);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ISteamNetworkingSockets_SetConnectionName(IntPtr sockets, Connection peer, string name);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(IntPtr sockets, Connection peer, StringBuilder name, int maxLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr sockets, Connection connection, byte[] data, uint length, SendType flags);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Result SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(IntPtr sockets, Connection connection);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(IntPtr sockets, Connection connection, IntPtr[] messages, int maxMessages);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnListenSocket(IntPtr sockets, ListenSocket socket, IntPtr[] messages, int maxMessages);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(IntPtr sockets, Connection connection, ref ConnectionInfo info);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(IntPtr sockets, Connection connection, ConnectionStatus status);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(IntPtr sockets, Connection connection, StringBuilder status, int statusLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(IntPtr sockets, ListenSocket socket, ref Address address);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ISteamNetworkingSockets_RunConnectionStatusChangedCallbacks(IntPtr sockets, StatusCallback callback, IntPtr context);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(IntPtr sockets, Connection connectionOne, Connection connectionTwo, bool useNetworkLoopback, NetworkingIdentity identityOne, NetworkingIdentity identityTwo);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref Address address, byte[] ip, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref Address address, uint ip, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(Address address);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref Address address, ushort port);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(Address address);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_SteamNetworkingIdentity_IsInvalid(NetworkingIdentity identity);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref NetworkingIdentity identity, ulong steamID);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(NetworkingIdentity identity);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_SteamNetworkingIdentity_EqualTo(NetworkingIdentity identityOne, NetworkingIdentity identityTwo);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Microseconds SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr utils);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr utils, DebugType detailLevel, DebugCallback callback);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(IntPtr utils, ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ConfigurationDataType dataType, IntPtr value);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ConfigurationValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(IntPtr utils, ConfigurationValue configurationValue, ConfigurationScope configurationScope, IntPtr scopeObject, ConfigurationDataType dataType, IntPtr result, IntPtr resultLength);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(IntPtr utils, ConfigurationValue configurationValue, IntPtr outName, ConfigurationDataType dataType, ConfigurationScope configurationScope, ConfigurationValue outNextValue);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ConfigurationValue SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(IntPtr utils);

		[DllImport(nativeLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SteamAPI_SteamNetworkingMessage_t_Release(IntPtr nativeMessage);
	}
}
