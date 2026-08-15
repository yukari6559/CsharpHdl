using System.Collections.ObjectModel;

namespace SharpHdl.Core.Model;

public class Module
{
	private List<Signal> Ports = [];
	public void SetPorts(List<Signal> ports)
	{
		Ports.AddRange(ports);
	}
	public void SetPort(Signal port)
	{
		Ports.Add(port);
	}
	public ReadOnlyCollection<Signal> GetPorts()
	{
		return Ports.AsReadOnly();
	}

	protected virtual void Describe()
	{
		
	}
}