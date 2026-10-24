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

	[Fact]
	public void CounterTopAdvancesChildSeqWithParentClock()
	{
		SimSession<CounterTop> c = new CounterTop().Run();

		c.Set(c.Module.Rst, 1);
		c.Advance(c.Module.Clk);
		Assert.Equal(0u, c.Get(c.Module.Count));

		c.Set(c.Module.Rst, 0);
		c.Set(c.Module.Step, 3);
		c.Advance(c.Module.Clk, 2);
		Assert.Equal(6u, c.Get(c.Module.Count));
	}

	[Fact]
	public void SeqResetUlongResetsAboveUintMax()
	{
		SimSession<SeqResetUlong> s = new SeqResetUlong().Run();

		s.Set(s.Module.Rst, 1);
		s.Advance(s.Module.Clk);
		Assert.Equal(0x1_0000_0000UL, s.Get(s.Module.Q));

		s.Set(s.Module.Rst, 0);
		s.Advance(s.Module.Clk);
		Assert.Equal(0x1_0000_0001UL, s.Get(s.Module.Q));
	}

	[Fact]
	public void DualCombAssignRunThrowsMultiDrive()
	{
		BadMultiDriveModule module = new();
		module.DescribeDualCombAssign();
		_ = Assert.Throws<MultiDriveException>(module.Run);
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
	[InlineData(0u, 10u)]
	[InlineData(1u, 20u)]
	[InlineData(2u, 30u)]
	[InlineData(3u, 40u)]
	[InlineData(5u, 20u)]
	public void SwitchSliceSelSelectsByLowBits(uint instr, uint expected)
	{
		SimSession<SwitchSliceSel> m = new SwitchSliceSel().Run();
		m.Set(m.Module.Instr, instr);
		m.Settle();
		Assert.Equal(expected, m.Get(m.Module.Y));
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
	[InlineData(0xF0u, 0x0Fu, 0xFFu, 0x00u)]
	[InlineData(0xAAu, 0xAAu, 0x00u, 0xFFu)]
	[InlineData(0x3Cu, 0x5Au, 0x66u, 0x99u)]
	public void XorCombXorAndXnorStayWithinWidth(uint a, uint b, uint x, uint xn)
	{
		SimSession<XorComb> m = new XorComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Settle();
		Assert.Equal(x, m.Get(m.Module.X));
		Assert.Equal(xn, m.Get(m.Module.Xn));
	}

	[Theory]
	[InlineData(0xF0u, 0x0Fu, 0x3Cu, 0x3Cu, 0x3Cu, 0x30u)]
	[InlineData(0xAAu, 0x55u, 0x0Fu, 0x0Fu, 0x0Fu, 0x0Au)]
	public void NestedOpCombKeepsTreeGrouping(uint a, uint b, uint c, uint xorAnd, uint orAnd, uint andOr)
	{
		SimSession<NestedOpComb> m = new NestedOpComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.C, c);
		m.Settle();
		Assert.Equal(xorAnd, m.Get(m.Module.XorAnd));
		Assert.Equal(orAnd, m.Get(m.Module.OrAnd));
		Assert.Equal(andOr, m.Get(m.Module.AndOr));
	}

	[Theory]
	[InlineData(0xFFu, 0xFFu, 0xFu, 0xFE01u, 0xEF1u)]
	[InlineData(0x12u, 0x34u, 0x7u, 0x3A8u, 0x7Eu)]
	[InlineData(0x80u, 0x02u, 0x0u, 0x100u, 0x000u)]
	public void MulFullCombKeepsTheFullProduct(uint a, uint b, uint n, uint full, uint mixed)
	{
		SimSession<MulFullComb> m = new MulFullComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.N, n);
		m.Settle();
		Assert.Equal(full, m.Get(m.Module.Full));
		Assert.Equal(mixed, m.Get(m.Module.Mixed));
	}

	[Theory]
	[InlineData(0u, 1u, 0u, 0u, 0u, 0u, 0u)]
	[InlineData(2u, 1u, 0u, 1u, 1u, 1u, 0u)]
	[InlineData(2u, 0u, 5u, 0u, 1u, 1u, 1u)]
	[InlineData(0u, 0u, 1u, 0u, 1u, 1u, 0u)]
	[InlineData(0u, 3u, 4u, 0u, 1u, 1u, 1u)]
	public void LogicCombTreatsNonZeroAsTrue(uint a, uint b, uint n, uint andAB, uint orAN, uint andOfOr, uint orOfAnd)
	{
		SimSession<LogicComb> m = new LogicComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.N, n);
		m.Settle();
		Assert.Equal(andAB, m.Get(m.Module.AndAB));
		Assert.Equal(orAN, m.Get(m.Module.OrAN));
		Assert.Equal(andOfOr, m.Get(m.Module.AndOfOr));
		Assert.Equal(orOfAnd, m.Get(m.Module.OrOfAnd));
	}

	[Theory]
	[InlineData(0x0Fu, 0x3Cu, 0u, 0xF0u, 0xF1u, 0u, 0xF3u, 0x0Fu, 0x30u, 0x0u, 1u)]
	[InlineData(0x00u, 0xFFu, 1u, 0xFFu, 0x00u, 1u, 0xFFu, 0x00u, 0xFFu, 0xFu, 0u)]
	[InlineData(0x02u, 0x01u, 0u, 0xFDu, 0xFEu, 0u, 0xFFu, 0x02u, 0x01u, 0xDu, 1u)]
	[InlineData(0x80u, 0x80u, 1u, 0x7Fu, 0x80u, 0u, 0x7Fu, 0x80u, 0x00u, 0xFu, 0u)]
	public void UnaryCombMasksToOperandWidth(uint a, uint b, uint en, uint notA, uint negA, uint lNotA, uint notAnd, uint negNeg, uint notAAndB, uint notLow, uint lNotEn)
	{
		SimSession<UnaryComb> m = new UnaryComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.En, en);
		m.Settle();
		Assert.Equal(notA, m.Get(m.Module.NotA));
		Assert.Equal(negA, m.Get(m.Module.NegA));
		Assert.Equal(lNotA, m.Get(m.Module.LNotA));
		Assert.Equal(notAnd, m.Get(m.Module.NotAnd));
		Assert.Equal(negNeg, m.Get(m.Module.NegNeg));
		Assert.Equal(notAAndB, m.Get(m.Module.NotAAndB));
		Assert.Equal(notLow, m.Get(m.Module.NotLow));
		Assert.Equal(lNotEn, m.Get(m.Module.LNotEn));
	}

	[Theory]
	[InlineData(0xFFu, 0x0Fu, 1u, new uint[] { 1, 1, 0, 0, 0, 1, 0, 1, 1, 0 })]
	[InlineData(0x00u, 0xFFu, 1u, new uint[] { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 })]
	[InlineData(0x07u, 0xF0u, 0u, new uint[] { 0, 1, 1, 1, 0, 0, 1, 0, 0, 1 })]
	[InlineData(0x80u, 0x80u, 1u, new uint[] { 0, 1, 1, 1, 0, 0, 1, 1, 1, 0 })]
	public void ReduceCombFoldsAllBits(uint a, uint b, uint en, uint[] expected)
	{
		SimSession<ReduceComb> m = new ReduceComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.En, en);
		m.Settle();
		uint[] actual =
		[
			(uint)m.Get(m.Module.AndR), (uint)m.Get(m.Module.OrR), (uint)m.Get(m.Module.XorR),
			(uint)m.Get(m.Module.NandR), (uint)m.Get(m.Module.NorR), (uint)m.Get(m.Module.XnorR),
			(uint)m.Get(m.Module.NotAndR), (uint)m.Get(m.Module.EnOr), (uint)m.Get(m.Module.OrOfAnd),
			(uint)m.Get(m.Module.LowXor),
		];
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10u, 7u, 3u, 6u, 9u)]
	[InlineData(0x80u, 0x01u, 0x02u, 0x81u, 0xFEu)]
	public void SameWidthExtendCombKeepsTreeGrouping(uint a, uint b, uint c, uint subSub, uint subMul)
	{
		SimSession<SameWidthExtendComb> m = new SameWidthExtendComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.C, c);
		m.Settle();
		Assert.Equal(subSub, m.Get(m.Module.SubSub));
		Assert.Equal(subMul, m.Get(m.Module.SubMul));
	}

	[Theory]
	[InlineData(0xF0u, 0xFu, 0xFFu, 1u)]
	[InlineData(0x0Au, 0xFu, 0x19u, 0u)]
	[InlineData(0xFFu, 0x1u, 0x00u, 1u)]
	public void ExtendedOperandCombAddsAndComparesAtExtendedWidth(uint a, uint b, uint sum, uint gt)
	{
		SimSession<ExtendedOperandComb> m = new ExtendedOperandComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Settle();
		Assert.Equal(sum, m.Get(m.Module.Sum));
		Assert.Equal(gt, m.Get(m.Module.Gt));
	}

	[Theory]
	[InlineData(0xFFF0_0A00u, 1u, 0xFFFF_FFFF_FFFF_FFFFUL, 0xAu, 0xFFu, 0xFFu, 1u)]
	[InlineData(0x7FF0_0500u, 0u, 0x7FFUL, 0x5u, 0x00u, 0x00u, 0u)]
	public void SliceSelectCombFoldsAndSignExtendsSlices(uint instr, uint f, ulong imm, uint folded, uint top, uint fExt, uint fSlice)
	{
		SimSession<SliceSelectComb> m = new SliceSelectComb().Run();
		m.Set(m.Module.Instr, instr);
		m.Set(m.Module.F, f);
		m.Settle();
		Assert.Equal(imm, m.Get(m.Module.Imm));
		Assert.Equal(folded, m.Get(m.Module.Folded));
		Assert.Equal(top, m.Get(m.Module.Top));
		Assert.Equal(instr, m.Get(m.Module.Full));
		Assert.Equal(fExt, m.Get(m.Module.FExt));
		Assert.Equal(fSlice, m.Get(m.Module.FSlice));
	}

	[Theory]
	[InlineData(1u, 1u, 0x86u, 0x61u, 0x0000_0002u)]
	[InlineData(7u, 31u, 0x80u, 0x01u, 0x8000_0000u)]
	[InlineData(8u, 64u, 0x00u, 0x00u, 0x0000_0000u)]
	[InlineData(15u, 100u, 0x00u, 0x00u, 0x0000_0000u)]
	public void ShiftCombShiftsLogicallyAndZeroesOutOfRangeAmounts(uint amt, uint amt7, uint shlVar, uint shrVar, uint wShlVar)
	{
		SimSession<ShiftComb> m = new ShiftComb().Run();
		m.Set(m.Module.A, 0xC3);
		m.Set(m.Module.W, 0x8000_0001);
		m.Set(m.Module.Amt, amt);
		m.Set(m.Module.Amt7, amt7);
		m.Settle();

		Assert.Equal(0x0Cu, m.Get(m.Module.ShlConst));
		Assert.Equal(0x18u, m.Get(m.Module.ShrConst));
		Assert.Equal(shlVar, m.Get(m.Module.ShlVar));
		Assert.Equal(shrVar, m.Get(m.Module.ShrVar));
		Assert.Equal(0x4000_0000u, m.Get(m.Module.WShr));
		Assert.Equal(wShlVar, m.Get(m.Module.WShlVar));
		Assert.Equal(0x49u, m.Get(m.Module.Nested));
		Assert.Equal(0xC3u, m.Get(m.Module.ShlZero));
	}

	[Theory]
	[InlineData(0xFFu, 0xFFu, 0x02u, 0xFFFF_FFFF_FFFF_FFFFUL, 0x2UL, 0x01u, 0xFE01u, 0x03u, 0xFFFF_FFFF_FFFF_FFFEUL)]
	[InlineData(0x12u, 0x34u, 0x10u, 0x1_0000_0001UL, 0x1_0000_0001UL, 0xA8u, 0x03A8u, 0xB8u, 0x0000_0002_0000_0001UL)]
	public void MulCombTruncatesToOperandWidthAndWidensExplicitly(uint a, uint b, uint c, ulong q, ulong r, uint low, uint full, uint nested, ulong q64)
	{
		SimSession<MulComb> m = new MulComb().Run();
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.C, c);
		m.Set(m.Module.Q, q);
		m.Set(m.Module.R, r);
		m.Settle();

		Assert.Equal(low, m.Get(m.Module.Low));
		Assert.Equal(full, m.Get(m.Module.Full));
		Assert.Equal(nested, m.Get(m.Module.Nested));
		Assert.Equal(q64, m.Get(m.Module.Q64));
	}

	[Fact]
	public void BitUtilsShiftHandlesFullWidthAndLargeAmounts()
	{
		Assert.Equal(1UL, BitUtils.ShiftRight(ulong.MaxValue, 63, 64));
		Assert.Equal(0x8000_0000_0000_0000UL, BitUtils.ShiftLeft(1, 63, 64));
		Assert.Equal(0UL, BitUtils.ShiftLeft(ulong.MaxValue, 64, 64));
		Assert.Equal(0UL, BitUtils.ShiftRight(ulong.MaxValue, 64, 64));
		Assert.Equal(0UL, BitUtils.ShiftLeft(0xFF, 200, 8));
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
	[InlineData(0u, 0u, 10u, 20u, 30u, 30u)]
	[InlineData(1u, 0u, 10u, 20u, 30u, 20u)]
	[InlineData(1u, 1u, 10u, 20u, 30u, 10u)]
	public void NestedIfCombSelectsByOuterAndInner(uint outer, uint inner, uint a, uint b, uint c, uint expected)
	{
		SimSession<NestedIfComb> m = new NestedIfComb().Run();
		m.Set(m.Module.Outer, outer);
		m.Set(m.Module.Inner, inner);
		m.Set(m.Module.A, a);
		m.Set(m.Module.B, b);
		m.Set(m.Module.C, c);
		m.Settle();
		Assert.Equal(expected, m.Get(m.Module.Y));
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
