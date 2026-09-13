using SharpHdl.Core.Model;

namespace SharpHdl.Sim;

public static class ModuleSimExtensions
{
	public static SimSession<T> Run<T>(this T module) where T : Module
	{
		throw new NotImplementedException();
	}
}