namespace SharpHdl.Core.Exceptions;

public class UnsupportedSelectException : Exception
{
	public UnsupportedSelectException() { }
	public UnsupportedSelectException(string message) : base(message) { }
	public UnsupportedSelectException(string message, Exception inner) : base(message, inner) { }
}
