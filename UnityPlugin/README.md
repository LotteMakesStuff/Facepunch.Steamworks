# LOTTE'S NOTES
Facepunch.Steamworks is probably the best steam wrapper to work with in C# - unfortunatly its not recieveing updates anymore in the mainline repository (the last official release was in 2020!). over the last 4 years ive regularly had to download the dang git repository, remember how it all works and build an updated version just before shipping some Unity game to steam... This time i figured WHY NOT COMMIT A FORK. Maybe ill save you an hours work.

## So whats in this fork?
✔ Built against [Steamworks 1.61](https://partner.steamgames.com/downloads/list)

✔ Exposes all the new [Steam Timeline](https://partner.steamgames.com/doc/features/timeline) stuff needed to have full [Game Recording](https://store.steampowered.com/gamerecording) support in a game

✔ Added support for [Dynamic Cloud Sync](https://steamcommunity.com/groups/steamworks/announcements/detail/3142949576401813670)

✔ Added support for Steam [beta branch querying and switching](https://store.steampowered.com/news/group/4145017/view/4547039255696769966)

✔ Adds a Unity package that can be easily installed with Unity Package Manager, with prebuilt 64bit .Net Standard 2.1 assemblies for Windows, OSX and Steamdeck


## CHANGELOG
### 1.61.0
- Updated Steam Timeline implementation to support the new API design shipped in Steamworks 1.61
    - ```SetTimelineStateDescription``` has been replaced with ```SetTimelineTooltip```
    - ```ClearTimelineStateDescription``` has been replaced with ```ClearTimelineTooltip```
    - ```AddTimelineEvent``` has been replaced with ```TimelineEvents.AddInstantEvent``` and ```TimelineEvents.AddRangedEvent```
- Added support beta branch APIs in SteamApps
- Added``` DismissGamepadTextInput``` to SteamUtils
- Added ```BeginFileWriteBatch``` & ```EndFileWriteBatch``` in SteamRemoteStorage
- Added support for Dynamic Cloud Sync in SteamRemoteStorage
- Removed ```RequestCurrentStats``` from SteamUserStats as it is now obsolete 
### 1.60.5
- Fixed linux editor issues
### 1.60.3
- Removed old steamworks DLL that was in the root folder for some reason
### 1.60.2
- Rebuilt DLLS for all platforms
### 1.60.1
- Fixed issues with ```SteamClient.Init()``` binding
- Exposed an Achievements's hidden state via ```achievement.IsHidden```
### 1.60.0
- Added bindning for Steam Timeline, accessable via ```Steamworks.SteamTimeline``` 

---



# Facepunch.Steamworks

[Another fucking c# Steamworks implementation](https://wiki.facepunch.com/steamworks/)

![Build All](https://github.com/Crytilis/Facepunch.Steamworks/workflows/Build%20All/badge.svg)

## Features

| Feature | Supported |
|----------|------------ |
| Windows | ✔ |
| Linux | ✔ |
| MacOS | ✔ |
| Unity Support | ✔ |
| Unity IL2CPP Support | ✔ |
| Async Callbacks (steam callresults) | ✔ |
| Events (steam callbacks) | ✔ |
| Single C# dll (no native requirements apart from Steam) | ✔ |
| Open Source | ✔ |
| MIT license | ✔ |
| Any 32bit OS | ✔  |

## Why

The Steamworks C# implementations I found that were compatible with Unity have worked for a long time. But I hate them all. For a number of different reasons.

* They're not C#, they're just a collection of functions.
* They're not up to date.
* They require a 3rd party native dll.
* They can't be compiled into a standalone dll (in Unity).
* They're not free
* They have a restrictive license.

C# is meant to make things easier. So lets try to wrap it up in a way that makes it all easier.

## What

### Get your own information

```csharp
    SteamClient.SteamId // Your SteamId
    SteamClient.Name // Your Name
```

### View your friends list

```csharp
foreach ( var friend in SteamFriends.GetFriends() )
{
    Console.WriteLine( $"{friend.Id}: {friend.Name}" );
    Console.WriteLine( $"{friend.IsOnline} / {friend.SteamLevel}" );
    
    friend.SendMessage( "Hello Friend" );
}
```


### App Info

```csharp
    Console.WriteLine( SteamApps.GameLanguage ); // Print the current game language
    var installDir = SteamApps.AppInstallDir( 4000 ); // Get the path to the Garry's Mod install folder

    var fileinfo = await SteamApps.GetFileDetailsAsync( "hl2.exe" ); // async get file details
    DoSomething( fileinfo.SizeInBytes, fileinfo.Sha1 );
```

### Get Avatars

```csharp
    var image = await SteamFriends.GetLargeAvatarAsync( steamid );
    if ( !image.HasValue ) return DefaultImage;

    return MakeTextureFromRGBA( image.Value.Data, image.Value.Width, image.Value.Height );
```

### Get a list of servers

```csharp
using ( var list = new ServerList.Internet() )
{
    list.AddFilter( "map", "de_dust" );
    await list.RunQueryAsync();

    foreach ( var server in list.Responsive )
    {
        Console.WriteLine( $"{server.Address} {server.Name}" );
    }
}
```

### Achievements

List them

```csharp
    foreach ( var a in SteamUserStats.Achievements )
    {
        Console.WriteLine( $"{a.Name} ({a.State})" );
    }	
```

Unlock them

```csharp
    var ach = new Achievement( "GM_PLAYED_WITH_GARRY" );
    ach.Trigger();
```

### Voice

```csharp
    SteamUser.VoiceRecord = KeyDown( "V" );

    if ( SteamUser.HasVoiceData )
    {
        var bytesrwritten = SteamUser.ReadVoiceData( stream );
        // Send Stream Data To Server or Something
    }
```


### Auth

```csharp
    // Client sends ticket data to server somehow
    var ticket = SteamUser.GetAuthSessionTicket();

    // server listens to event
    SteamServer.OnValidateAuthTicketResponse += ( steamid, ownerid, rsponse ) =>
    {
        if ( rsponse == AuthResponse.OK )
            TellUserTheyCanBeOnServer( steamid );
        else
            KickUser( steamid );
    };

    // server gets ticket data from client, calls this function.. which either returns
    // false straight away, or will issue a TicketResponse.
    if ( !SteamServer.BeginAuthSession( ticketData, clientSteamId ) )
    {
        KickUser( clientSteamId );
    }

    //
    // Client is leaving, cancels their ticket OnValidateAuth is called on the server again
    // this time with AuthResponse.AuthTicketCanceled
    //
    ticket.Cancel();
```

### Utils

```csharp
    SteamUtils.SecondsSinceAppActive;
    SteamUtils.SecondsSinceComputerActive;
    SteamUtils.IpCountry;
    SteamUtils.UsingBatteryPower;
    SteamUtils.CurrentBatteryPower;
    SteamUtils.AppId;
    SteamUtils.IsOverlayEnabled;
    SteamUtils.IsSteamRunningInVR;
    SteamUtils.IsSteamInBigPictureMode;
```

### Workshop

Download a workshop item by ID

```csharp
    SteamUGC.Download( 1717844711 );
```

Get a workshop item information

```csharp
    var itemInfo = await Ugc.Item.Get( 1720164672 );

    Console.WriteLine( $"Title: {itemInfo?.Title}" );
    Console.WriteLine( $"IsInstalled: {itemInfo?.IsInstalled}" );
    Console.WriteLine( $"IsDownloading: {itemInfo?.IsDownloading}" );
    Console.WriteLine( $"IsDownloadPending: {itemInfo?.IsDownloadPending}" );
    Console.WriteLine( $"IsSubscribed: {itemInfo?.IsSubscribed}" );
    Console.WriteLine( $"NeedsUpdate: {itemInfo?.NeedsUpdate}" );
    Console.WriteLine( $"Description: {itemInfo?.Description}" );
```

Query a list of workshop items

```csharp
    var q = Ugc.Query.All
                    .WithTag( "Fun" )
                    .WithTag( "Movie" )
                    .MatchAllTags();

    var result = await q.GetPageAsync( 1 );

    Console.WriteLine( $"ResultCount: {result?.ResultCount}" );
    Console.WriteLine( $"TotalCount: {result?.TotalCount}" );

    foreach ( Ugc.Item entry in result.Value.Entries )
    {
        Console.WriteLine( $"{entry.Title}" );
    }
```

Query items created by friends

```csharp
    var q = Ugc.UserQuery.All
                        .CreatedByFriends();
```

Query items created by yourself

```csharp
    var q = Ugc.UserQuery.All
                        .FromSelf();
```

Publish your own file

```csharp
    var result = await Ugc.Editor.NewCommunityFile
                      .WithTitle( "My New FIle" )
                      .WithDescription( "This is a description" )
                      .WithContent( "c:/folder/addon/location" )
                      .WithTag( "awesome" )
                      .WithTag( "small" )
                      .SubmitAsync( iProgressBar );
```

### Steam Cloud

Write a cloud file

```csharp
    SteamRemoteStorage.FileWrite( "file.txt", fileContents );
```

Read a cloud file

```csharp
    var fileContents = SteamRemoteStorage.FileRead( "file.txt" );
```

List all files

```csharp
    foreach ( var file in SteamRemoteStorage.Files )
    {
        Console.WriteLine( $"{file} ({SteamRemoteStorage.FileSize(file)} {SteamRemoteStorage.FileTime( file )})" );
    }
```


### Steam Inventory

Get item definitions

```csharp
    foreach ( InventoryDef def in SteamInventory.Definitions )
    {
        Console.WriteLine( $"{def.Name}" );
    }
```

Get items that are for sale in the item shop

```csharp
    var defs = await SteamInventory.GetDefinitionsWithPricesAsync();

    foreach ( var def in defs )
    {
        Console.WriteLine( $"{def.Name} [{def.LocalPriceFormatted}]" );
    }
```

Get a list of your items

```csharp
    var result = await SteamInventory.GetItems();

    // result is disposable, good manners to dispose after use
    using ( result )
    {
        var items = result?.GetItems( bWithProperties );

        foreach ( InventoryItem item in items )
        {
            Console.WriteLine( $"{item.Id} / {item.Quantity} / {item.Def.Name} " );
        }
    }
```

# Getting Started

## Client

To initialize a client you can do this.

```csharp
using Steamworks;

// ...

try 
{
    SteamClient.Init( 4000 );
}
catch ( System.Exception e )
{
    // Couldn't init for some reason (steam is closed etc)
}
```

Replace 4000 with the appid of your game. You shouldn't call any Steam functions before you initialize.

When you're done, when you're closing your game, just shutdown.

```csharp
SteamClient.Shutdown();
```

## Server

To create a server do this.

```csharp
var serverInit = new SteamServerInit( "gmod", "Garry Mode" )
{
    GamePort = 28015,
    Secure = true,
    QueryPort = 28016
};

try
{
    Steamworks.SteamServer.Init( 4000, serverInit );
}
catch ( System.Exception )
{
    // Couldn't init for some reason (dll errors, blocked ports)
}
```

# Help

Wanna help? Go for it, pull requests, bug reports, yes, do it.

You can also hit up the [Steamworks Thread](http://steamcommunity.com/groups/steamworks/discussions/0/1319961618833314524/) for help/discussion.

We also have [a wiki you can read](https://wiki.facepunch.com/steamworks/) and help fill out with examples and advice.

# License

MIT - do whatever you want.