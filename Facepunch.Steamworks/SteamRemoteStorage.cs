using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;
using Steamworks.Structs;

namespace Steamworks
{
	/// <summary>
	/// Class for utilizing the Steam Remote Storage API.
	/// </summary>
	public class SteamRemoteStorage : SteamClientClass<SteamRemoteStorage>
	{
		internal static ISteamRemoteStorage Internal => Interface as ISteamRemoteStorage;

		internal override bool InitializeInterface( bool server )
		{
			SetInterface( server, new ISteamRemoteStorage( server ) );
			if ( Interface.Self == IntPtr.Zero ) return false;
			
			InstallEvents();

			return true;
		}

		internal void InstallEvents()
		{
			Dispatch.Install<RemoteStorageLocalFileChange_t>( x => OnRemoteStorageLocalFileChange?.Invoke() );
		}

		/// <summary>
		/// Creates a new file, writes the bytes to the file, and then closes the file.
		/// If the target file already exists, it is overwritten
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		/// <param name="data">The bytes of data.</param>
		/// <returns>A boolean, detailing whether or not the operation was successful.</returns>
		public unsafe static bool FileWrite( string filename, byte[] data )
		{
			fixed ( byte* ptr = data )
			{
				return Internal.FileWrite( filename, (IntPtr) ptr, data.Length );
			}
		}

		/// <summary>
		/// Use this along with EndFileWriteBatch to wrap a set of local file writes/deletes that should be considered part of one single state change. For example, if saving game progress requires updating both savegame1.dat and maxprogress.dat, wrap those operations with calls to BeginFileWriteBatch and EndFileWriteBatch.
		///
		/// These functions provide a hint to Steam which will help it manage the app's Cloud files. Using these functions is optional, however it will provide better reliability.
		///
		/// Note that the functions may be used whether the writes are done using the ISteamRemoteStorage API, or done directly to local disk (where AutoCloud is used).
		/// </summary>
		/// <returns>Returns true if a new write batch has been started, or false if there was a batch already in progress</returns>
		public static bool BeginFileWriteBatch()
			=> Internal.BeginFileWriteBatch();
		
		/// <summary>
		/// Use this along with BeginFileWriteBatch
		/// </summary>
		/// <returns>Returns true if the write batch was ended, false if there was no batch already in progress.</returns>
		public static bool EndFileWriteBatch()
			=> Internal.EndFileWriteBatch();

		/// <summary>
		/// Opens a binary file, reads the contents of the file into a byte array, and then closes the file.
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		public unsafe static byte[] FileRead( string filename )
		{
			var size = FileSize( filename );
			if ( size <= 0 ) return null;
			var buffer = new byte[size];

			fixed ( byte* ptr = buffer )
			{
				var readsize = Internal.FileRead( filename, (IntPtr)ptr, size );
				if ( readsize != size )
				{
					return null;
				}
				return buffer;
			}
		}

		/// <summary>
		/// Checks whether the specified file exists.
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		/// <returns>Whether or not the file exists.</returns>
		public static bool FileExists( string filename ) => Internal.FileExists( filename );

		/// <summary>
		/// Checks if a specific file is persisted in the steam cloud.
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		/// <returns>Boolean.</returns>
		public static bool FilePersisted( string filename ) => Internal.FilePersisted( filename );

		/// <summary>
		/// Gets the specified file's last modified date/time.
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		/// <returns>A <see cref="DateTime"/> describing when the file was modified last.</returns>
		public static DateTime FileTime( string filename ) => Epoch.ToDateTime( Internal.GetFileTimestamp( filename ) );

		/// <summary>
		/// Returns the specified files size in bytes, or <c>0</c> if the file does not exist.
		/// </summary>
		/// <param name="filename">The path of the file.</param>
		/// <returns>The size of the file in bytes, or <c>0</c> if the file doesn't exist.</returns>
		public static int FileSize( string filename ) => Internal.GetFileSize( filename );

		/// <summary>
		/// Deletes the file from remote storage, but leaves it on the local disk and remains accessible from the API.
		/// </summary>
		/// <returns>A boolean, detailing whether or not the operation was successful.</returns>
		public static bool FileForget( string filename ) => Internal.FileForget( filename );

		/// <summary>
		/// Deletes a file from the local disk, and propagates that delete to the cloud.
		/// </summary>
		public static bool FileDelete( string filename ) => Internal.FileDelete( filename );


		/// <summary>
		/// Gets the total number of quota bytes.
		/// </summary>
		public static ulong QuotaBytes
		{
			get
			{
				ulong t = 0, a = 0;
				Internal.GetQuota( ref t, ref a );
				return t;
			}
		}

		/// <summary>
		/// Gets the total number of quota bytes that have been used.
		/// </summary>
		public static ulong QuotaUsedBytes
		{
			get
			{
				ulong t = 0, a = 0;
				Internal.GetQuota( ref t, ref a );
				return t - a;
			}
		}

		/// <summary>
		/// Number of bytes remaining until the quota is used.
		/// </summary>
		public static ulong QuotaRemainingBytes
		{
			get
			{
				ulong t = 0, a = 0;
				Internal.GetQuota( ref t, ref a );
				return a;
			}
		}

		/// <summary>
		/// returns <see langword="true"/> if <see cref="IsCloudEnabledForAccount"/> AND <see cref="IsCloudEnabledForApp"/> are <see langword="true"/>.
		/// </summary>
		public static bool IsCloudEnabled => IsCloudEnabledForAccount && IsCloudEnabledForApp;

		/// <summary>
		/// Checks if the account wide Steam Cloud setting is enabled for this user
		/// or if they disabled it in the Settings->Cloud dialog.
		/// </summary>
		public static bool IsCloudEnabledForAccount => Internal.IsCloudEnabledForAccount();

		/// <summary>
		/// Checks if the per game Steam Cloud setting is enabled for this user
		/// or if they disabled it in the Game Properties->Update dialog.
		/// 
		/// This must only ever be set as the direct result of the user explicitly 
		/// requesting that it's enabled or not. This is typically accomplished with 
		/// a checkbox within your in-game options
		/// </summary>
		public static bool IsCloudEnabledForApp
		{
			get => Internal.IsCloudEnabledForApp();
			set => Internal.SetCloudEnabledForApp( value );
		}

		/// <summary>
		/// Gets the total number of local files synchronized by Steam Cloud.
		/// </summary>
		public static int FileCount => Internal.GetFileCount();

		/// <summary>
		/// Gets a list of filenames synchronized by Steam Cloud.
		/// </summary>
		public static IEnumerable<string> Files
		{
			get
			{
				int _ = 0;
				for( int i=0; i<FileCount; i++ )
				{
					var filename = Internal.GetFileNameAndSize( i, ref _ );
					yield return filename;
				}
			}
		}

		/// <summary>
		/// If a Steam app is flagged for supporting dynamic Steam Cloud sync, and a sync occurs, this callback will be posted to the app if any local files changed.
		/// </summary>
		public static event Action OnRemoteStorageLocalFileChange;

		/// <summary>
		/// When your application receives a RemoteStorageLocalFileChange, use this method to get the number of changes (file updates and file deletes) that have been made. You can then iterate the changes using GetLocalFileChange.
		///
		/// Note: only applies to applications flagged as supporting dynamic Steam Cloud sync.
		/// </summary>
		/// <returns></returns>
		public static int GetLocalFileChangeCount()
		{
			return Internal.GetLocalFileChangeCount();
		}

		/// <summary>
		/// After calling GetLocalFileChangeCount, use this method to iterate over the changes. The changes described have already been made to local files. Your application should take appropriate action to reload state from disk, and possibly notify the user.
		///
		/// For example: The local system had been suspended, during which time the user played elsewhere and uploaded changes to the Steam Cloud. On resume, Steam downloads those changes to the local system before resuming the application. The application receives an RemoteStorageLocalFileChange, and uses GetLocalFileChangeCount and GetLocalFileChange to iterate those changes. Depending on the application structure and the nature of the changes, the application could:
		///  - Re-load game progress to resume at exactly the point where the user was when they exited the game on the other device
		///  - Notify the user of any synchronized changes that don't require reloading
		///  - etc
		/// 
		/// Note: only applies to applications flagged as supporting dynamic Steam Cloud sync.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static FileChange GetLocalFileChange( int index )
		{
			RemoteStorageLocalFileChange remoteStorageLocalFileChange = RemoteStorageLocalFileChange.Invalid;
			RemoteStorageFilePathType remoteStorageFilePathType = RemoteStorageFilePathType.Invalid;
			string path = Internal.GetLocalFileChange( index, ref remoteStorageLocalFileChange, ref remoteStorageFilePathType );

			return new FileChange()
			{
				Filename = path,
				Updated = remoteStorageLocalFileChange == RemoteStorageLocalFileChange.FileUpdated,
				Deleted = remoteStorageLocalFileChange == RemoteStorageLocalFileChange.FileDeleted,
				AbsolutePath = remoteStorageFilePathType == RemoteStorageFilePathType.Absolute,
				CloudPath = remoteStorageFilePathType == RemoteStorageFilePathType.APIFilename
			};
		}
	}
}
