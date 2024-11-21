using System;
using System.Threading.Tasks;
using Steamworks.Data;
using Steamworks.Structs;

namespace Steamworks;

public class SteamTimeline : SteamClientClass<SteamTimeline>
{
	internal static ISteamTimeline Internal => Interface as ISteamTimeline;

	internal override bool InitializeInterface( bool server )
	{
		SetInterface( server, new ISteamTimeline( server ) );
		if ( Interface.Self == IntPtr.Zero ) return false;
		
		return true;
	}

	public uint MaxTimelinePriority => Defines.k_unMaxTimelinePriority;

	/// <summary>
	/// Sets a description (B) for the current game state in the timeline. These help the user to find specific moments in the timeline when saving clips. Setting a new state description replaces any previous description.
	/// Examples could include:
	/// - Where the user is in the world in a single player game
	/// - Which round is happening in a multiplayer game
	/// - The current score for a sports game
	/// </summary>
	/// <param name="description">A localized string in the language returned by SteamUtils.SteamUILanguage</param>
	/// <param name="timeDelta">The time offset in seconds to apply to this state change. Negative times indicate an event that happened in the past.</param>
	public static void SetTimelineTooltip(string description, float timeDelta) => Internal.SetTimelineTooltip(description, timeDelta);
	
	/// <summary>
	/// Clears the previous set game state in the timeline.
	/// </summary>
	/// <param name="timeDelta">The time offset in seconds to apply to this state change. Negative times indicate an event that happened in the past.</param>
	public static void ClearTimelineTooltip(float timeDelta) => Internal.ClearTimelineTooltip(timeDelta);
	
	/// <summary>
	/// Changes the color of the timeline bar (C). See TimelineGameMode for how to use each value.
	/// </summary>
	/// <param name="mode">The mode that the game is in.</param>
	public static void SetTimelineGameMode(TimelineGameMode mode) => Internal.SetTimelineGameMode( mode );
	
	/// <summary>
	/// Game phases allow the user to navigate their background recordings and clips. Exactly what a game phase means will vary game to game, but the game phase should be a section of gameplay that is usually between 10 minutes and a few hours in length, and should be the main way a user would think to divide up the game.
	/// </summary>
	public static class GamePhase
	{
		/// <summary>
		/// Use this to start a game phase. Game phases allow the user to navigate their background recordings and clips. Exactly what a game phase means will vary game to game, but the game phase should be a section of gameplay that is usually between 10 minutes and a few hours in length, and should be the main way a user would think to divide up the game. These are presented to the user in a UI that shows the date the game was played, with one row per game slice. Game phases should be used to mark sections of gameplay that the user might be interested in watching.
		///
		/// Examples could include:
		/// - A single match in a multiplayer PvP game
		/// - A chapter of a story-based singleplayer game
		/// - A single run in a roguelike
		///
		/// Game phases are started with StartGamePhase, and while a phase is still happening, they can have tags and attributes added to them with GamePhase.AddTag or GamePhase.SetAttribute. Only one game phase can be active at a time.
		/// </summary>
		public static void StartPhase() => Internal.StartGamePhase();

		/// <summary>
		/// Use this to end a game phase that was started with GamePhase.StartPhase.
		/// </summary>
		public static void EndPhase() => Internal.EndGamePhase();

		/// <summary>
		/// The phase ID is used to let the game identify which phase it is referring to in calls to DoesGamePhaseRecordingExist or GamePhase.OpenOverlay. It may also be used to associated multiple phases with each other.
		/// </summary>
		/// <param name="id">A game-provided persistent ID for a game phase. This could be the match ID in a multiplayer game, a chapter name in a single player game, the ID of a character, etc.</param>
		public static void SetId(string id) => Internal.SetGamePhaseID( id );
		
		/// <summary>
		/// Use this to determine if video recordings exist for the specified game phase. This can be useful when the game needs to decide whether or not to show a control that will call GamePhase.OpenOverlay.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<TimelineGamePhaseRecording> DoesRecordingExist(string id)
		{
			var result = await Internal.DoesGamePhaseRecordingExist( id );

			if ( result.HasValue )
			{
				return new TimelineGamePhaseRecording( id, result.Value.RecordingMS, result.Value.LongestClipMS, result.Value.ClipCount, result.Value.ScreenshotCount );
			}

			return new TimelineGamePhaseRecording( id, 0, 0, 0, 0 );
		}

		/// <summary>
		/// Use this to add a game phase tag (F). Phase tags represent data with a well defined set of options, which could be data such as match resolution, hero played, game mode, etc. Tags can have an icon in addition to a text name. Multiple tags within the same group may be added per phase and all will be remembered. For example, GamePhase.AddTag(...) may be called multiple times for a "Bosses Defeated" group, with different names and icons for each boss defeated during the phase, all of which will be shown to the user.
		/// </summary>
		/// <param name="tag">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="icon">The name of the icon to show when the tag is shown in the UI. This can be one of the icons uploaded through the Steamworks partner Site for your title, or one of the provided icons that start with steam_. The Steam Timelines overview includes a list of available icons.</param>
		/// <param name="tagGroup">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage. Tags within the same group will be shown together in the UI.</param>
		/// <param name="priority">Provide the priority to use when the UI is deciding which icons to display. Tags with larger priority values will be displayed more prominently than tags with smaller priority values. This value must be between 0 and MaxTimelinePriority.</param>
		public static void AddTag(string tag, string icon, string tagGroup, uint priority)
			=> Internal.AddGamePhaseTag(tag, icon, tagGroup, priority);

		/// <summary>
		/// Use this to add a game phase attribute (E). Phase attributes represent generic text fields that can be updated throughout the duration of the phase. They are meant to be used for phase metadata that is not part of a well defined set of options. For example, a KDA attribute that starts with the value "0/0/0" and updates as the phase progresses, or something like a played-entered character name. Attributes can be set as many times as the game likes with SetGamePhaseAttribute, and only the last value will be shown to the user.
		/// </summary>
		/// <param name="attributeGroup">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="attributeValue">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="priority">Provide the priority to use when the UI is deciding which icons to display. Tags with larger priority values will be displayed more prominently than tags with smaller priority values. This value must be between 0 and MaxTimelinePriority.</param>
		public static void SetAttribute(string attributeGroup, string attributeValue, uint priority)
			=> Internal.SetGamePhaseAttribute(attributeGroup, attributeValue, priority);
		
		/// <summary>
		/// Opens the Steam overlay to the section of the timeline represented by the game phase.
		/// </summary>
		/// <param name="phaseId">The game phase to show in the overlay.</param>
		public static void OpenOverlay(string phaseId) 
			=> Internal.OpenOverlayToGamePhase(phaseId);
	}
	
	/// <summary>
	/// Timeline Events mark sections of the Timeline with icons and tags. 
	/// </summary>
	public static class TimelineEvents
	{
		/// <summary>
		/// Use this to mark an event (A) on the Timeline. This event will be instantaneous. (See AddRangedEvent to add events that happened over time.)
		/// Examples could include:
		/// - picking up a new weapon or ammo
		/// - scoring a goal
		///
		/// The game can nominate an event as being suitable for a clip by passing TimelineEventClipPriority.Standard or TimelineEventClipPriority.Featured to possibleClip. Players can make clips of their own at any point, but this lets the game suggest some options to Steam to make that process easier for players.
		///
		/// This function returns a TimelineEvent that can be used with RemoveTimelineEvent or the overlay functions DoesEventRecordingExist and OpenOverlayToTimelineEvent.
		/// Maps to AddInstantaneousTimelineEvent
		/// </summary>
		/// <param name="title">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="description">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="icon">The name of the icon to show at the timeline at this point. This can be one of the icons uploaded through the Steamworks partner Site for your title, or one of the provided icons that start with steam_. The Steam Timelines overview includes a list of available icons.</param>
		/// <param name="priority">Provide the priority to use when the UI is deciding which icons to display in crowded parts of the timeline. Events with larger priority values will be displayed more prominently than events with smaller priority values. This value must be between 0 and MaxTimelinePriority.</param>
		/// <param name="startOffsetInSeconds">	The time offset in seconds to apply to the start of the event. Negative times indicate an event that happened in the past. One use of this parameter is to handle events whose significance is not clear until after the fact. For instance if the player starts a damage over time effect on another player, which kills them 3.5 seconds later, the game could pass -3.5 as the start offset and cause the event to appear in the timeline where the effect started.</param>
		/// <param name="possibleClip">Allows the game to describe events that should be suggested to the user as possible video clips.</param>
		public static TimelineEvent AddInstantEvent(string title, string description, string icon, uint priority, float startOffsetInSeconds, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
			=> new TimelineEvent(Internal.AddInstantaneousTimelineEvent(title, description, icon, priority, startOffsetInSeconds, possibleClip), title, description, icon, priority, startOffsetInSeconds, possibleClip);
		
		/// <summary>
		/// Use this to mark an event (A) on the Timeline that takes some amount of time to complete.
		/// Examples could include:
		/// - a boss battle
		/// - an impressive combo
		/// - a large team fight
		///
		/// The game can nominate an event as being suitable for a clip by passing TimelineEventClipPriority.Standard or TimelineEventClipPriority.Featured to possibleClip. Players can make clips of their own at any point, but this lets the game suggest some options to Steam to make that process easier for players.
		///
		/// This function returns a TimelineEvent that can be used with RemoveTimelineEvent or the overlay functions DoesEventRecordingExist and OpenOverlayToTimelineEvent.
		/// Maps to AddRangeTimelineEvent
		/// </summary>
		/// <param name="title">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="description">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="icon">The name of the icon to show at the timeline at this point. This can be one of the icons uploaded through the Steamworks partner Site for your title, or one of the provided icons that start with steam_. The Steam Timelines overview includes a list of available icons.</param>
		/// <param name="priority">Provide the priority to use when the UI is deciding which icons to display in crowded parts of the timeline. Events with larger priority values will be displayed more prominently than events with smaller priority values. This value must be between 0 and MaxTimelinePriority.</param>
		/// <param name="startOffsetInSeconds">	The time offset in seconds to apply to the start of the event. Negative times indicate an event that happened in the past. One use of this parameter is to handle events whose significance is not clear until after the fact. For instance if the player starts a damage over time effect on another player, which kills them 3.5 seconds later, the game could pass -3.5 as the start offset and cause the event to appear in the timeline where the effect started.</param>
		/// <param name="durationInSeconds">The duration of the event, in seconds. Pass 0 for instantaneous events.</param>
		/// <param name="possibleClip"></param>
		/// <returns>Allows the game to describe events that should be suggested to the user as possible video clips.</returns>
		public static TimelineEvent AddRangedEvent(string title, string description, string icon, uint priority, float startOffsetInSeconds, float durationInSeconds, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None) 
			=> new TimelineEvent(Internal.AddRangeTimelineEvent(title, description, icon, priority, startOffsetInSeconds, durationInSeconds, possibleClip), title, description, icon, priority, startOffsetInSeconds, possibleClip);

		/// <summary>
		/// Use this to mark the start of an event (A) on the Timeline that takes some amount of time to complete. The duration of the event is determined by a matching call to EndRangeTimelineEvent. If the game wants to cancel an event in progress, they can do that with a call to RemoveTimelineEvent.
		///
		/// The event in progress can be updated any number of times with UpdateRangeTimelineEvent.
		/// 
		/// The game can nominate an event as being suitable for a clip by passing TimelineEventClipPriority.Standard or TimelineEventClipPriority.Featured to possibleClip. Players can make clips of their own at any point, but this lets the game suggest some options to Steam to make that process easier for players.
		/// Maps to StartRangeTimelineEvent
		/// </summary>
		/// <param name="title">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="description">Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.</param>
		/// <param name="icon">The name of the icon to show at the timeline at this point. This can be one of the icons uploaded through the Steamworks partner Site for your title, or one of the provided icons that start with steam_. The Steam Timelines overview includes a list of available icons.</param>
		/// <param name="priority">Provide the priority to use when the UI is deciding which icons to display in crowded parts of the timeline. Events with larger priority values will be displayed more prominently than events with smaller priority values. This value must be between 0 and MaxTimelinePriority.</param>
		/// <param name="startOffsetInSeconds">	The time offset in seconds to apply to the start of the event. Negative times indicate an event that happened in the past. One use of this parameter is to handle events whose significance is not clear until after the fact. For instance if the player starts a damage over time effect on another player, which kills them 3.5 seconds later, the game could pass -3.5 as the start offset and cause the event to appear in the timeline where the effect started.</param>
		/// <param name="possibleClip">Allows the game to describe events that should be suggested to the user as possible video clips.</param>
		/// <returns></returns>
		public static TimelineEvent StartEvent(string title, string description, string icon, uint priority, float startOffsetInSeconds, TimelineEventClipPriority possibleClip = TimelineEventClipPriority.None)
		=> new TimelineEvent(title, description, icon, priority, startOffsetInSeconds, possibleClip);
		
		/// <summary>
		/// Use this to identify the end of an event that was started with StartRangeTimelineEvent.
		/// Maps to EndRangeTimelineEvent
		/// </summary>
		/// <param name="timelineEvent">The timeline event to update</param>
		/// <param name="endOffsetInSeconds">The time offset in seconds to apply to the end of the event. Negative times indicate an event that happened in the past.</param>
		public static void EndEvent(TimelineEvent timelineEvent, float endOffsetInSeconds)
			=> Internal.EndRangeTimelineEvent(timelineEvent.internalHandle, endOffsetInSeconds);
		
		/// <summary>
		/// Use this to determine if video recordings exist for the specified event. This can be useful when the game needs to decide whether or not to show a control that will call OpenOverlayToTimelineEvent.
		/// </summary>
		/// <param name="timelineEvent">The TimelineEvent to check for recordings.</param>
		/// <returns></returns>
		public static async Task<bool> DoesEventRecordingExist(TimelineEvent timelineEvent)
		{
			var result = await Internal.DoesEventRecordingExist(timelineEvent.internalHandle);
		
			return result.HasValue && result.Value.RecordingExists;
		}
		
		/// <summary>
		/// Opens the Steam overlay to the section of the timeline represented by the timeline event.
		/// This event must be in the current game session, since TimelineEvents are not valid for future runs of the game.
		/// </summary>
		/// <param name="timelineEvent">The TimelineEvent to show in the overlay.</param>
		public static void OpenOverlayToTimelineEvent(TimelineEvent timelineEvent) 
			=> Internal.OpenOverlayToTimelineEvent(timelineEvent.internalHandle);

		/// <summary>
		/// Delete an event from the timeline. This can be called on a timeline event created by AddInstantEvent, AddRangedEvent or Start/EndEvent.
		/// The timeline event handle must be from the current game process.
		/// </summary>
		/// <param name="timelineEvent">Event to remove.</param>
		public static void RemoveEvent( TimelineEvent timelineEvent )
			=> Internal.RemoveTimelineEvent( timelineEvent.internalHandle );
	}
}
