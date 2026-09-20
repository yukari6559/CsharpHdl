namespace SharpHdl.Sim;

public class SimUnsupportedException : Exception
{
	public SimUnsupportedException() { }
	public SimUnsupportedException(string message) : base(message) { }
	public SimUnsupportedException(string message, Exception inner) : base(message, inner) { }
}