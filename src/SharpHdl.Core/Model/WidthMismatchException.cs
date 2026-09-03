namespace SharpHdl.Core.Model;

public class WidthMismatchException : Exception
{
	public WidthMismatchException(){}
	public WidthMismatchException(string message):base(message){}
	public WidthMismatchException(string message, Exception inner):base(message, inner){}
}