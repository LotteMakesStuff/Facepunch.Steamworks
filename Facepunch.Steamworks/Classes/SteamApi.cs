using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;


namespace Steamworks
{
	internal static class SteamAPI
	{
		internal static class Native
		{
			[DllImport( Platform.LibraryName, EntryPoint = "SteamInternal_SteamAPI_Init", CallingConvention = CallingConvention.Cdecl )]
			public static extern SteamAPIInitResult SteamAPI_Init(IntPtr pszInternalCheckInterfaceVersions, IntPtr pOutErrMsg);

			[DllImport( Platform.LibraryName, EntryPoint = "SteamAPI_Shutdown", CallingConvention = CallingConvention.Cdecl )]
			public static extern void SteamAPI_Shutdown();
						
			[DllImport( Platform.LibraryName, EntryPoint = "SteamAPI_GetHSteamPipe", CallingConvention = CallingConvention.Cdecl )]
			public static extern HSteamPipe SteamAPI_GetHSteamPipe();
			
			[DllImport( Platform.LibraryName, EntryPoint = "SteamAPI_RestartAppIfNecessary", CallingConvention = CallingConvention.Cdecl )]
			[return: MarshalAs( UnmanagedType.I1 )]
			public static extern bool SteamAPI_RestartAppIfNecessary( uint unOwnAppID );
			
		}
		static internal bool Init(out string errMsg)
		{
			IntPtr SteamErrorMsgPtr = Marshal.AllocHGlobal(Defines.k_cchMaxSteamErrMsg);
			var ret = Native.SteamAPI_Init(IntPtr.Zero, SteamErrorMsgPtr);
			errMsg = PtrToStringUTF8(SteamErrorMsgPtr);
			Marshal.FreeHGlobal(SteamErrorMsgPtr);
			
			return ret == SteamAPIInitResult.OK;
			
			static string PtrToStringUTF8(IntPtr nativeUtf8) {
				if (nativeUtf8 == IntPtr.Zero) {
					return null;
				}

				int len = 0;

				while (Marshal.ReadByte(nativeUtf8, len) != 0) {
					++len;
				}

				if (len == 0) {
					return string.Empty;
				}

				byte[] buffer = new byte[len];
				Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
				return Encoding.UTF8.GetString(buffer);
			}
		}
		
		static internal void Shutdown()
		{
			Native.SteamAPI_Shutdown();
		}
				
		static internal HSteamPipe GetHSteamPipe()
		{
			return Native.SteamAPI_GetHSteamPipe();
		}
		
		static internal bool RestartAppIfNecessary( uint unOwnAppID )
		{
			return Native.SteamAPI_RestartAppIfNecessary( unOwnAppID );
		}
		
	}
}
