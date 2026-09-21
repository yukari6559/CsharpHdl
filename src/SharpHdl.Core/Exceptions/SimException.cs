namespace SharpHdl.Core.Exceptions;

public class SimException : Exception
{
	public SimException() { }
	public SimException(string message) : base(message) { }
	public SimException(string message, Exception inner) : base(message, inner) { }
}