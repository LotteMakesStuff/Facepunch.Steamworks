namespace Steamworks.Structs;

public struct FileChange
{
	public string Filename;
	public bool Updated;
	public bool Deleted;

	public bool AbsolutePath;
	public bool CloudPath;
}
