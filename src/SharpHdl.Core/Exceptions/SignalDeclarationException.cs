namespace SharpHdl.Core.Exceptions;

public class SignalDeclarationException : Exception
{
	public SignalDeclarationException() { }
	public SignalDeclarationException(string message) : base(message) { }
	public SignalDeclarationException(string message, Exception inner) : base(message, inner) { }
}
