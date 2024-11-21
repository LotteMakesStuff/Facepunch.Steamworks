namespace Steamworks.Structs;

public struct TimelineGamePhaseRecording
{
	/// <summary>
	/// The phase ID that this result corresponds with
	/// </summary>
	public readonly string PhaseId;
	/// <summary>
	/// The total length of the recordings in this phase in milliseconds
	/// </summary>
	public readonly ulong TotalLengthInMs;
	/// <summary>
	/// The total length of the longest clip in this phase in milliseconds
	/// </summary>
	public readonly ulong LongestClipLengthInMs; 
	/// <summary>
	/// The number of clips that include video from this phase
	/// </summary>
	public readonly uint ClipCount;
	/// <summary>
	/// The number of screenshots the user has from this phase
	/// </summary>
	public readonly uint ScreenshotCount;

	public TimelineGamePhaseRecording( string phaseId, ulong totalLengthInMs, ulong longestClipLengthInMs, uint clipCount, uint screenshotCount )
	{
		PhaseId = phaseId;
		TotalLengthInMs = totalLengthInMs;
		LongestClipLengthInMs = longestClipLengthInMs;
		ClipCount = clipCount;
		ScreenshotCount = screenshotCount; 
	}
}
