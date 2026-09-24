namespace SharpHdl.Core.Exceptions;

public class MultiDriveException : Exception
{
	public MultiDriveException() { }
	public MultiDriveException(string message) : base(message) { }
	public MultiDriveException(string message, Exception inner) : base(message, inner) { }
}