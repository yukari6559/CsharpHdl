namespace SharpHdl.Core.Exceptions;

public class DescribeContextException : Exception
{
	public DescribeContextException() { }
	public DescribeContextException(string message) : base(message) { }
	public DescribeContextException(string message, Exception inner) : base(message, inner) { }
}