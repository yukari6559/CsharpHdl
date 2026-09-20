namespace SharpHdl.Core.Model;

public class PortConnection()
{
	public required Signal ChildPort { get; set; }
	public required Signal ParentSignal { get; set; }
}