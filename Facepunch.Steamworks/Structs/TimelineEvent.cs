using Steamworks.Data;

namespace Steamworks.Structs;

public struct TimelineEvent
{
	internal readonly TimelineEventHandle_t internalHandle;
	internal readonly bool isUpdatable;
	internal readonly bool isValid;
	private string title;
	private string description;
	private string icon;
	private uint priority;
	private float startOffsetInSeconds;
	private TimelineEventClipPriority possibleClip;
	
	/// <summary>
	/// Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.
	/// </summary>
	public string Title
	{
		get => title;
		set
		{
			if (!isValid || !isUpdatable) return;
			if ( title == value )
			{
				return;
			}

			title = value;
			SteamTimeline.Internal.UpdateRangeTimelineEvent(internalHandle, title, description, icon, priority, possibleClip );
		}
	}
	
	/// <summary>
	/// Title-provided localized string in the language returned by SteamUtils.SteamUILanguage.
	/// </summary>
	public string Description
	{
		get => description;
		set
		{
			if (!isValid || !isUpdatable) return;
			if ( description == value )
			{
				return;
			}

			description = value;
			SteamTimeline.Internal.UpdateRangeTimelineEvent(internalHandle, title, description, icon, priority, possibleClip );
		}
	}
	
	/// <summary>
	/// The name of the icon to show at the timeline at this point. This can be one of the icons uploaded through the Steamworks partner Site for your title, or one of the provided icons that start with steam_.
	/// The Steam Timelines overview includes a list of available icons.
	/// </summary>
	public string Icon
	{
		get => icon;
		set
		{
			if (!isValid || !isUpdatable) return;
			if ( icon == value )
			{
				return;
			}

			icon = value;
			SteamTimeline.Internal.UpdateRangeTimelineEvent(internalHandle, title, description, icon, priority, possibleClip );
		}
	}
	
	/// <summary>
	/// Provide the priority to use when the UI is deciding which icons to display in crowded parts of the timeline. Events with larger priority values will be displayed more prominently than events with smaller priority values.
	/// This value must be between 0 and SteamTimeline.MaxTimelinePriority.
	/// </summary>
	public uint Priority
	{
		get => priority;
		set
		{
			if (!isValid || !isUpdatable) return;
			if ( priority == value )
			{
				return;
			}

			priority = value;
			SteamTimeline.Internal.UpdateRangeTimelineEvent(internalHandle, title, description, icon, priority, possibleClip );
		}
	}
	
	/// <summary>
	/// The time offset in seconds to apply to the start of the event. Negative times indicate an event that happened in the past. One use of this parameter is to handle events whose significance is not clear until after the fact.
	/// For instance if the player starts a damage over time effect on another player, which kills them 3.5 seconds later, the game could pass -3.5 as the start offset and cause the event to appear in the timeline where the effect started
	/// </summary>
	public float StartOffsetInSeconds
	{
		get => startOffsetInSeconds;
	}
	
	/// <summary>
	/// Allows the game to describe events that should be suggested to the user as possible video clips.
	/// </summary>
	public TimelineEventClipPriority PossibleClip
	{
		get => possibleClip;
		set
		{
			if (!isValid || !isUpdatable) return;
			if ( possibleClip == value )
			{
				return;
			}

			possibleClip = value;
			SteamTimeline.Internal.UpdateRangeTimelineEvent(internalHandle, title, description, icon, priority, possibleClip );
		}
	}

	public bool IsUpdateable
	{
		get => isUpdatable;
	}
	
	public bool IsValid
	{
		get => isValid;
	}

	public TimelineEvent()
	{
		this.title = default;
		this.description = default;
		this.icon = default;
		this.priority = default;
		this.startOffsetInSeconds = default;
		this.possibleClip = TimelineEventClipPriority.Invalid;
		this.isUpdatable = false;
		this.internalHandle = default;
		this.isValid = false;
	}

	internal TimelineEvent(string title, string description, string icon, uint priority, float startOffsetInSeconds, TimelineEventClipPriority possibleClip) 
	{
		this.title = title;
		this.description = description;
		this.icon = icon;
		this.priority = priority;
		this.startOffsetInSeconds = startOffsetInSeconds;
		this.possibleClip = possibleClip;
		this.isUpdatable = true;
		this.internalHandle = SteamTimeline.Internal.StartRangeTimelineEvent( title, description, icon, priority, startOffsetInSeconds, possibleClip );

		this.isValid = true;
	}

	internal TimelineEvent(TimelineEventHandle_t addInstantaneousTimelineEvent, string title, string description, string icon, uint priority, float startOffsetInSeconds, TimelineEventClipPriority possibleClip)
	{
		this.title = title;
		this.description = description;
		this.icon = icon;
		this.priority = priority;
		this.startOffsetInSeconds = startOffsetInSeconds;
		this.possibleClip = possibleClip;
		this.isUpdatable = false;
		this.internalHandle = addInstantaneousTimelineEvent;

		this.isValid = true;
	}
}
