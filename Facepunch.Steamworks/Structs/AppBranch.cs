namespace Steamworks.Structs;

public struct AppBranch
{
	internal readonly int index;
	/// <summary>
	/// Set of flags (EBetaBranchFlags) describing current branch state.
	/// </summary>
	internal readonly uint flags;
	
	/// <summary>
	/// Content BuildID set live on this branch.
	/// </summary>
	public readonly uint BuildID;
	/// <summary>
	/// Branch name
	/// </summary>
	public readonly string Name;
	/// <summary>
	/// Branch description
	/// </summary>
	public readonly string Description;

	/// <summary>
	/// Is this the default public branch?
	/// </summary>
	public bool IsDefaultBranch
	{
		get { return (flags & (int)BetaBranchFlags.Default) != 0; }
	}
	
	/// <summary>
	/// Is this branch avaliable to be selected?
	/// </summary>
	public bool IsBranchAvailable
	{
		get { return (flags & (int)BetaBranchFlags.Available) != 0; }
	}
	
	/// <summary>
	/// Is this branch password protected?
	/// </summary>
	public bool IsBranchPrivate
	{
		get { return (flags & (int)BetaBranchFlags.Private) != 0; }
	}
	
	/// <summary>
	/// Is this branch selected (active)? 
	/// </summary>
	public bool IsBranchSelected
	{
		get { return (flags & (int)BetaBranchFlags.Selected) != 0; }
	}
	
	/// <summary>
	/// Is this branch installed?
	/// </summary>
	public bool IsBranchInstalled
	{
		get { return (flags & (int)BetaBranchFlags.Installed) != 0; }
	}

	internal AppBranch( int index, uint flags, uint buildId, string name, string description )
	{
		this.index = index;
		this.flags = flags;
		BuildID = buildId;
		Name = name;
		Description = description;
	}

	/// <summary>
	/// Select this branch and ask steam to make it active. This might need the game to restart so Steam can update its' content that branch
	/// </summary>
	/// <returns></returns>
	public bool SetBranchActive()
	{
		return SteamApps.SetActiveBranch(Name);
	}

	public override string ToString()
	{
		return $"[{index}] {Name} - {Description} [Default:{(IsDefaultBranch ? "Y" : "N")} Available:{(IsBranchAvailable ? "Y" : "N")} Private:{(IsBranchPrivate ? "Y" : "N")} Selected:{(IsBranchSelected ? "Y" : "N")} Installed:{(IsBranchInstalled ? "Y" : "N")}]";
	}
}
