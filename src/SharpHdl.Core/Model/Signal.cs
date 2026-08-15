namespace SharpHdl.Core.Model;

public enum SignalDirection
{
    Input,
    Output
}
public class Signal
{
	protected SignalDirection Direction{get;}
	public string Name {get;}
	public uint Width {get;}

	protected Signal(uint width, string name, SignalDirection direction)
	{
		Width = width;
		Name = name;
		Direction = direction;
	}
}

public class In : Signal
{
	private In(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	public static In UInt(uint width, string name)
		=> new In(width, name, SignalDirection.Input);
}

public class Out : Signal
{
	private Out(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}
	public static Out UInt(uint width, string name)
		=> new Out(width, name, SignalDirection.Output);
}
