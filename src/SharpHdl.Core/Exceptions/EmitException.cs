namespace SharpHdl.Core.Exceptions;

public class EmitException : Exception
{
	public EmitException() { }
	public EmitException(string message) : base(message) { }
	public EmitException(string message, Exception inner) : base(message, inner) { }
}