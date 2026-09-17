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
	public void SimpleRam_SyncRead_WriteVisibleNextCycle()
	{
		var ram = new SimpleRam().Run();

		ram.Set(ram.Module.We, 1);
		ram.Set(ram.Module.Addr, 3);
		ram.Set(ram.Module.Wdata, 0xABu);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0u, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xABu, ram.Get(ram.Module.Rdata));
	}

	[Fact]
	public void RegFile2R1W_WriteThenCombRead()
	{
		var rf = new RegFile2R1W().Run();

		rf.Set(rf.Module.Raddr0, 3);
		rf.Set(rf.Module.Raddr1, 3);
		rf.Settle();
		Assert.Equal(0u, rf.Get(rf.Module.Rdata0));
		Assert.Equal(0u, rf.Get(rf.Module.Rdata1));

		rf.Set(rf.Module.We, 1);
		rf.Set(rf.Module.Waddr, 3);
		rf.Set(rf.Module.Wdata, 0x1122334455667788UL);
		rf.Advance(rf.Module.Clk);
		Assert.Equal(0x1122334455667788UL, rf.Get(rf.Module.Rdata0));
		Assert.Equal(0x1122334455667788UL, rf.Get(rf.Module.Rdata1));
	}

	[Fact]
	public void RegFile2R1W_DualReadPorts()
	{
		var rf = new RegFile2R1W().Run();

		rf.Set(rf.Module.We, 1);
		rf.Set(rf.Module.Waddr, 1);
		rf.Set(rf.Module.Wdata, 10);
		rf.Advance(rf.Module.Clk);
		rf.Set(rf.Module.Waddr, 2);
		rf.Set(rf.Module.Wdata, 20);
		rf.Advance(rf.Module.Clk);

		rf.Set(rf.Module.We, 0);
		rf.Set(rf.Module.Raddr0, 1);
		rf.Set(rf.Module.Raddr1, 2);
		rf.Settle();
		Assert.Equal(10u, rf.Get(rf.Module.Rdata0));
		Assert.Equal(20u, rf.Get(rf.Module.Rdata1));
	}

	[Fact]
	public void ByteWriteRam_SyncRead_FullStrobe()
	{
		var ram = new ByteWriteRam().Run();

		ram.Set(ram.Module.We, 1);
		ram.Set(ram.Module.Wstrb, 0xF);
		ram.Set(ram.Module.Addr, 1);
		ram.Set(ram.Module.Wdata, 0xAABBCCDDu);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0u, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xAABBCCDDu, ram.Get(ram.Module.Rdata));
	}

	[Fact]
	public void ByteWriteRam_PartialStrobe_UpdatesOnlySelectedBytes()
	{
		var ram = new ByteWriteRam().Run();

		ram.Set(ram.Module.We, 1);
		ram.Set(ram.Module.Wstrb, 0xF);
		ram.Set(ram.Module.Addr, 2);
		ram.Set(ram.Module.Wdata, 0xAABBCCDDu);
		ram.Advance(ram.Module.Clk);
		ram.Set(ram.Module.We, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xAABBCCDDu, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 1);
		ram.Set(ram.Module.Wstrb, 0x1);
		ram.Set(ram.Module.Wdata, 0x00000011u);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xAABBCCDDu, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xAABBCC11u, ram.Get(ram.Module.Rdata));
	}

	[Fact]
	public void LoadMem_Words_ThenPeekAndPoke()
	{
		var ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, new ulong[] { 0x10, 0x20, 0x30 });

		Assert.Equal(0x10u, ram.PeekMem(ram.Module.Rdata, 0));
		Assert.Equal(0x20u, ram.PeekMem(ram.Module.Rdata, 1));
		Assert.Equal(0x30u, ram.PeekMem(ram.Module.Rdata, 2));
		Assert.Equal(0u, ram.PeekMem(ram.Module.Rdata, 3));

		ram.PokeMem(ram.Module.Rdata, 1, 0x99);
		Assert.Equal(0x99u, ram.PeekMem(ram.Module.Rdata, 1));
	}

	[Fact]
	public void LoadMem_Bytes_LittleEndian()
	{
		var ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, new byte[] { 0x78, 0x56, 0x34, 0x12 });
		Assert.Equal(0x12345678u, ram.PeekMem(ram.Module.Rdata, 0));
	}

	[Fact]
	public void LoadMem_Words_VisibleAfterSyncRead()
	{
		var ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, new ulong[] { 0xABu });
		Assert.Equal(0u, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 0);
		ram.Set(ram.Module.Addr, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xABu, ram.Get(ram.Module.Rdata));
	}

	[Fact]
	public void AdvanceWhile_StopsWhenConditionMet()
	{
		var c = new Counter().Run();
		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 1);

		c.AdvanceWhile(c.Module.Clk, 16, s => s.Get(s.Module.Count) != 5);
		Assert.Equal(5u, c.Get(c.Module.Count));
	}

	[Fact]
	public void AdvanceWhile_ThrowsWhenMaxCyclesExceeded()
	{
		var c = new Counter().Run();
		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 1);

		Assert.Throws<InvalidOperationException>(() =>
			c.AdvanceWhile(c.Module.Clk, 3, s => s.Get(s.Module.Count) != 100));
	}
}
