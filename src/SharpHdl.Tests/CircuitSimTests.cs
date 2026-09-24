using SharpHdl.Core.Exceptions;
using SharpHdl.Sim;
using SharpHdl.Tests.TestModule;

namespace SharpHdl.Tests;

public class CircuitSimTests
{
	[Fact]
	public void CounterResetsAndIncrements()
	{
		SimSession<Counter> c = new Counter().Run();

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
	public void AluOps(uint op, uint a, uint b, uint expected)
	{
		SimSession<Alu> alu = new Alu().Run();
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
	public void AluTopOpsThroughInstance(uint op, uint a, uint b, uint expected)
	{
		SimSession<AluTop> top = new AluTop().Run();
		top.Set(top.Module.A, a);
		top.Set(top.Module.B, b);
		top.Set(top.Module.Op, op);
		top.Settle();
		Assert.Equal(expected, top.Get(top.Module.Y));
	}

	[Fact]
	public void DualAluTopIndependentInstances()
	{
		SimSession<DualAluTop> top = new DualAluTop().Run();
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
	public void SimpleRamSyncReadWriteVisibleNextCycle()
	{
		SimSession<SimpleRam> ram = new SimpleRam().Run();

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
	public void RegFile2R1WWriteThenCombRead()
	{
		SimSession<RegFile2R1W> rf = new RegFile2R1W().Run();

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
	public void RegFile2R1WDualReadPorts()
	{
		SimSession<RegFile2R1W> rf = new RegFile2R1W().Run();

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
	public void ByteWriteRamSyncReadFullStrobe()
	{
		SimSession<ByteWriteRam> ram = new ByteWriteRam().Run();

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
	public void ByteWriteRamPartialStrobeUpdatesOnlySelectedBytes()
	{
		SimSession<ByteWriteRam> ram = new ByteWriteRam().Run();

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
	public void LoadMemWordsThenPeekAndPoke()
	{
		SimSession<SimpleRam> ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, new ulong[] { 0x10, 0x20, 0x30 });

		Assert.Equal(0x10u, ram.PeekMem(ram.Module.Rdata, 0));
		Assert.Equal(0x20u, ram.PeekMem(ram.Module.Rdata, 1));
		Assert.Equal(0x30u, ram.PeekMem(ram.Module.Rdata, 2));
		Assert.Equal(0u, ram.PeekMem(ram.Module.Rdata, 3));

		ram.PokeMem(ram.Module.Rdata, 1, 0x99);
		Assert.Equal(0x99u, ram.PeekMem(ram.Module.Rdata, 1));
	}

	[Fact]
	public void LoadMemBytesLittleEndian()
	{
		SimSession<SimpleRam> ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, [0x78, 0x56, 0x34, 0x12]);
		Assert.Equal(0x12345678u, ram.PeekMem(ram.Module.Rdata, 0));
	}

	[Fact]
	public void LoadMemWordsVisibleAfterSyncRead()
	{
		SimSession<SimpleRam> ram = new SimpleRam().Run();
		ram.LoadMem(ram.Module.Rdata, [0xABu]);
		Assert.Equal(0u, ram.Get(ram.Module.Rdata));

		ram.Set(ram.Module.We, 0);
		ram.Set(ram.Module.Addr, 0);
		ram.Advance(ram.Module.Clk);
		Assert.Equal(0xABu, ram.Get(ram.Module.Rdata));
	}

	[Fact]
	public void AdvanceWhileStopsWhenConditionMet()
	{
		SimSession<Counter> c = new Counter().Run();
		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 1);

		c.AdvanceWhile(c.Module.Clk, 16, s => s.Get(s.Module.Count) != 5);
		Assert.Equal(5u, c.Get(c.Module.Count));
	}

	[Fact]
	public void AdvanceWhileThrowsWhenMaxCyclesExceeded()
	{
		SimSession<Counter> c = new Counter().Run();
		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 1);

		_ = Assert.Throws<SimException>(() =>
			c.AdvanceWhile(c.Module.Clk, 3, s => s.Get(s.Module.Count) != 100));
	}

	[Fact]
	public void SeqAdvanceNonAssignBodyThrowsSimUnsupported()
	{
		SimSession<SeqNestedIf> s = new SeqNestedIf().Run();
		s.Set(s.Module.Rst, 0);
		s.Set(s.Module.En, 1);
		_ = Assert.Throws<SimUnsupportedException>(() => s.Advance(s.Module.Clk));
	}

	[Theory]
	[InlineData(3u, 3u, 1u, 0u, 0u, 1u, 0u, 1u)]
	[InlineData(2u, 5u, 0u, 1u, 1u, 1u, 0u, 0u)]
	[InlineData(9u, 4u, 0u, 1u, 0u, 0u, 1u, 1u)]
	public void CompareCombOps(uint a, uint b, uint eq, uint neq, uint lt, uint le, uint gt, uint ge)
	{
		SimSession<CompareComb> c = new CompareComb().Run();
		c.Set(c.Module.A, a);
		c.Set(c.Module.B, b);
		c.Settle();
		Assert.Equal(eq, c.Get(c.Module.Eq));
		Assert.Equal(neq, c.Get(c.Module.Neq));
		Assert.Equal(lt, c.Get(c.Module.Lt));
		Assert.Equal(le, c.Get(c.Module.Le));
		Assert.Equal(gt, c.Get(c.Module.Gt));
		Assert.Equal(ge, c.Get(c.Module.Ge));
	}

	[Theory]
	[InlineData(3u, 3u, 10u, 20u, 10u)]
	[InlineData(2u, 5u, 10u, 20u, 20u)]
	public void IfMuxCombSelectsThenOrElse(uint selA, uint selB, uint cVal, uint dVal, uint expected)
	{
		SimSession<IfMuxComb> m = new IfMuxComb().Run();
		m.Set(m.Module.SelA, selA);
		m.Set(m.Module.SelB, selB);
		m.Set(m.Module.C, cVal);
		m.Set(m.Module.D, dVal);
		m.Settle();
		Assert.Equal(expected, m.Get(m.Module.Y));
	}

	[Fact]
	public void IfThenOnlyCombAssignsWhenTrueKeepsPriorWhenFalse()
	{
		SimSession<IfThenOnlyComb> m = new IfThenOnlyComb().Run();
		m.Set(m.Module.A, 1);
		m.Set(m.Module.B, 2);
		m.Set(m.Module.C, 9);
		m.Settle();
		Assert.Equal(0u, m.Get(m.Module.Y));

		m.Set(m.Module.A, 4);
		m.Set(m.Module.B, 4);
		m.Settle();
		Assert.Equal(9u, m.Get(m.Module.Y));
	}

	[Theory]
	[InlineData(0u, 10u, 0u)]
	[InlineData(3u, 13u, 1u)]
	[InlineData(250u, 4u, 0u)]
	public void ConstCombLitSumCompareAndMask(uint a, uint expectedSum, uint expectedIsThree)
	{
		SimSession<ConstComb> m = new ConstComb().Run();
		m.Set(m.Module.A, a);
		m.Settle();
		Assert.Equal(5u, m.Get(m.Module.Imm));
		Assert.Equal(expectedSum, m.Get(m.Module.Sum));
		Assert.Equal(expectedIsThree, m.Get(m.Module.IsThree));
		Assert.Equal(44u, m.Get(m.Module.Masked));
	}
}
