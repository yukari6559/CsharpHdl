using SharpHdl.Sim;

namespace SharpHdl.Tests;

public class CircuitSimTests
{
	[Fact]
	public void Counter_Resets_AndIncrements()
	{
		var c = new Counter().Run();

		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		Assert.Equal(0u, c.Get(c.Module.Count));

		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 1);
		c.Advance(c.Module.Clk);
		Assert.Equal(1u, c.Get(c.Module.Count));

		c.Advance(c.Module.Clk);
		Assert.Equal(2u, c.Get(c.Module.Count));

		c.Advance(c.Module.Clk, 3);
		Assert.Equal(5u, c.Get(c.Module.Count));
	}

	[Theory]
	[InlineData(0u, 1u, 2u, 3u)]
	[InlineData(1u, 5u, 3u, 2u)]
	[InlineData(2u, 0xFFu, 0x0Fu, 0x0Fu)]
	[InlineData(3u, 0xF0u, 0x0Fu, 0xFFu)]
	public void Alu_Ops(uint op, uint a, uint b, uint expected)
	{
		var alu = new Alu().Run();
		alu.Set(alu.Module.A, a);
		alu.Set(alu.Module.B, b);
		alu.Set(alu.Module.Op, op);
		alu.Settle();
		Assert.Equal(expected, alu.Get(alu.Module.Y));
	}

	[Theory]
	[InlineData(0u, 1u, 2u, 3u)]
	[InlineData(1u, 5u, 3u, 2u)]
	[InlineData(2u, 0xFFu, 0x0Fu, 0x0Fu)]
	[InlineData(3u, 0xF0u, 0x0Fu, 0xFFu)]
	public void AluTop_Ops_ThroughInstance(uint op, uint a, uint b, uint expected)
	{
		var top = new AluTop().Run();
		top.Set(top.Module.A, a);
		top.Set(top.Module.B, b);
		top.Set(top.Module.Op, op);
		top.Settle();
		Assert.Equal(expected, top.Get(top.Module.Y));
	}

	[Fact]
	public void DualAluTop_IndependentInstances()
	{
		var top = new DualAluTop().Run();
		top.Set(top.Module.A0, 1);
		top.Set(top.Module.B0, 2);
		top.Set(top.Module.Op0, 0);
		top.Set(top.Module.A1, 5);
		top.Set(top.Module.B1, 3);
		top.Set(top.Module.Op1, 1);
		top.Settle();
		Assert.Equal(3u, top.Get(top.Module.Y0));
		Assert.Equal(2u, top.Get(top.Module.Y1));
	}

	[Fact]
	public void Run_RejectsMemModule()
	{
		Assert.Throws<SimUnsupportedException>(() => new SimpleRam().Run());
	}
}
